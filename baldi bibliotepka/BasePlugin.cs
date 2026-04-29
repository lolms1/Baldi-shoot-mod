using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.Components;
using System.Collections;
using UnityEngine;

namespace BaldiTestMod
{
    [BepInPlugin("baldi.test.mod.setup", "Baldi test mod", "1.0.0.1")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static AssetManager assetMan = new AssetManager();
        public void Awake()
        {
            Harmony harmony = new Harmony("baldi.test.mod.setup");

            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMyAssets(), LoadingEventOrder.Start);
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
                .SetShopPrice(100)
                .SetGeneratorCost(10)
                .SetItemComponent<Item>()
                .Build();
            
            assetMan.Add<ItemObject>("TestItem", testItem);

            yield break;
        }

        IEnumerator RegularLoad() { yield break; }
        IEnumerator PostLoad() { yield break; }
    }
}