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
            if (spriteRenderer.GetComponent<SpriteOverlay>() == null)
            {
                spriteRenderer.gameObject.AddComponent<SpriteOverlay>();
            }
            ;
        }
    }
    [HarmonyPatch(typeof(MainGameManager), "CreateHappyBaldi")]
    class HappyBaldiOverlayPatch
    {
        static void Postfix(MainGameManager __instance)
        {
            var ecField = AccessTools.Field(typeof(BaseGameManager), "ec");
            EnvironmentController ec = (EnvironmentController)ecField.GetValue(__instance);

            var happyBaldi = ec.transform.GetComponentInChildren<HappyBaldi>();
            if (happyBaldi == null) return;

            var spriteRenderer = happyBaldi.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.GetComponent<SpriteOverlay>() == null)
            {
                spriteRenderer.gameObject.AddComponent<SpriteOverlay>();
            }

            var player = ec.Players[0];
            if (player == null) return;

            var tracker = player.gameObject.GetComponent<BaldiTrackerComponent>();
            if (tracker != null)
            {
                tracker.Deactivate();
            }

        }
    }

    [HarmonyPatch(typeof(EndlessGameManager), "CreateHappyBaldi")]
    class EndlessHappyBaldiOverlayPatch
    {
        static void Postfix(MainGameManager __instance)
        {
            var ecField = AccessTools.Field(typeof(BaseGameManager), "ec");
            EnvironmentController ec = (EnvironmentController)ecField.GetValue(__instance);

            var happyBaldi = ec.transform.GetComponentInChildren<HappyBaldi>();
            if (happyBaldi == null) return;

            var spriteRenderer = happyBaldi.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.GetComponent<SpriteOverlay>() == null)
            {
                spriteRenderer.gameObject.AddComponent<SpriteOverlay>();
            }

            var player = ec.Players[0];
            if (player == null) return;

            var tracker = player.gameObject.GetComponent<BaldiTrackerComponent>();
            if (tracker != null)
            {
                tracker.Deactivate();
            }

        }
    }
}