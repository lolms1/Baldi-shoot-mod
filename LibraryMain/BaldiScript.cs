using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BaldiTestMod
{
    [HarmonyPatch(typeof(Baldi_StateBase))]
    [HarmonyPatch("PlayerInSight")]
    class BaldiFreezeWhenSeenPatch
    {
        static bool Prefix(Baldi_StateBase __instance, PlayerManager player)
        {
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);

            var navigatorField = AccessTools.Field(typeof(Baldi), "navigator");
            Navigator navigator = (Navigator)navigatorField.GetValue(baldi);

            float originalSpeed = navigator.maxSpeed; 
            navigator.speed = 0f;
            navigator.maxSpeed = 0f;

            baldi.StartCoroutine(UnfreezeAfterDelay(navigator, originalSpeed, 3f));

            return false;
        }

        static IEnumerator UnfreezeAfterDelay(Navigator navigator, float originalSpeed, float delay)
        {
            yield return new WaitForSeconds(delay);
            navigator.maxSpeed = originalSpeed;
            navigator.speed = originalSpeed;
        }
    }
}