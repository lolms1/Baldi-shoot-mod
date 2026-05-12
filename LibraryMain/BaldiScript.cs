using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Components.Animation;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BaldiTestMod
{

    /// <summary>
    /// Patches Baldi's GetAngry method to increase his anger by a flat amount.
    /// Demonstrates how to modify private fields using AccessTools.SetValue.
    ///
    /// Important: Because "anger" is a float (value type), we must:
    /// 1. GetValue to obtain a copy
    /// 2. Modify the copy
    /// 3. SetValue to write the modified copy back
    /// This is different from reference types (like Navigator) where GetValue returns
    /// a reference and you can modify it directly without SetValue.
    /// </summary>
    [HarmonyPatch(typeof(Baldi), "GetAngry")]
    class BaldiChangingAngry
    {
        static bool Prefix(Baldi __instance)
        {
            var angerField = AccessTools.Field(typeof(Baldi), "anger");
            float anger = (float)angerField.GetValue(__instance);
            anger += 0f;
            angerField.SetValue(__instance, anger);
            return false;
        }
    }
    [HarmonyPatch(typeof(Baldi), "Initialize")]
    class BaldiSpriteOverlayPatch
    {
        static void Postfix(Baldi __instance)
        {
            var spriteRenderer = __instance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("[BaldiReskin] No SpriteRenderer found on Baldi!");
                return;
            }

            if (spriteRenderer.GetComponent<SpriteOverlay>() != null) return;

            spriteRenderer.gameObject.AddComponent<SpriteOverlay>();
        }
    }

    [HarmonyPatch(typeof(Baldi_StateBase))]
    [HarmonyPatch("PlayerInSight")]
    class BaldiShootPatch
    {
        static float lastShootTime = -999f;
        const float cooldown = 10f;

        static void Postfix(Baldi_StateBase __instance)
        {
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);
            if (baldi == null) return;

            if (Time.time - lastShootTime >= cooldown)
            {
                lastShootTime = Time.time;
                baldi.ShootAtPlayer();
            }
        }
    }

    public static class BaldiShootExtensions
    {
        public static void ShootAtPlayer(this Baldi baldi)
        {
            baldi.StartCoroutine(ShootSequence(baldi));
        }

        private static IEnumerator ShootSequence(Baldi baldi)
        {
            Sprite shootSprite = BasePlugin.assetMan.Get<Sprite>("placeholder4");

            var angerField = AccessTools.Field(typeof(Baldi), "animator");
            Animator animator = (Animator)angerField.GetValue(baldi);

            PlayerManager player = baldi.ec.Players[0];
            if (player == null) yield break;

            Vector3 baldiPos = baldi.transform.position + Vector3.up * 2f;
            Vector3 playerPos = player.transform.position;
            Vector3 direction = (playerPos - baldiPos).normalized;

            Vector3[] bulletDirections = new Vector3[3];

            for (int i = 0; i < 3; i++)
            {
                Vector3 CurrentplayerPos = player.transform.position;
                Vector3 baseDirection = (CurrentplayerPos - baldiPos).normalized;

                float randomAngleX = UnityEngine.Random.Range(-0.5f, 0.5f); 
                float randomAngleY = UnityEngine.Random.Range(-0.5f, 0.5f); 

                Vector3 laserDirection = Quaternion.Euler(randomAngleX, randomAngleY, 0f) * baseDirection;

                Vector3 startPos = baldiPos;

                CreateGameobject.CreateLaserBeam(startPos, laserDirection, 43333f, Color.red);

                bulletDirections[i] = laserDirection;

                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < 3; i++)
            {
                Vector3 bulletStartPos = baldiPos + baldi.transform.right * (i - 1) * 0.3f;

                CreateGameobject.CreateBullet(bulletStartPos, bulletDirections[i], 20f, new Color(1f, 0.5f, 0f));

                if (i < 2)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }
       /* private static IEnumerator PlayOneFrameAnimation(Baldi baldi, Sprite frame, float duration)
        {
            var spriteRenderer = baldi.GetComponentInChildren<SpriteRenderer>();
            Debug.LogWarning("getting spriterendere");
            if (spriteRenderer == null) yield break;
            Debug.LogWarning("NOT got null on spriterender");

            Sprite originalSprite = spriteRenderer.sprite;
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(duration);
            spriteRenderer.sprite = originalSprite;
            Debug.LogWarning("did reset sprite");
        }
       */
    }
}
