using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Components.Animation;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BaldiShootTexturePack
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
            yield return 3;

            yield return "Loading Anton Chigurh sprites...";


            Sprite small = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 25f, "placeholder.png");
            Texture2D AntonChigurhSlapsSheets = AssetLoader.TextureFromMod(this, "anton_chigurhslaps.png");
            Sprite placeholder = AssetLoader.SpriteFromMod(this, Vector2.one / 5f, 50f, "placeholder2.png");
            Sprite placeholder2 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder3.png");
            Sprite placeholder3 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder4.png");

            assetMan.Add<Sprite>("Test_Sprite_Small", small);
            assetMan.Add<Sprite>("placeholder2", placeholder);
            assetMan.Add<Sprite>("placeholder3", placeholder2);
            assetMan.Add<Sprite>("placeholder4", placeholder3);

            Sprite[] AntonChigurhSlapsSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhSlapsSheets
            );
            Sprite[] AntonChigurhSlapsSprites = AntonChigurhSlapsSpritesArray.Take(5).ToArray();

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

            yield return "Replacing Baldi's sprites...";

            currentSpriteReplacements["Slap_Sheet_0"] = AntonChigurhSlapsSprites[0];
            currentSpriteReplacements["Slap_Sheet_1"] = AntonChigurhSlapsSprites[1];
            currentSpriteReplacements["Slap_Sheet_2"] = AntonChigurhSlapsSprites[2];
            currentSpriteReplacements["Slap_Sheet_3"] = AntonChigurhSlapsSprites[3];
            currentSpriteReplacements["Slap_Sheet_4"] = AntonChigurhSlapsSprites[4];

            yield break;
        }


        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}