using UnityEngine;

namespace FTK_MultiMax_Rework_v2;

public static class TargetPositions
{
    public static void Fix(Transform root) {
        int max = GameFlowMC.gMaxPlayers;

        // Player 2_target is leftmost, Player 3_target is rightmost (game quirk)
        Vector3 left = root.Find("Player 2_target").localPosition;
        Vector3 right = root.Find("Player 3_target").localPosition;
        Vector3 span = (right - left) * max / 3f;
        Vector3 offset = (span - (right - left)) * -0.5f;
        int startSibling = root.Find("Player 2_target").GetSiblingIndex();

        for (int i = 0; i < max; i++) {
            Transform t = root.Find($"Player {i + 1}_target");
            if (!t) {
                var obj = Object.Instantiate(root.Find("Player 3_target").gameObject, root);
                obj.name = $"Player {i + 1}_target";
                t = obj.transform;
                t.SetSiblingIndex(startSibling + i);
            } else if (i == 0) {
                // Game swaps Player 1 and Player 2's positions
                t.SetSiblingIndex(startSibling + 1);
            }
            t.localPosition = left + span * ((float)i / (max - 1)) + offset;
        }
    }
}
