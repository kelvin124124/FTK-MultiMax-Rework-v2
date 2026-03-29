using System.Collections.Generic;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.Main;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using UnityEngine;

namespace FTK_MultiMax_Rework_v2
{
    public static class DummiesHandler
    {
        public static void CreateDummies() {
            Log("Making Dummies");
            List<GameObject> dummies = new List<GameObject>();

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxPlayers); i++) {
                if (i < 3) {
                    dummies.Add(FTKHub.Instance.m_Dummies[i]);
                } else {
                    GameObject copy = Object.Instantiate(FTKHub.Instance.m_Dummies[2], FTKHub.Instance.m_Dummies[2].transform.parent);
                    copy.name = "Player " + (i + 1) + " Dummy";
                    copy.GetComponent<PhotonView>().viewID = 3245 + i;
                    dummies.Add(copy);
                }
            }

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxEnemies); i++) {
                if (i < 3) {
                    dummies.Add(FTKHub.Instance.m_Dummies[i + 3]);
                } else {
                    GameObject copy = Object.Instantiate(FTKHub.Instance.m_Dummies[5], FTKHub.Instance.m_Dummies[5].transform.parent);
                    copy.name = "Enemy " + (i + 1) + " Dummy";
                    copy.GetComponent<PhotonView>().viewID = 3045 + i;
                    dummies.Add(copy);
                }
            }

            FTKHub.Instance.m_Dummies = dummies.ToArray();
            Log("Dummies created");
        }
    }

    // Triggers dummy recreation when a game starts
    [PatchType(typeof(uiStartGame))]
    public class uiStartGamePatches
    {
        [PatchMethod("StartGame")]
        [PatchPosition(Prefix)]
        public static void RecreateDummies() {
            DummiesHandler.CreateDummies();
        }
    }
}
