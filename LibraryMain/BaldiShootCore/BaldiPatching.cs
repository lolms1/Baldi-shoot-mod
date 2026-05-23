using HarmonyLib;
using UnityEngine;

namespace BaldiShootCore
{
    /// <summary>
    /// Patches Baldi's GetAngry method.
    /// Currently modifies anger by 0 (placeholder for future balancing).
    /// Demonstrates AccessTools for reading/writing private value-type fields.
    /// 
    /// Value-type note: float is a value type, so we must:
    /// 1. GetValue (returns a copy)
    /// 2. Modify the copy
    /// 3. SetValue (writes the copy back to the original field)
    /// </summary>
    [HarmonyPatch(typeof(Baldi), "GetAngry")]
    class BaldiChangingAngry
    {
        static bool Prefix(Baldi __instance)
        {
            var angerField = AccessTools.Field(typeof(Baldi), "anger");
            float anger = (float)angerField.GetValue(__instance);
            anger += 0f; // Placeholder — change this value to alter Baldi's anger
            angerField.SetValue(__instance, anger);
            return false; // Skip original GetAngry method
        }
    }

    /// <summary>
    /// Patches Baldi's PlayerInSight to trigger the shooting state.
    /// When Baldi spots the player and the cooldown has elapsed,
    /// he enters Baldi_ShootState instead of immediately chasing.
    /// 
    /// Cooldown: 10 seconds between shots.
    /// Does NOT interrupt if Baldi is already in Praise or Shoot state.
    /// </summary>
    [HarmonyPatch(typeof(Baldi_StateBase))]
    [HarmonyPatch("PlayerInSight")]
    class BaldiShootPatch
    {
        static float lastShootTime = -999f;
        const float cooldown = 20f;

        static void Postfix(Baldi_StateBase __instance)
        {
            // Extract the Baldi instance from the state base's protected field
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);
            if (baldi == null) return;

            // Don't interrupt praise or an already-running shooting sequence
            NpcState currentState = baldi.behaviorStateMachine.CurrentState;
            if (currentState is Baldi_Praise || currentState is Baldi_ShootState)
            {
                return;
            }

            // Check cooldown
            if (Time.time - lastShootTime >= cooldown)
            {
                lastShootTime = Time.time;

                // Create and enter the shooting state
                Baldi_ShootState shootState = new Baldi_ShootState(
                    baldi,           // NPC reference
                    baldi,           // Baldi reference
                    currentState,    // State to return to after shooting
                    4f               // State duration in seconds
                );

                baldi.behaviorStateMachine.ChangeState(shootState);
            }
        }
    }
    [HarmonyPatch(typeof(NpcStateMachine), "ChangeState")]
    class FixPreviousStatePatch
    {
        static void Prefix(NpcStateMachine __instance, NpcState newState)
        {
            if (__instance.CurrentState is Baldi_ShootState shootState
                && !(newState is Baldi_ShootState))
            {
                shootState.Exit();
                var previousStateField = AccessTools.Field(typeof(Baldi_SubState), "previousState");
                var stateBeforeShoot = (NpcState)previousStateField.GetValue(shootState);

                __instance.currentState = stateBeforeShoot;
            }
        }
    }
}