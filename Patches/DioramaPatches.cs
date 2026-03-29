using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using static FTK_MultiMax_Rework_v2.Main;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(Diorama))]
    public class DioramaPatches
    {
        [PatchMethod("_resetTargetQueue")]
        [PatchPosition(Prefix)]
        public static void DummySlide() {
            foreach (var slide in Object.FindObjectsOfType<DummyAttackSlide>()) {
                if (slide.m_Distances.Length >= 1000) continue;
                // TODO: 1000 is excessive (default is 3) — find proper upper bound
                var expanded = new float[1000];
                Array.Copy(slide.m_Distances, expanded, slide.m_Distances.Length);
                slide.m_Distances = expanded;
            }
        }

        [PatchMethod("SetupTargets")]
        [PatchPosition(Postfix)]
        public static void SortTargets(ref List<Transform> _targetList) {
            for (int pass = 0; pass < _targetList.Count; pass++)
                for (int i = 0; i < _targetList.Count - 1; i++)
                    if (_targetList[i].name.Contains("Player ") && _targetList[i + 1].name.Contains("Player ")
                        && int.Parse(Regex.Match(_targetList[i].name, "\\d+").Value) > int.Parse(Regex.Match(_targetList[i + 1].name, "\\d+").Value)) {
                        var tmp = _targetList[i];
                        _targetList[i] = _targetList[i + 1];
                        _targetList[i + 1] = tmp;
                    }
        }
    }

    [PatchType(typeof(SceneDiorama))]
    public class SceneDioramaPatches
    {
        [PatchMethod("Awake")]
        [PatchPosition(Postfix)]
        public static void FixDummyPositions(SceneDiorama __instance) {
            foreach (var diorama in __instance.GetComponentsInChildren<Diorama>()) {
                if (!diorama) continue;
                Log($"Fixing dummy positions for {diorama.name}");
                foreach (var layout in diorama.m_LayoutTable.Values)
                    TargetPositions.Fix(layout.m_TargetRoot);
            }
        }
    }
}
