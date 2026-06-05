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
    [BepInPlugin("lolms.bbplusmod.baldishootmod", "Baldi shoot mod", "1.0.0.0")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; private set; }

        public static AssetManager assetMan = new AssetManager();

        public static Dictionary<string, Sprite> currentSpriteReplacements = new Dictionary<string, Sprite>();

        public void Awake()
        {
            Instance = this;
            BaldiShootCfgLoader.LoadAndApply();

            Harmony harmony = new Harmony("lolms.bbplusmod.baldishootmod");

            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMyAssets(), LoadingEventOrder.Start);
        }
        IEnumerator LoadMyAssets()
        {
            yield return 3;

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

            AudioClip shootClip = AssetLoader.AudioClipFromMod(this, "shoot.wav");
            AudioClip BaldiAimingClip = AssetLoader.AudioClipFromMod(this, "BaldiAiming.wav");
            AudioClip BulletHit0Clip = AssetLoader.AudioClipFromMod(this, "BulletHit0.wav");
            AudioClip BulletHit1Clip = AssetLoader.AudioClipFromMod(this, "BulletHit1.wav");

            SoundObject shootSound = ObjectCreators.CreateSoundObject(
                shootClip,
                "Sfx_Baldi_Shoot_Shoot",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BaldiAimingSound = ObjectCreators.CreateSoundObject(
                BaldiAimingClip,
                "Sfx_Baldi_Shoot_Aim",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BulletHit0Sound = ObjectCreators.CreateSoundObject(
                BulletHit0Clip,
                "Sfx_Baldi_Shoot_Hit",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BulletHit1Sound = ObjectCreators.CreateSoundObject(
                BulletHit1Clip,
                "Sfx_Baldi_Shoot_Hit",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject[] BulletHitsSounds = new SoundObject[] {
                BulletHit0Sound,
                BulletHit1Sound,
            };

            assetMan.Add<SoundObject>("BaldiShootSound", shootSound);
            assetMan.Add<SoundObject>("BaldiAimingSound", BaldiAimingSound);
            assetMan.Add<SoundObject[]>("BulletHitsSounds", BulletHitsSounds);

            yield return "Localization Loading...";

            string LocalizationPath = Path.Combine(AssetLoader.GetModPath(BasePlugin.Instance), "Subtitles_English.json");
            AssetLoader.LocalizationFromFile(LocalizationPath, Language.English);


            yield break;
        }


        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}