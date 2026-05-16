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

namespace BaldiTestMod
{
    [BepInPlugin("baldi.test.mod.setup", "Baldi test mod", "1.0.0.1")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [HarmonyPatch(typeof(Principal))]

    public class BasePlugin : BaseUnityPlugin
    {
        public static AssetManager assetMan = new AssetManager();

        public static Dictionary<string, Sprite> currentSpriteReplacements = new Dictionary<string, Sprite>();

        public void Awake()
        {
            Harmony harmony = new Harmony("baldi.test.mod.setup");

            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMyAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, GeneratorChanges);
        }

        void GeneratorChanges(string floorname, int levelId, SceneObject obj)
        {
            obj.shopItems = obj.shopItems.AddRangeToArray(new WeightedItemObject[]
                {
                    new WeightedItemObject()
                    {
                        selection = assetMan.Get<ItemObject>("TestItem"),
                        weight = 500
                    },
                });
        }
        IEnumerator LoadMyAssets()
        {
            yield return 2;

            yield return "Loading test item...";


            Sprite small = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 25f, "placeholder.png");
            Sprite big = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder.png");
            Sprite placeholder = AssetLoader.SpriteFromMod(this, Vector2.one / 5f, 50f, "placeholder2.png");
            Sprite placeholder2 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder3.png");
            Sprite placeholder3 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder4.png");
            Sprite placeholder4 = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "title.png");

            assetMan.Add<Sprite>("Test_Sprite_Small", small);
            assetMan.Add<Sprite>("placeholder", big);
            assetMan.Add<Sprite>("placeholder2", placeholder);
            assetMan.Add<Sprite>("placeholder3", placeholder2);
            assetMan.Add<Sprite>("placeholder4", placeholder3);
            assetMan.Add<Sprite>("placeholder5", placeholder4);

            ItemObject testItem = new ItemBuilder(Info)
                .SetSprites(small, big)
                .SetEnum("TestItem")
                .SetNameAndDescription("Itm_Test", "Desc_Test")
                .SetShopPrice(10)
                .SetGeneratorCost(1)
                .SetItemComponent<TestItem>()
                .Build();
            
            assetMan.Add<ItemObject>("TestItem", testItem);

            currentSpriteReplacements["Slap_Sheet_0"] = assetMan.Get<Sprite>("placeholder5");
            currentSpriteReplacements["Slap_Sheet_1"] = assetMan.Get<Sprite>("placeholder5");
            currentSpriteReplacements["Slap_Sheet_2"] = assetMan.Get<Sprite>("placeholder5");
            currentSpriteReplacements["Slap_Sheet_3"] = assetMan.Get<Sprite>("placeholder5");
            currentSpriteReplacements["Slap_Sheet_4"] = assetMan.Get<Sprite>("placeholder5");

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