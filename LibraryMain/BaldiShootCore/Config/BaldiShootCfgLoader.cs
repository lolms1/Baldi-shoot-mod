using MTM101BaldAPI.AssetTools;
using Newtonsoft.Json;
using System.Reflection;
using UnityEngine;

namespace BaldiShootCore
{
    public static class BaldiShootCfgLoader
    {
        public static void LoadAndApply()
        {
            string configPath = Path.Combine(AssetLoader.GetModPath(BasePlugin.Instance), "Config.json");

            BaldiShootCfgData data;

            if (!File.Exists(configPath))
            {
                data = new BaldiShootCfgData();
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Debug.Log("[BaldiShoot] Default config created at: " + configPath);
            }
            else
            {
                string json = File.ReadAllText(configPath);
                data = JsonConvert.DeserializeObject<BaldiShootCfgData>(json);
            }

            BaldiShootCfg.SetDefaults(data);
            BaldiShootCfg.ResetToDefaults();
        }
    }
}