using HarmonyLib;

namespace BaldiShootTexturePack
{
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
}