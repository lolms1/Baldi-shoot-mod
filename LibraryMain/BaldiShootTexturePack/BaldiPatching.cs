using HarmonyLib;
using MTM101BaldAPI;
using Rewired;
using System.Reflection;
using Unity.Mathematics;
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
            };
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
    [HarmonyPatch(typeof(Baldi), "SlapNormal")]
    class ReplaceSlapSoundPatch
    { 
        static void Prefix(Baldi __instance)
        {
            SoundObject[] sounds = BasePlugin.assetMan.Get<SoundObject[]>("stepSounds");
            var randomSound = sounds[UnityEngine.Random.Range(0, sounds.Length)];

            var slapField = AccessTools.Field(typeof(Baldi), "slap");
            slapField.SetValue(__instance, randomSound);
        }
    }

    [HarmonyPatch(typeof(Baldi), "SlapBroken")]
    class AddBrokenRulerSoundPatch
    {
        static void Prefix(Baldi __instance)
        {
            SoundObject[] sounds = BasePlugin.assetMan.Get<SoundObject[]>("stepSounds");
            var randomSound = sounds[UnityEngine.Random.Range(0, sounds.Length)];

            var audManField = AccessTools.Field(typeof(Baldi), "audMan");
            AudioManager audMan = (AudioManager)audManField.GetValue(__instance);

            audMan.PlaySingle(randomSound);
        }
    }

    [HarmonyPatch(typeof(Baldi_Chase), "Update")]
    class BaldiTrackerChasePatch
    {
        static void Postfix(Baldi_Chase __instance)
        {
            Baldi baldi = (Baldi)AccessTools.Field(typeof(Baldi_StateBase), "baldi").GetValue(__instance);
            if (baldi == null) return;

            var player = baldi.ec.Players[0];
            if (player == null) return;

            var tracker = player.gameObject.GetComponent<BaldiTrackerComponent>();
            if (tracker == null || !tracker.IsActive) return;

            baldi.ClearSoundLocations();
            baldi.Hear(null, player.transform.position, 127, false);
        }
    }
}