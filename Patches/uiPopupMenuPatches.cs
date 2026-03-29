using System.Linq;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using static uiPopupMenu;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(uiPopupMenu))]
    public class uiPopupMenuPatches
    {
        [PatchMethod("Awake")]
        [PatchPosition(Prefix)]
        public static void PopupAwake(uiPopupMenu __instance) {
            if (!__instance || __instance.m_Popups == null) return;
            var give = __instance.m_Popups.FirstOrDefault(p => p.m_Action == Action.Give);
            if (give != null) give.m_Count = GameFlowMC.gMaxPlayers - 1;
        }
    }
}
