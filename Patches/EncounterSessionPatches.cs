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

            CharacterOverworld cow = FTKHub.Instance.GetCharacterOverworldByFID(_recvPlayer);
            float xpMod = cow.m_CharacterStats.XpModifier;
            float goldMod = cow.m_CharacterStats.GoldModifier;

            _xp = Mathf.RoundToInt(_xp * xpMod * 1.5f);
            _gold = Mathf.RoundToInt(_gold * goldMod * 1.5f);
        }
    }
}
