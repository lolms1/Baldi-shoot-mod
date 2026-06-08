using HarmonyLib;
using UnityEngine;

namespace BaldiShootCore
{

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

        static void Postfix(Baldi_StateBase __instance)
        {
            // Extract the Baldi instance from the state base's protected field
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);
            if (baldi == null) return;

            var angerField = AccessTools.Field(typeof(Baldi), "anger");
            float anger = (float)angerField.GetValue(baldi);

            float cooldown = BaldiShootCfg.ShootCooldown;
            float cooldowncoefficient = BaldiShootCfg.CooldownCoefficient;
            float cooldowmMultiplierLogBase = BaldiShootCfg.CooldownMultiplierLogBase;
            float cooldownStarterAnger = BaldiShootCfg.CooldownStarterAnger;
            float cooldownMultiplier = (Mathf.Log(cooldownStarterAnger + anger, cooldowmMultiplierLogBase) * cooldowncoefficient);
            float currentcooldown = cooldown / cooldownMultiplier;

            bool ignoreStuns = BaldiShootCfg.IgnoreStuns;

            // Don't interrupt praise or an already-running shooting sequence
            NpcState currentState = baldi.behaviorStateMachine.CurrentState;
            if (currentState is Baldi_Praise || currentState is Baldi_ShootState || currentState is Baldi_Chase_Broken || currentState is Baldi_Apple)
            {
                if (!ignoreStuns || currentState is Baldi_Chase_Broken) return;
            }

            // Check cooldown
            if (Time.time - lastShootTime >= currentcooldown)
            {
                lastShootTime = Time.time;

                // Create and enter the shooting state
                Baldi_ShootState shootState = new Baldi_ShootState(
                    baldi,           // NPC reference
                    baldi,           // Baldi reference
                    currentState    // State to return to after shooting
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