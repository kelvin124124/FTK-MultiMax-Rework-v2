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
            var dummies = new List<GameObject>();

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxPlayers); i++)
                dummies.Add(i < 3
                    ? FTKHub.Instance.m_Dummies[i]
                    : CreateDummy(FTKHub.Instance.m_Dummies[2], $"Player {i + 1} Dummy", 3245 + i));

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxEnemies); i++)
                dummies.Add(i < 3
                    ? FTKHub.Instance.m_Dummies[i + 3]
                    : CreateDummy(FTKHub.Instance.m_Dummies[5], $"Enemy {i + 1} Dummy", 3045 + i));

            FTKHub.Instance.m_Dummies = dummies.ToArray();
            Log("Dummies created");
        }

        private static GameObject CreateDummy(GameObject template, string name, int viewID) {
            var copy = Object.Instantiate(template, template.transform.parent);
            copy.name = name;
            copy.GetComponent<PhotonView>().viewID = viewID;
            return copy;
        }
    }

    [PatchType(typeof(uiStartGame))]
    public class uiStartGamePatches
    {
        [PatchMethod("StartGame")]
        [PatchPosition(Prefix)]
        public static void RecreateDummies() => DummiesHandler.CreateDummies();
    }
}
