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
        [PatchMethod("UpdateDisplay")]
        [PatchPosition(Prefix)]
        public static bool UpdateDisplayPatch(uiPortraitHolder __instance, ref bool __result) {
            if (__instance.m_PortraitActionPoints == null) {
                __result = true;
                return false;
            }

            FieldInfo followCarrierField = typeof(uiPortraitHolder).GetField("m_FollowCarrier", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo carrierPassengersField = typeof(uiPortraitHolder).GetField("m_CarrierPassengers", BindingFlags.NonPublic | BindingFlags.Instance);

            MiniHexInfo followCarrier = (MiniHexInfo)followCarrierField?.GetValue(__instance);
            List<CharacterOverworld> carrierPassengers = (List<CharacterOverworld>)carrierPassengersField?.GetValue(__instance);

            if (followCarrierField == null && __instance.m_HexLand.m_PlayersInHex.Count == 0) {
                __instance.gameObject.SetActive(false);
                Object.Destroy(__instance.gameObject);
                __result = true;
                return false;
            }

            __instance.gameObject.SetActive(true);
            __instance.m_PortraitAndName.Hide();
            __instance.m_PortraitRoot.gameObject.SetActive(true);

            foreach (uiPortraitActionPoint point in __instance.m_PortraitActionPoints)
                point.ResetShouldShow();

            if (followCarrier != null) {
                int idx = 0;
                foreach (CharacterOverworld passenger in carrierPassengers) {
                    if (idx < __instance.m_PortraitActionPoints.Count)
                        __instance.m_PortraitActionPoints[idx].CalculateShouldShow(passenger, _alwaysShowPortrait: true);
                    idx++;
                }
            } else {
                int idx = 0;
                foreach (CharacterOverworld player in __instance.m_HexLand.m_PlayersInHex) {
                    if (idx < __instance.m_PortraitActionPoints.Count)
                        __instance.m_PortraitActionPoints[idx].CalculateShouldShow(player);
                    idx++;
                }
            }

            foreach (uiPortraitActionPoint point in __instance.m_PortraitActionPoints)
                point.UpdateShow();

            __result = true;
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
            int currentCount = __result.m_PortraitActionPoints.Count;
            var template = __result.m_PortraitActionPoints[currentCount - 1];

            for (int i = currentCount; i < GameFlowMC.gMaxPlayers; i++) {
                var point = Object.Instantiate(template, template.transform.parent);
                __result.m_PortraitActionPoints.Add(point);
            }
        }
    }
}
