using System.Collections.Generic;
using UnityEngine;

public class TransCreater : MonoBehaviour
{
    [Header("Single Card")]
    [SerializeField] Vector2 singleCardOffset;
    [SerializeField] float xSingleGrid;

    [Header("Double Card")]
    [SerializeField] Vector2 doubleCardOffset;
    [SerializeField] float xDoubleGrid;
    [SerializeField] float yGrid;

    public List<Vector2> GetSingleCardTrans() {
        List<Vector2> trans = new List<Vector2>();

        for (int i = 0; i < GameSettings.SingleCardCount; i++) {
            Vector2 pos = new Vector2(singleCardOffset.x + i * xSingleGrid, singleCardOffset.y);
            trans.Add(pos);
        }

        return trans;
    }

    public List<Vector2> GetDoubleCardTrans() {
        List<Vector2> trans = new List<Vector2>();

        for (int i = 0; i < GameSettings.MaxBottomCount; i++) {
            float yPos = -i * yGrid;
            float start = -i * xDoubleGrid / 2;
            for (int j = 0; j <= i; j++) {
                Vector2 pos = new Vector2(start + j * xDoubleGrid + doubleCardOffset.x, yPos + doubleCardOffset.y);
                trans.Add(pos);
            }
        }

        return trans;
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < GameSettings.MaxBottomCount; i++) {
            float yPos = -i * yGrid;
            float start = -i * xDoubleGrid / 2;
            for (int j = 0; j <= i; j++) {
                Vector2 pos = new Vector2(start + j * xDoubleGrid + doubleCardOffset.x, yPos + doubleCardOffset.y);
                Gizmos.DrawSphere(pos, 0.05f);
            }
        }

        for (int i = 0; i < GameSettings.SingleCardCount; i++) {
            Vector2 pos = new Vector2(singleCardOffset.x + i * xSingleGrid, singleCardOffset.y);
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }
}
