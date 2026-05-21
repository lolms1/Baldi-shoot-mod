using HarmonyLib;
using UnityEngine;

namespace BaldiShootTexturePack
{
    [HarmonyPatch(typeof(Baldi), "Initialize")]
    class BaldiSpriteOverlayPatch
    {
        static void Postfix(Baldi __instance)
        {
            // Find ANY SpriteRenderer on Baldi or his children
            var spriteRenderer = __instance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("[BaldiReskin] No SpriteRenderer found on Baldi!");
                return;
            }

            // Don't add duplicate overlays
            if (spriteRenderer.GetComponent<SpriteOverlay>() != null) return;

            spriteRenderer.gameObject.AddComponent<SpriteOverlay>();
        }
    }
}