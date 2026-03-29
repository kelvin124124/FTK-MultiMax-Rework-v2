using System.Collections.Generic;
using System.Reflection;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using Object = UnityEngine.Object;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(uiPortraitHolder))]
    public class uiPortraitHolderPatches
    {
        private static readonly FieldInfo FollowCarrierField = typeof(uiPortraitHolder).GetField("m_FollowCarrier", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo CarrierPassengersField = typeof(uiPortraitHolder).GetField("m_CarrierPassengers", BindingFlags.NonPublic | BindingFlags.Instance);

        [PatchMethod("UpdateDisplay")]
        [PatchPosition(Prefix)]
        public static bool UpdateDisplayPatch(uiPortraitHolder __instance, ref bool __result) {
            __result = true;
            if (__instance.m_PortraitActionPoints == null) return false;

            var followCarrier = (MiniHexInfo)FollowCarrierField?.GetValue(__instance);
            var passengers = (List<CharacterOverworld>)CarrierPassengersField?.GetValue(__instance);

            if (FollowCarrierField == null && __instance.m_HexLand.m_PlayersInHex.Count == 0) {
                __instance.gameObject.SetActive(false);
                Object.Destroy(__instance.gameObject);
                return false;
            }

            __instance.gameObject.SetActive(true);
            __instance.m_PortraitAndName.Hide();
            __instance.m_PortraitRoot.gameObject.SetActive(true);

            foreach (var p in __instance.m_PortraitActionPoints) p.ResetShouldShow();

            var characters = followCarrier != null
                ? (IEnumerable<CharacterOverworld>)passengers
                : __instance.m_HexLand.m_PlayersInHex;
            int idx = 0;
            foreach (var c in characters) {
                if (idx < __instance.m_PortraitActionPoints.Count)
                    __instance.m_PortraitActionPoints[idx].CalculateShouldShow(c, _alwaysShowPortrait: followCarrier != null);
                idx++;
            }

            foreach (var p in __instance.m_PortraitActionPoints) p.UpdateShow();
            return false;
        }
    }

    [PatchType(typeof(uiPortraitHolderManager))]
    public class uiPortraitHolderManagerPatches
    {
        [PatchMethod("Create")]
        [PatchPosition(Postfix)]
        [PatchParams(typeof(HexLand))]
        public static void AddMorePlayersToUI(ref uiPortraitHolder __result) {
            var last = __result.m_PortraitActionPoints[__result.m_PortraitActionPoints.Count - 1];
            while (__result.m_PortraitActionPoints.Count < GameFlowMC.gMaxPlayers)
                __result.m_PortraitActionPoints.Add(Object.Instantiate(last, last.transform.parent));
        }
    }
}
