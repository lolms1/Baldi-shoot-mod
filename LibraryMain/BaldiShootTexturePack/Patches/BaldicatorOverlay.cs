using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BaldiShootTexturePack
{
    [HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(string), typeof(int), typeof(float) })]
    class BaldicatorAnimatorPatch
    {
        private static Coroutine activeOverlayCoroutine;
        private static Image activeOverlay;
        private static Image activeOriginal;

        static void Postfix(Animator __instance, string stateName)
        {
            if (stateName != "Baldicator_Look" && stateName != "Baldicator_Think") return;

            var baldiImage = __instance.transform.Find("Baldi")?.GetComponent<Image>();
            if (baldiImage == null) return;

            var hudManager = __instance.GetComponentInParent<HudManager>();
            if (activeOverlayCoroutine != null && hudManager != null)
            {
                hudManager.StopCoroutine(activeOverlayCoroutine);
                activeOverlayCoroutine = null;
            }

            if (activeOriginal != null)
            {
                activeOriginal.enabled = true;
            }

            baldiImage.enabled = false;
            activeOriginal = baldiImage;

            var overlay = baldiImage.transform.Find("BaldicatorOverlay")?.GetComponent<Image>();
            if (overlay == null)
            {
                var overlayObj = new GameObject("BaldicatorOverlay");
                overlayObj.transform.SetParent(baldiImage.transform, false);
                overlayObj.transform.SetAsLastSibling();
                overlayObj.transform.localScale = new Vector3(0.42f, 0.42f, 1f); // actually good scale is 0.5f, 0.5f, but my sprites is not completly going down well yeah 
                overlay = overlayObj.AddComponent<Image>();

                var overlayRect = overlayObj.GetComponent<RectTransform>();
                var originalRect = baldiImage.GetComponent<RectTransform>();
                overlayRect.sizeDelta = originalRect.sizeDelta;
                overlayRect.anchoredPosition = Vector2.zero / 16f;
                overlayRect.anchorMin = Vector2.zero;
                overlayRect.anchorMax = Vector2.one;
            }

            overlay.enabled = true;
            activeOverlay = overlay;

            if (hudManager != null)
            {
                activeOverlayCoroutine = hudManager.StartCoroutine(
                    OverrideSpritesDuringAnimation(overlay, baldiImage, stateName, __instance)
                );
            }
        }

        private static IEnumerator OverrideSpritesDuringAnimation(Image overlay, Image original, string animationName, Animator animator)
        {
            Sprite[] frames = animationName == "Baldicator_Look"
                ? BasePlugin.assetMan.Get<Sprite[]>("BaldicatorLook")
                : BasePlugin.assetMan.Get<Sprite[]>("BaldicatorThink");

            if (frames == null || frames.Length == 0) yield break;

            float frameTime = 0.033f;

            for (int i = 0; i < frames.Length - 1; i++)
            {
                overlay.sprite = frames[i];
                yield return new WaitForSeconds(frameTime);
            }

            yield return new WaitForSeconds(0.8f);

            overlay.sprite = frames[frames.Length - 1];

            yield return new WaitForSeconds(1f);

            CleanupOverlay();
        }

        private static void CleanupOverlay()
        {
            if (activeOriginal != null)
            {
                activeOriginal.enabled = true;
                activeOriginal = null;
            }
            if (activeOverlay != null)
            {
                activeOverlay.enabled = false;
                activeOverlay = null;
            }
            activeOverlayCoroutine = null;
        }
    }
}