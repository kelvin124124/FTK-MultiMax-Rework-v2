using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FTK_MultiMax_Rework_v2.PatchHelpers;
using UnityEngine;
using static FTK_MultiMax_Rework_v2.Main;
using static FTK_MultiMax_Rework_v2.PatchHelpers.PatchPositions;
using Object = UnityEngine.Object;

namespace FTK_MultiMax_Rework_v2.Patches
{
    [PatchType(typeof(Diorama))]
    public class DioramaPatches
    {
        [PatchMethod("_resetTargetQueue")]
        [PatchPosition(Prefix)]
        public static void DummySlide() {
            foreach (DummyAttackSlide slide in Object.FindObjectsOfType<DummyAttackSlide>()) {
                if (slide.m_Distances.Length >= 1000) continue;

                // TODO: 1000 is excessive (default is 3) — find proper upper bound
                float[] newDistances = new float[1000];
                Array.Copy(slide.m_Distances, newDistances, slide.m_Distances.Length);
                slide.m_Distances = newDistances;
            }
        }

        [PatchMethod("SetupTargets")]
        [PatchPosition(Postfix)]
        public static void SortTargets(ref List<Transform> _targetList) {
            // Bubble sort player targets into ascending order (1-2-3-4-...)
            for (int pass = 0; pass < _targetList.Count; pass++) {
                for (int i = 0; i < _targetList.Count - 1; i++) {
                    Transform a = _targetList[i], b = _targetList[i + 1];
                    if (!a.name.Contains("Player ") || !b.name.Contains("Player "))
                        continue;

                    int idxA = int.Parse(Regex.Match(a.name, "\\d+").Value);
                    int idxB = int.Parse(Regex.Match(b.name, "\\d+").Value);

                    if (idxA > idxB) {
                        _targetList[i] = b;
                        _targetList[i + 1] = a;
                    }
                }
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
