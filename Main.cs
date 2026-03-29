using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchUtils;
using Debug = UnityEngine.Debug;

namespace FTK_MultiMax_Rework_v2 {
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin {
        private const string pluginGuid = "polarsbear.ftk.multimaxrework.patched";
        private const string pluginName = "MultiMaxReworkV2";
        private const string pluginVersion = "2.0";

        public static Harmony Harmony { get; } = new(pluginGuid);

        public static void Log(Object message) => Debug.Log($"[{pluginName}]: {message}");

        public IEnumerator Start() {
            Log("Initializing...");
            ConfigHandler.InitializeConfig();
            ConfigHandler.InitializeMaxPlayers();

            Log("Patching...");
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypesWithAttribute<PatchType>())
                PatchClass(type);

            typeof(uiQuickPlayerCreate)
                .GetField("guiQuickPlayerCreates", BindingFlags.Static | BindingFlags.NonPublic)
                ?.SetValue(null, new uiQuickPlayerCreate[GameFlowMC.gMaxPlayers]);
            uiQuickPlayerCreate.Default_Classes = new int[GameFlowMC.gMaxPlayers];

            Log("Waiting for game to load...");
            while (!FTKHub.Instance) yield return null;
            Log("Startup done");
        }
    }
}
