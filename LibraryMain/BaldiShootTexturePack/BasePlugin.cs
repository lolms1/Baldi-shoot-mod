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
using System.IO;
using System.Linq;
using UnityEngine;

namespace BaldiShootTexturePack
{
    [BepInPlugin("lolms.bbplusmod.antonchigurhtexturepack", "Anton Chigurh Texture pack mod", "1.0.0.0")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static AssetManager assetMan = new AssetManager();

        public static Dictionary<string, Sprite> currentSpriteReplacements = new Dictionary<string, Sprite>();

        public void Awake()
        {
            Harmony harmony = new Harmony("lolms.bbplusmod.antonchigurhtexturepack");

            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMyAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, GeneratorChanges);
        }

        void GeneratorChanges(string floorName, int levelId, SceneObject obj)
        {
            var trackerItem = assetMan.Get<ItemObject>("BaldiTracker");

            CustomLevelObject[] objects = obj.GetCustomLevelObjects();
            if ((levelId > 0) || ((obj.GetMeta().tags.Contains("endless"))))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    CustomLevelObject levelObj = objects[i];
                    if (levelObj.IsModifiedByMod(Info)) continue;

                    levelObj.potentialItems = levelObj.potentialItems.AddRangeToArray(new WeightedItemObject[] {
                new WeightedItemObject()
                {
                    selection = trackerItem,
                    weight = 20
                }});

                    levelObj.MarkAsModifiedByMod(Info);
                }
            }
            obj.MarkAsNeverUnload();
        }

        IEnumerator LoadMyAssets()
        {
            yield return 5;

            yield return "Loading Anton Chigurh sprites...";


            Texture2D AntonChigurhAppleSheets = AssetLoader.TextureFromMod(this, "anton_chigurhapple.png");
            Texture2D AntonChigurhSlapsSheets = AssetLoader.TextureFromMod(this, "anton_chigurhslaps.png");
            Texture2D AntonChigurhBrokenRulerSheets = AssetLoader.TextureFromMod(this, "anton_chigurhbrokenruler.png");
            Texture2D AntonChigurhTutorialSheets = AssetLoader.TextureFromMod(this, "anton_chigurhtutorial.png");
            Texture2D AntonChigurhCountdownSheets = AssetLoader.TextureFromMod(this, "anton_chigurhcountdown.png");
            Texture2D AntonChigurhBaldicatorSheets = AssetLoader.TextureFromMod(this, "anton_chigurhbaldicator.png");
            Sprite AntonChigurhIdleSprite = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 35f, "anton_chigurhidle.png");
            Sprite SuitcaseIcon = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 100f, "suitcase.png");

            assetMan.Add<Sprite>("AntonChigurhIdle", AntonChigurhIdleSprite);

            Sprite[] AntonChigurhSlapsSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhSlapsSheets
            );
            Sprite[] AntonChigurhSlapsSprites = AntonChigurhSlapsSpritesArray.Take(5).ToArray();

            Sprite[] AntonChigurhBrokenRulerSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhBrokenRulerSheets
            );
            Sprite[] AntonChigurhBrokenRulerSprites = AntonChigurhBrokenRulerSpritesArray.Take(5).ToArray();

            Sprite[] AntonChigurhTutorialSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                2,
                35f,
                Vector2.one / 2f,
                AntonChigurhTutorialSheets
            );
            Sprite[] AntonChigurhTutorialSprites = AntonChigurhTutorialSpritesArray.Take(7).ToArray();

            Sprite[] AntonChigurhBaldicatorSpritesArray = AssetLoader.SpritesFromSpritesheet(
                4,
                4,
                35f,
                Vector2.one / 2f,
                AntonChigurhBaldicatorSheets
            );
            Sprite[] AntonChigurhBaldicatotrSprites = AntonChigurhBaldicatorSpritesArray.Take(13).ToArray();

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

            Sprite[] BaldicatorThink = new Sprite[]
            {
                AntonChigurhBaldicatotrSprites[4],
                AntonChigurhBaldicatotrSprites[4],
                AntonChigurhBaldicatotrSprites[5],
                AntonChigurhBaldicatotrSprites[6],
                AntonChigurhBaldicatotrSprites[7],
                AntonChigurhBaldicatotrSprites[8],
                AntonChigurhBaldicatotrSprites[9],
                AntonChigurhBaldicatotrSprites[10],
                AntonChigurhBaldicatotrSprites[11],
                AntonChigurhBaldicatotrSprites[12]
            };

            Sprite[] BaldicatorLook = new Sprite[]
            {
                AntonChigurhBaldicatotrSprites[4],
                AntonChigurhBaldicatotrSprites[4],
                AntonChigurhBaldicatotrSprites[5],
                AntonChigurhBaldicatotrSprites[6],
                AntonChigurhBaldicatotrSprites[7],
                AntonChigurhBaldicatotrSprites[8],
                AntonChigurhBaldicatotrSprites[9],
                AntonChigurhBaldicatotrSprites[10],
                AntonChigurhBaldicatotrSprites[11],
                AntonChigurhBaldicatotrSprites[0]
            };

            assetMan.Add<Sprite[]>("BaldicatorThink", BaldicatorThink);
            assetMan.Add<Sprite[]>("BaldicatorLook", BaldicatorLook);


            yield return "Loading audio..";

            AudioClip stepClip0 = AssetLoader.AudioClipFromMod(this, "step0.wav");
            AudioClip stepClip1 = AssetLoader.AudioClipFromMod(this, "step1.wav");
            AudioClip stepClip2 = AssetLoader.AudioClipFromMod(this, "step2.wav");
            AudioClip stepClip3 = AssetLoader.AudioClipFromMod(this, "step3.wav");
            AudioClip stepClip4 = AssetLoader.AudioClipFromMod(this, "step4.wav");

            SoundObject stepSound0 = ObjectCreators.CreateSoundObject(
                stepClip0,
                "Sfx_Anton_Chigurh_Step",
                SoundType.Effect,
                Color.black
            );

            SoundObject stepSound1 = ObjectCreators.CreateSoundObject(
                stepClip1,
                "Sfx_Anton_Chigurh_Step",
                SoundType.Effect,
                Color.black
            );

            SoundObject stepSound2 = ObjectCreators.CreateSoundObject(
                stepClip2,
                "Sfx_Anton_Chigurh_Step",
                SoundType.Effect,
                Color.black
            );

            SoundObject stepSound3 = ObjectCreators.CreateSoundObject(
                stepClip3,
                "Sfx_Anton_Chigurh_Step",
                SoundType.Effect,
                Color.black
            );

            SoundObject stepSound4 = ObjectCreators.CreateSoundObject(
                stepClip4,
                "Sfx_Anton_Chigurh_Step",
                SoundType.Effect,
                Color.black
            );

            assetMan.Add<SoundObject>("stepSound0", stepSound0);
            assetMan.Add<SoundObject>("stepSound1", stepSound1);
            assetMan.Add<SoundObject>("stepSound2", stepSound2);
            assetMan.Add<SoundObject>("stepSound3", stepSound3);
            assetMan.Add<SoundObject>("stepSound4", stepSound4);

            var stepSounds = new SoundObject[]
            {
                BasePlugin.assetMan.Get<SoundObject>("stepSound0"),
                BasePlugin.assetMan.Get<SoundObject>("stepSound1"),
                BasePlugin.assetMan.Get<SoundObject>("stepSound2"),
                BasePlugin.assetMan.Get<SoundObject>("stepSound3"),
                BasePlugin.assetMan.Get<SoundObject>("stepSound4")
            };

            assetMan.Add<SoundObject[]>("stepSounds", stepSounds);

            yield return "Replacing Baldi's sprites...";

            currentSpriteReplacements["Slap_Sheet_0"] = AntonChigurhSlapsSprites[0];
            currentSpriteReplacements["Slap_Sheet_1"] = AntonChigurhSlapsSprites[1];
            currentSpriteReplacements["Slap_Sheet_2"] = AntonChigurhSlapsSprites[2];
            currentSpriteReplacements["Slap_Sheet_3"] = AntonChigurhSlapsSprites[3];
            currentSpriteReplacements["Slap_Sheet_4"] = AntonChigurhSlapsSprites[4];

            currentSpriteReplacements["Slap_Sheet_Broken_0"] = AntonChigurhBrokenRulerSprites[0];
            currentSpriteReplacements["Slap_Sheet_Broken_1"] = AntonChigurhBrokenRulerSprites[1];
            currentSpriteReplacements["Slap_Sheet_Broken_2"] = AntonChigurhBrokenRulerSprites[2];
            currentSpriteReplacements["Slap_Sheet_Broken_3"] = AntonChigurhBrokenRulerSprites[3];
            currentSpriteReplacements["Slap_Sheet_Broken_4"] = AntonChigurhBrokenRulerSprites[4];

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
                string spriteName = $"Baldi_Wave{i:D4}"; // D4 = 0000, 0001, ... 0099
                currentSpriteReplacements[spriteName] = AntonChigurhIdleSprite; // boiiiiii i wont replace all his spritessss
            }

            yield return "Loading items...";

            ItemObject trackerItem = new ItemBuilder(Info)
                .SetSprites(SuitcaseIcon, SuitcaseIcon)
                .SetEnum("BaldiTracker")
                .SetNameAndDescription("Suitcase", "Desc_BaldiTracker")
                .SetShopPrice(99)
                .SetGeneratorCost(99)
                .SetItemComponent<ITM_BaldiTracker>()
                .SetAsInstantUse()
                .Build();

            assetMan.Add<ItemObject>("BaldiTracker", trackerItem);

            yield return "Loading Localization...";

            string LocalizationPath = Path.Combine(AssetLoader.GetModPath(this), "Subtitles_English.json");
            AssetLoader.LocalizationFromFile(LocalizationPath, Language.English);

            yield break;
        }

        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}