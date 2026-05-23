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
    [BepInPlugin("anton.chigurh.texturepack.mod.setup", "Anton Chigurh Texture pack mod", "1.0.0.0")]

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


            Texture2D AntonChigurhAppleSheets = AssetLoader.TextureFromMod(this, "anton_chigurhsheetsapple.png");
            Texture2D AntonChigurhSlapsSheets = AssetLoader.TextureFromMod(this, "anton_chigurhslaps.png");
            Texture2D AntonChigurhTutorialSheets = AssetLoader.TextureFromMod(this, "anton_chigurhtutorial.png");
            Texture2D AntonChigurhCountdownSheets = AssetLoader.TextureFromMod(this, "anton_chigurhsheetscountdown.png");
            Sprite placeholder2 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder3.png");
            Sprite placeholder3 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder4.png");
            Sprite AntonChigurhIdleSprite = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 35f, "anton_chigurhidle.png");

            assetMan.Add<Sprite>("placeholder3", placeholder2);
            assetMan.Add<Sprite>("placeholder4", placeholder3);
            assetMan.Add<Sprite>("AntonChigurhIdle", AntonChigurhIdleSprite);

            Sprite[] AntonChigurhSlapsSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhSlapsSheets
            );
            Sprite[] AntonChigurhSlapsSprites = AntonChigurhSlapsSpritesArray.Take(5).ToArray();

            Sprite[] AntonChigurhTutorialSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhTutorialSheets
            );
            Sprite[] AntonChigurhTutorialSprites = AntonChigurhTutorialSpritesArray.Take(7).ToArray();

            Sprite[] AntonChigurhAppleSprites = AssetLoader.SpritesFromSpritesheet(
                2,
                1,
                35f,
                Vector2.one / 2f,
                AntonChigurhAppleSheets
            );

            Sprite[] AntonChigurhCountdownSprites = AssetLoader.SpritesFromSpritesheet(
                3,
                1,
                35f,
                Vector2.one / 2f,
                AntonChigurhCountdownSheets
            );

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

            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_0"] = AntonChigurhTutorialSprites[0];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_1"] = AntonChigurhTutorialSprites[1];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_2"] = AntonChigurhTutorialSprites[2];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_3"] = AntonChigurhTutorialSprites[3];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_4"] = AntonChigurhTutorialSprites[4];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_5"] = AntonChigurhTutorialSprites[5];
            currentSpriteReplacements["Baldi_Talk_Standing_Sheet_6"] = AntonChigurhTutorialSprites[6];

            currentSpriteReplacements["BaldiApple_0"] = AntonChigurhAppleSprites[0];
            currentSpriteReplacements["BaldiApple_1"] = AntonChigurhAppleSprites[1];

            currentSpriteReplacements["BAL_Countdown_Sheet_0"] = AntonChigurhCountdownSprites[0];
            currentSpriteReplacements["BAL_Countdown_Sheet_1"] = AntonChigurhCountdownSprites[1];
            currentSpriteReplacements["BAL_Countdown_Sheet_2"] = AntonChigurhCountdownSprites[2];

            for (int i = 0; i <= 99; i++)
            {
                string spriteName = $"Baldi_Wave{i:D4}"; // D3 = 0000, 0001, ... 0099
                currentSpriteReplacements[spriteName] = AntonChigurhIdleSprite; // boiiiiii i wont replace all his spritessss
            }


            yield break;
        }


        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}