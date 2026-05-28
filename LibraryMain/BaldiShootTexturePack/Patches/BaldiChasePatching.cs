using HarmonyLib;

namespace BaldiShootTexturePack
{

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