using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.Components;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BaldiTestMod
{
    /// <summary>
    /// Test item that shoots a gum projectile in the direction the player is looking.
    /// Uses the vanilla Gum prefab and patches it to work without a Beans dependency.
    /// </summary>
    public class TestItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            base.Use(pm);

            // Search through ALL loaded assets (including disabled prefabs) for the Gum prefab
            // This is necessary because Resources.FindObjectsOfTypeAll is the only way
            // to find a prefab that isn't instantiated in the scene yet.
            // We identify the prefab by checking if it doesn't belong to any loaded scene.
            GameObject? gumPrefab = null;
            foreach (Gum gum in Resources.FindObjectsOfTypeAll<Gum>())
            {
                if (gum.gameObject.scene.name == null) // Prefabs have no scene name
                {
                    gumPrefab = gum.gameObject;
                    break;
                }
            }

            if (gumPrefab == null)
            {
                Debug.LogError("GumPrefab not found");
                return false;
            }

            // Spawn the gum slightly in front of the player, facing the same direction
            Vector3 spawnPos = pm.transform.position + pm.transform.forward * 6f;
            GameObject gumObject = Instantiate(gumPrefab, spawnPos, pm.transform.rotation);

            // Get the Gum component from the prefab (may be on a child object)
            // Initialize it with the EnvironmentController and null Beans
            // (null beans triggers the Harmony patches to handle collision logic)
            Gum gumComponent = gumObject.GetComponentInChildren<Gum>(true);
            gumComponent?.Initialize(pm.ec, null);

            // Launch the gum using its built-in Reset method
            // This sets the gum into a "flying" state, moving in the direction it's facing
            gumComponent?.Reset(pm.plm.Entity);

            return true;
        }

        /// <summary>
        /// Patches the vanilla Gum class to prevent NullReferenceException when beans is null.
        /// Without these patches, the game crashes because Gum methods try to access this.beans
        /// (which is null when the gum is created by the player, not by Beans the NPC).
        ///
        /// Key lesson: Instead of creating a completely new projectile from scratch (which would
        /// require recreating sprites, colliders, audio, etc.), we patch the existing Gum prefab
        /// to make it work independently of Beans. Always prefer patching an existing GameObjects
        /// over building one from scratch when doing Unity modding.
        /// </summary>
        [HarmonyPatch(typeof(Gum))]
        class GumNullSafetyPatch
        {
            /// <summary>
            /// Returns true if this gum was created by the player (beans is null),
            /// and therefore needs our custom collision handling.
            /// </summary>
            static bool NeedsProtection(Gum gum)
            {
                var beansField = AccessTools.Field(typeof(Gum), "beans");
                Beans beans = (Beans)beansField.GetValue(gum);
                return beans == null;
            }

            /// <summary>
            /// Block the original EntityTriggerExit if beans is null.
            /// The original method checks "if we exited Beans' collider", which makes no sense
            /// when beans doesn't exist. Blocking it prevents the NullReferenceException.
            /// </summary>
            [HarmonyPatch("EntityTriggerExit")]
            [HarmonyPrefix]
            static bool EntityTriggerExitPrefix(Gum __instance) => !NeedsProtection(__instance);

            /// <summary>
            /// Handle wall collision for player-created gum.
            /// The original method tries to call beans.GumHit(), which fails when beans is null.
            /// Our version freezes the gum on the wall and destroys it after a delay.
            /// </summary>
            [HarmonyPatch("OnEntityMoveCollision")]
            [HarmonyPrefix]
            static bool OnEntityMoveCollisionPrefix(Gum __instance, RaycastHit hit)
            {
                // If beans is not null, this is a regular Beans gum — let the original code run
                if (!NeedsProtection(__instance)) return true;

                // For player-created gum: freeze it on the wall surface
                var entityField = AccessTools.Field(typeof(Gum), "entity");
                Entity entity = (Entity)entityField.GetValue(__instance);

                entity?.SetFrozen(true);
                // Rotate to "stick" flat against the wall surface
                __instance.transform.rotation = Quaternion.LookRotation(hit.normal * -1f, Vector3.up);
                __instance.StartCoroutine(DestroyAfterDelay(__instance.gameObject, 10f));

                return false; // Don't execute original method
            }

            /// <summary>
            /// Handle NPC/Player collision for player-created gum.
            /// The original method calls beans.HitPlayer() or beans.HitNPC(), which fails when beans is null.
            /// Our version applies the slowing effect directly and destroys the gum.
            ///
            /// Note: We access the private "flying" field via AccessTools because it's [SerializeField] private
            /// and we need to stop the gum from moving once it sticks to a target.
            /// </summary>
            [HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
            [HarmonyPrefix]
            static bool EntityTriggerEnterPrefix(Gum __instance, Entity otherEntity, Collider other, bool validCollision)
            {
                if (!NeedsProtection(__instance)) return true;
                if (!validCollision) return false;

                // Read the private "flying" field to check if gum is still in flight
                var flyingField = AccessTools.Field(typeof(Gum), "flying");
                bool isFlying = (bool)flyingField.GetValue(__instance);
                if (!isFlying) return false;

                if (other.isTrigger && (other.CompareTag("Player") || other.CompareTag("NPC")))
                {
                    var entityField = AccessTools.Field(typeof(Gum), "entity");
                    Entity entity = (Entity)entityField.GetValue(__instance);

                    // Set flying to false to stop the gum from moving
                    flyingField.SetValue(__instance, false);
                    entity?.SetFrozen(true);

                    // Apply slowdown effect based on target type
                    ActivityModifier actMod = otherEntity.ExternalActivity;
                    if (other.CompareTag("Player"))
                    {
                        actMod.moveMods.Add(new MovementModifier(Vector3.zero, 0.25f));
                    }
                    else
                    {
                        actMod.moveMods.Add(new MovementModifier(Vector3.zero, 0.1f));
                    }

                    __instance.StartCoroutine(DestroyAfterDelay(__instance.gameObject, 10f));
                    return false;
                }

                return false;
            }

            /// <summary>
            /// Helper coroutine to destroy the gum object after a delay.
            /// Used when gum sticks to a wall, NPC, or player.
            /// </summary>
            static System.Collections.IEnumerator DestroyAfterDelay(GameObject obj, float delay)
            {
                yield return new WaitForSeconds(delay);
                UnityEngine.Object.Destroy(obj);
            }
        }

        /// <summary>
        /// Safety patch for Beans methods. These are called from within Gum.EntityTriggerEnter
        /// and would normally crash when beans is null. By blocking them at the Beans level,
        /// we prevent the game from trying to play audio or update state for a Beans that doesn't exist.
        ///
        /// Note: These patches technically aren't needed after our EntityTriggerEnter patch,
        /// but they serve as an additional safety net if any other code path tries to call
        /// these methods on a null Beans reference.
        /// </summary>
        [HarmonyPatch(typeof(Beans))]
        class BeansNullSafetyPatch
        {
            [HarmonyPatch("HitPlayer")]
            [HarmonyPrefix]
            static bool HitPlayerPrefix(Beans __instance) => __instance != null;

            [HarmonyPatch("HitNPC")]
            [HarmonyPrefix]
            static bool HitNPCPrefix(Beans __instance) => __instance != null;

            [HarmonyPatch("GumHit")]
            [HarmonyPrefix]
            static bool GumHitPrefix(Beans __instance) => __instance != null;
        }
    }
}