using System;
using System.Collections.Generic;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(uiPlayerMainHud))]
    public class uiPlayerMainHudPatches
    {
        [PatchMethod("Update")]
        [PatchPosition(Prefix)]
        public static void PlaceUI(ref uiPlayerMainHud __instance) {
            var rt = __instance.GetComponent<RectTransform>();
            float t = __instance.m_Cow.m_FTKPlayerID.TurnIndex / (float)(GameFlowMC.gMaxPlayers - 1);
            float scale = Mathf.Min(1f, 1450f / GameFlowMC.gMaxPlayers / (rt.rect.width - 220f));
            rt.anchoredPosition = new Vector2(Mathf.Lerp(-725f, 725f, t), rt.anchoredPosition.y);
            rt.localScale = Vector3.one * scale;
        }
    }

    [PatchType(typeof(uiHudScroller))]
    public class uiHudScrollerPatches
    {
        [PatchMethod("Init")]
        [PatchPosition(Prefix)]
        public static bool InitHUD(ref uiHudScroller __instance, uiPlayerMainHud _playerHud, ref int ___m_Index, ref Dictionary<uiPlayerMainHud, int> ___m_TargetIndex, ref List<uiPlayerMainHud> ___m_Huds, ref float ___m_HudWidth, ref float[] ___m_Positions) {
            int posIndex = _playerHud.m_Cow.m_FTKPlayerID.TurnIndex + 1;
            ___m_Index = GameLogic.Instance.IsSinglePlayer() ? 0 : posIndex - 1;
            ___m_TargetIndex[_playerHud] = posIndex;
            ___m_Huds.Add(_playerHud);

            var rt = _playerHud.GetComponent<RectTransform>();
            ___m_HudWidth = rt.rect.width;

            if (posIndex >= ___m_Positions.Length) {
                var expanded = new float[posIndex + 1];
                Array.Copy(___m_Positions, expanded, ___m_Positions.Length);
                ___m_Positions = expanded;
            }

            rt.localPosition = new Vector3(___m_Positions[posIndex], -rt.anchoredPosition.y, rt.localPosition.z);
            return false;
        }
    }
}
