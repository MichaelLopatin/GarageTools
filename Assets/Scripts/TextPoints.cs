using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPoints : MonoBehaviour
{
    private TextMesh textMesh;
    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        textMesh.text = 0.ToString();
    }

    private void OnEnable()
    {
        Tool.ChangePointsEvent += PointsOnScreen;
    }
    private void OnDisable()
    {
        Tool.ChangePointsEvent -= PointsOnScreen;
    }

    private void PointsOnScreen()
    {
        textMesh.text = GameIndicators.points.ToString();
    }
}
