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
    [HarmonyPatch(typeof(Baldi))]
    [HarmonyPatch("GetAngry")]
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
    [HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(string), typeof(int), typeof(float) })]
    class BaldiSlapReskinPatch
    {
        static void Postfix(Animator __instance, string stateName, int layer, float normalizedTime)
        {
            if (__instance.gameObject.name.Contains("Baldi") && stateName == "BAL_Slap")
            {
                var spriteRenderer = __instance.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = BasePlugin.assetMan.Get<Sprite>("Test_Sprite_Big");
                    __instance.enabled = false;

                    __instance.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(
                        ReenableAnimator(__instance, 1f)
                    );
                }
            }
        }

        static IEnumerator ReenableAnimator(Animator animator, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.enabled = true;
        }
    }
}
