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
            yield return 1;

            yield return "Loading test item...";


            Sprite small = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 25f, "placeholder.png");
            Sprite big = AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "placeholder.png");
            
            assetMan.Add<Sprite>("Test_Sprite_Small", small);
            assetMan.Add<Sprite>("Test_Sprite_Big", big);

            
            ItemObject testItem = new ItemBuilder(Info)
                .SetSprites(small, big)
                .SetEnum("TestItem")
                .SetNameAndDescription("Itm_Test", "Desc_Test")
                .SetShopPrice(10)
                .SetGeneratorCost(1)
                .SetItemComponent<TestItem>()
                .Build();
            
            assetMan.Add<ItemObject>("TestItem", testItem);

            yield break;
        }

        
        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}