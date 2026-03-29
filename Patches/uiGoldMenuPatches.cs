using System.Collections.Generic;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using HarmonyLib;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(uiGoldMenu))]
    public class uiGoldMenuPatches
    {
        [PatchMethod("Awake")]
        [PatchPosition(Prefix)]
        public static bool GoldAwake(uiGoldMenu __instance) {
            __instance.m_InputFocus = __instance.gameObject.GetComponent<FTKInputFocus>();
            __instance.m_InputFocus.m_InputMode = FTKInput.InputMode.InGameUI;
            __instance.m_InputFocus.m_Cancel = __instance.OnButtonCancel;

            var entries = Traverse.Create(__instance).Field("m_GoldEntries").GetValue<List<uiGoldMenuEntry>>();
            if (entries == null) return false;

            entries.Add(__instance.m_FirstEntry);
            for (int i = 0; i < GameFlowMC.gMaxPlayers - 2; i++)
                entries.Add(Object.Instantiate(__instance.m_FirstEntry, __instance.m_FirstEntry.transform.parent, false));
            return false;
        }
    }
}
