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

namespace BaldiShootCore
{


    [HarmonyPatch(typeof(Baldi), "Initialize")]
    class ReplaceAppleSoundPatch
    {
        static void Prefix(Baldi __instance)
        {
            var assetMan = BasePlugin.assetMan;

            var appleThanksField = AccessTools.Field(typeof(Baldi), "audAppleThanks");
            appleThanksField.SetValue(__instance, assetMan.Get<SoundObject>("BaldiShootSound"));

            var praiseSounds = new WeightedSoundObject[]
            {
                new WeightedSoundObject { selection = assetMan.Get<SoundObject>("BaldiShootSound"), weight = 100 },
                new WeightedSoundObject { selection = assetMan.Get<SoundObject>("BaldiShootSound2"), weight = 100 }
            };
        }
    }
}