using BepInEx;
using BepInEx.Configuration;
using System.IO;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2 {
    public static class ConfigHandler {
        public static ConfigEntry<int> MaxPlayersConfig { get; private set; }

        public static void InitializeConfig() {
            var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "MultiMaxRework.cfg"), true);
            MaxPlayersConfig = configFile.Bind("General", "MaxPlayers", 5, "The max number of players");
        }

        public static void InitializeMaxPlayers() {
            if (MaxPlayersConfig == null) {
                Debug.LogError("maxPlayersConfig is not initialized!");
                return;
            }
            GameFlowMC.gMaxPlayers = MaxPlayersConfig.Value;
            GameFlowMC.gMaxEnemies = MaxPlayersConfig.Value;
            uiQuickPlayerCreate.Default_Classes = new int[MaxPlayersConfig.Value];
        }
    }
}
