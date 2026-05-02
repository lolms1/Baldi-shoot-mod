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
    /// <summary>
    /// Patches Baldi's PlayerInSight method to freeze him in place when he spots the player.
    /// This demonstrates how to intercept NPC behavior states and modify them.
    ///
    /// The private "navigator" field is accessed via AccessTools because it's not publicly exposed.
    /// </summary>
    [HarmonyPatch(typeof(Baldi_StateBase))]
    [HarmonyPatch("PlayerInSight")]
    class BaldiFreezeWhenSeenPatch
    {
        static bool Prefix(Baldi_StateBase __instance, PlayerManager player)
        {
            // Get the Baldi instance from the state base (protected field)
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);

            // Get the Navigator component (private field on Baldi)
            var navigatorField = AccessTools.Field(typeof(Baldi), "navigator");
            Navigator navigator = (Navigator)navigatorField.GetValue(baldi);

            // Store the original max speed for potential future unfreezing
            float originalSpeed = navigator.maxSpeed;
            // Set both speed and maxSpeed to 0 to completely stop movement
            navigator.speed = 0;
            navigator.maxSpeed = 0;

            // Return false to prevent Baldi from starting his chase behavior
            return false;
        }

        /// <summary>
        /// Coroutine to unfreeze Baldi after a delay.
        /// Currently unused, but kept as a reference for implementing timed freeze mechanics.
        /// </summary>
        static IEnumerator UnfreezeAfterDelay(Navigator navigator, float originalSpeed, float delay)
        {
            yield return new WaitForSeconds(delay);
            navigator.maxSpeed = originalSpeed;
            navigator.speed = originalSpeed;
        }
    }

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
    [HarmonyPatch(typeof(Baldi))]
    [HarmonyPatch("GetAngry")]
    class BaldiChangingAngry
    {
        static bool Prefix(Baldi __instance)
        {
            var angerField = AccessTools.Field(typeof(Baldi), "anger");
            float anger = (float)angerField.GetValue(__instance);
            anger += 10f;
            // Must call SetValue because float is a value type — GetValue returns a copy
            angerField.SetValue(__instance, anger);
            return false; // Prevent original GetAngry from executing
        }
    }
}