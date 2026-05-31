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

            AudioClip shootClip = AssetLoader.AudioClipFromMod(this, "shoot.wav");
            AudioClip BaldiAimingClip = AssetLoader.AudioClipFromMod(this, "BaldiAiming.wav");
            AudioClip BulletHit0Clip = AssetLoader.AudioClipFromMod(this, "BulletHit0.wav");
            AudioClip BulletHit1Clip = AssetLoader.AudioClipFromMod(this, "BulletHit1.wav");

            SoundObject shootSound = ObjectCreators.CreateSoundObject(
                shootClip,
                "*BANG*",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BaldiAimingSound = ObjectCreators.CreateSoundObject(
                BaldiAimingClip,
                "*AIMING*",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BulletHit0Sound = ObjectCreators.CreateSoundObject(
                BulletHit0Clip,
                "*HIT*",
                SoundType.Effect,
                Color.red
            );
            shootSound.subtitle = true;

            SoundObject BulletHit1Sound = ObjectCreators.CreateSoundObject(
                BulletHit1Clip,
                "*HIT*",
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

            yield break;
        }


        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}