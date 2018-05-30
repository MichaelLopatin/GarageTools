using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesBar : MonoBehaviour
{
    public delegate void WinLevel();
    public static event WinLevel WinLevelEvent;
    public delegate void LoseLevel();
    public static event LoseLevel LoseLevelEvent;

    private Transform movesBarTransform;
    [SerializeField] private GameObject curentText;
    private TextMesh curentTextMesh;
    [SerializeField] private GameObject curentMovesBar;
    private Transform curentMovesBarTransform;
    [SerializeField] private GameObject maxMovesBar;
    private Transform maxMovesBarTransform;
    [SerializeField] private GameObject maxMovesText;
    private TextMesh maxMovesTextMesh;
    

    private int curentMoves = 0;
    private int maxMoves = 10;
    private string maxMovesStr = "/";

    private Vector3 barPosition;
    private Vector3 startBarPosition;
    private Vector3 barScale;
    private float barStep;

    private void OnEnable()
    {
        AnalysisToolsRelativePosition.PlayerMoveEvent += GetMove;
        movesBarTransform = this.transform;
        curentMovesBarTransform = curentMovesBar.transform;
        maxMovesBarTransform = maxMovesBar.transform;
        curentTextMesh = curentText.GetComponent<TextMesh>();
        StartCoroutine(FirstPositionCoroutine());
        maxMovesTextMesh= maxMovesText.GetComponent<TextMesh>();
    }
    private void OnDisable()
    {
        AnalysisToolsRelativePosition.PlayerMoveEvent -= GetMove;
    }

    private void Start()
    {

    }
    private IEnumerator FirstPositionCoroutine()
    {
        yield return null;
        maxMovesStr = maxMovesStr + maxMoves.ToString();
        maxMovesTextMesh.text = maxMovesStr;
        movesBarTransform.position = Field.IndicatorsCentrePosition + new Vector3(0, -1, 0);
        maxMovesBarTransform.localPosition = Vector3.zero;
        curentMovesBarTransform.localPosition = new Vector3(0f, 0f, -0.5f);
        startBarPosition = curentMovesBarTransform.position;
        barScale = new Vector3(2f, 0.5f, 1);
        barStep = barScale.x / maxMoves;
        barScale.x = curentMoves * barStep;
        curentMovesBarTransform.localScale = barScale;
        barPosition = startBarPosition;
        curentTextMesh.text = curentMoves.ToString();
    }

    private void GetMove()
    {
        curentMoves++;
        barScale.x = curentMoves * barStep;
        barPosition.x = startBarPosition.x - 1f + curentMoves * barStep * 0.5f;
        curentMovesBarTransform.position = barPosition;
        curentMovesBarTransform.localScale = barScale;
        curentTextMesh.text = curentMoves.ToString();

        if (curentMoves == maxMoves)
        {
            if (GameIndicators.points >= GameIndicators.pointsWinByMoves)
            {
                if (WinLevelEvent != null)
                {
                    WinLevelEvent();
                }
            }
            else
            {
                if (LoseLevelEvent != null)
                {
                    LoseLevelEvent();
                }
            }
        }
    }
}
