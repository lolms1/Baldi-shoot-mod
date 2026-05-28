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
        private static RawImage activeOverlay;
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

            var overlay = baldiImage.transform.Find("BaldicatorOverlay")?.GetComponent<RawImage>();
            if (overlay == null)
            {
                var overlayObj = new GameObject("BaldicatorOverlay");
                overlayObj.transform.SetParent(baldiImage.transform, false);
                overlayObj.transform.SetAsLastSibling();
                overlay = overlayObj.AddComponent<RawImage>();

                var overlayRect = overlayObj.GetComponent<RectTransform>();
                var originalRect = baldiImage.GetComponent<RectTransform>();
                overlayRect.sizeDelta = originalRect.sizeDelta / 512f;
                overlayRect.anchoredPosition = Vector2.zero;
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

        private static IEnumerator OverrideSpritesDuringAnimation(RawImage overlay, Image original, string animationName, Animator animator)
        {
            Sprite[] frames = animationName == "Baldicator_Look"
                ? BasePlugin.assetMan.Get<Sprite[]>("ok")
                : BasePlugin.assetMan.Get<Sprite[]>("ok2");

            if (frames == null || frames.Length == 0) yield break;

            float frameTime = 0.033f;
            float duration = 2.3f;
            float elapsed = 0f;
            int lastFrame = -1;

            while (elapsed < duration && animator != null)
            {
                int frameIndex = Mathf.Min((int)(elapsed / frameTime), frames.Length - 1);

                if (frameIndex != lastFrame && overlay != null)
                {
                    overlay.texture = frames[frameIndex].texture;
                    lastFrame = frameIndex;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (overlay != null && frames.Length > 0)
            {
                overlay.texture = frames[frames.Length - 1].texture;
            }

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