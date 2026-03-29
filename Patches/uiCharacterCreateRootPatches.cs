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
            int maxPlayers = GameFlowMC.gMaxPlayers;
            if (__instance.m_CreateUITargets.Length >= maxPlayers) return;

            Transform[] uiTargets = new Transform[maxPlayers];
            Transform[] camTargets = new Transform[maxPlayers];

            Vector3 firstCamPos = SelectScreenCamera.Instance.m_PlayerTargets[0].position;
            Vector3 lastCamPos = SelectScreenCamera.Instance.m_PlayerTargets[2].position;

            for (int i = 0; i < maxPlayers; i++) {
                if (i < __instance.m_CreateUITargets.Length) {
                    uiTargets[i] = __instance.m_CreateUITargets[i];
                    camTargets[i] = SelectScreenCamera.Instance.m_PlayerTargets[i];
                } else {
                    uiTargets[i] = Object.Instantiate(uiTargets[i - 1], uiTargets[i - 1].parent);
                    camTargets[i] = Object.Instantiate(camTargets[i - 1], camTargets[i - 1].parent);
                }
            }

            __instance.m_CreateUITargets = uiTargets;
            SelectScreenCamera.Instance.m_PlayerTargets = camTargets;

            for (int i = 0; i < maxPlayers; i++) {
                float t = i / (float)(maxPlayers - 1);
                uiTargets[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-550f, 550f, t), 129f);
                camTargets[i].position = Vector3.Lerp(firstCamPos, lastCamPos, t);
            }
        }
    }
}
