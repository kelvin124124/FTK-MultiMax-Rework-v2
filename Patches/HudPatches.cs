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
            int turnIndex = __instance.m_Cow.m_FTKPlayerID.TurnIndex;
            int maxPlayers = GameFlowMC.gMaxPlayers;

            float halfSpan = 725f;
            float totalSpan = 2 * halfSpan;

            RectTransform rt = __instance.GetComponent<RectTransform>();
            float hudWidth = rt.rect.width - 220f;
            float slotWidth = totalSpan / maxPlayers;
            float scale = Mathf.Min(1f, slotWidth / hudWidth);

            float t = turnIndex / (float)(maxPlayers - 1);
            rt.anchoredPosition = new Vector2(Mathf.Lerp(-halfSpan, halfSpan, t), rt.anchoredPosition.y);
            rt.localScale = new Vector3(scale, scale, scale);
        }
    }

    [PatchType(typeof(uiHudScroller))]
    public class uiHudScrollerPatches
    {
        [PatchMethod("Init")]
        [PatchPosition(Prefix)]
        public static bool InitHUD(ref uiHudScroller __instance, uiPlayerMainHud _playerHud, ref int ___m_Index, ref Dictionary<uiPlayerMainHud, int> ___m_TargetIndex, ref List<uiPlayerMainHud> ___m_Huds, ref float ___m_HudWidth, ref float[] ___m_Positions) {
            CharacterOverworld cow = _playerHud.m_Cow;
            int posIndex = cow.m_FTKPlayerID.TurnIndex + 1;
            ___m_Index = GameLogic.Instance.IsSinglePlayer() ? 0 : cow.m_FTKPlayerID.TurnIndex;

            ___m_TargetIndex[_playerHud] = posIndex;
            ___m_Huds.Add(_playerHud);

            RectTransform rt = _playerHud.GetComponent<RectTransform>();
            ___m_HudWidth = rt.rect.width;

            // Expand positions array if needed
            if (posIndex >= ___m_Positions.Length) {
                float[] expanded = new float[posIndex + 1];
                Array.Copy(___m_Positions, expanded, ___m_Positions.Length);
                ___m_Positions = expanded;
            }

            Vector3 pos = rt.localPosition;
            pos.y = -rt.anchoredPosition.y;
            pos.x = ___m_Positions[posIndex];
            rt.localPosition = pos;
            return false;
        }
    }
}
