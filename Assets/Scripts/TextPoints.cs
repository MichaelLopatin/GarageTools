using UnityEngine;

public class TextPoints : MonoBehaviour
{
    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
    }

    private void OnEnable()
    {
        textMesh.text = 0.ToString();
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
