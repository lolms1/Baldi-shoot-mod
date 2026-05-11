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
            anger += 4f;
            // Must call SetValue because float is a value type — GetValue returns a copy
            angerField.SetValue(__instance, anger);
            return false; // Prevent original GetAngry from executing
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
}
