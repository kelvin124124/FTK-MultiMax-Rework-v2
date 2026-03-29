using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(EncounterSession))]
    public class EncounterSessionPatches
    {
        [PatchMethod("GiveOutLootXPGold")]
        [PatchPosition(Prefix)]
        public static void XPModifierPatch(ref FTKPlayerID _recvPlayer, ref int _xp, ref int _gold) {
            if (GameFlowMC.gMaxPlayers <= 3) return;
            var stats = FTKHub.Instance.GetCharacterOverworldByFID(_recvPlayer).m_CharacterStats;
            _xp = Mathf.RoundToInt(_xp * stats.XpModifier * 1.5f);
            _gold = Mathf.RoundToInt(_gold * stats.GoldModifier * 1.5f);
        }
    }
}
