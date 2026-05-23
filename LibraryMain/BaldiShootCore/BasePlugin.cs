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
using UnityEngine.Rendering;

namespace BaldiShootCore
{
    [BepInPlugin("anton.chigurh.mod.setup", "Anton Chigurh mod", "1.0.0.0")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static AssetManager assetMan = new AssetManager();

        public static Dictionary<string, Sprite> currentSpriteReplacements = new Dictionary<string, Sprite>();

        public void Awake()
        {
            Harmony harmony = new Harmony("anton.chigurh.mod.setup");

            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMyAssets(), LoadingEventOrder.Start);
        }
        IEnumerator LoadMyAssets()
        {
            yield return 2;

            yield return "Loading Baldi shooting  sprites...";

            Texture2D AntonChigurhShootingSheets = AssetLoader.TextureFromMod(this, "anton_chigurhshooting.png");

            Sprite[] AntonChigurhShootingSprites = AssetLoader.SpritesFromSpritesheet(
                2,
                1,
                35f,
                Vector2.one / 2f,
                AntonChigurhShootingSheets
            );

            assetMan.Add<Sprite>("BaldiAim", AntonChigurhShootingSprites[0]);
            assetMan.Add<Sprite>("BaldiShoot", AntonChigurhShootingSprites[1]);


            yield return "Loading audio..";

            AudioClip shootClip = AssetLoader.AudioClipFromMod(this, "testing.mp3");
            AudioClip shootClip2 = AssetLoader.AudioClipFromMod(this, "testing2.mp3");

            SoundObject shootSound = ObjectCreators.CreateSoundObject(
                shootClip,
                "Sfx_Baldi_Shoot",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject shootSound2 = ObjectCreators.CreateSoundObject(
                shootClip2,
                "Sfx_Baldi_Shoot",
                SoundType.Effect,
                Color.red
            );
            shootSound2.subtitle = true;
            assetMan.Add<SoundObject>("BaldiShootSound", shootSound);
            assetMan.Add<SoundObject>("BaldiShootSound2", shootSound2);

            yield break;
        }


        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}