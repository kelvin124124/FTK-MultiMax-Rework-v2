using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(uiCharacterCreateRoot))]
    public class uiCharacterCreateRootPatches
    {
        [PatchMethod("Start")]
        [PatchPosition(Prefix)]
        public static void AddMorePlayerSlotsInMenu(ref uiCharacterCreateRoot __instance) {
            int max = GameFlowMC.gMaxPlayers;
            if (__instance.m_CreateUITargets.Length >= max) return;

            var cam = SelectScreenCamera.Instance;
            Vector3 firstPos = cam.m_PlayerTargets[0].position, lastPos = cam.m_PlayerTargets[2].position;

            var ui = new Transform[max];
            var camT = new Transform[max];
            for (int i = 0; i < max; i++) {
                ui[i] = i < __instance.m_CreateUITargets.Length ? __instance.m_CreateUITargets[i] : Object.Instantiate(ui[i - 1], ui[i - 1].parent);
                camT[i] = i < cam.m_PlayerTargets.Length ? cam.m_PlayerTargets[i] : Object.Instantiate(camT[i - 1], camT[i - 1].parent);
                float t = i / (float)(max - 1);
                ui[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-550f, 550f, t), 129f);
                camT[i].position = Vector3.Lerp(firstPos, lastPos, t);
            }
            __instance.m_CreateUITargets = ui;
            cam.m_PlayerTargets = camT;
        }
    }
}
