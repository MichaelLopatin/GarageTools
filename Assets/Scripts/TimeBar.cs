using System.Collections;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
    public delegate void WinLevel();
    public static event WinLevel WinLevelEvent;
    public delegate void LoseLevel();
    public static event LoseLevel LoseLevelEvent;

    private Transform timeBarTransform;
    [SerializeField] private GameObject curentText;
    private TextMesh curentTextMesh;
    [SerializeField] private GameObject curentTimeBar;
    private Transform curentTimeBarTransform;
    [SerializeField] private GameObject maxTimeBar;
    private Transform maxTimeBarTransform;
    [SerializeField] private GameObject maxTimeText;
    private TextMesh maxTimeTextMesh;

    private int curentSeconds = 0;
    private int maxSeconds = 60;

    private Vector3 barPosition;
    private Vector3 startBarPosition;
    private Vector3 barScale;
    private float barStep;

    private void OnEnable()
    {
        timeBarTransform = this.transform;
        curentTimeBarTransform = curentTimeBar.transform;
        maxTimeBarTransform = maxTimeBar.transform;
        curentTextMesh = curentText.GetComponent<TextMesh>();
        maxTimeTextMesh = maxTimeText.GetComponent<TextMesh>();
        StartCoroutine(FirstPositionCoroutine());
        StartCoroutine(TimeProcessingCoroutine());
    }

    private IEnumerator FirstPositionCoroutine()
    {
        yield return null;
        maxTimeTextMesh.text = "/" + maxSeconds.ToString();
        timeBarTransform.position = Field.IndicatorsCentrePosition + new Vector3(0, -1, 0);
        maxTimeBarTransform.localPosition = Vector3.zero;
        curentTimeBarTransform.localPosition = new Vector3(0f, 0f, -0.5f);
        startBarPosition = curentTimeBarTransform.position;
        barScale = new Vector3(2f, 0.5f, 1);
        barStep = barScale.x / maxSeconds;
        barScale.x = curentSeconds * barStep;
        curentTimeBarTransform.localScale = barScale;
        barPosition = startBarPosition;
        curentTextMesh.text = curentSeconds.ToString();
    }

    private IEnumerator TimeProcessingCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (curentSeconds != maxSeconds)
        {
            curentSeconds++;
            barScale.x = curentSeconds * barStep;
            barPosition.x = startBarPosition.x - 1f + curentSeconds * barStep * 0.5f;
            curentTimeBarTransform.position = barPosition;
            curentTimeBarTransform.localScale = barScale;
            curentTextMesh.text = curentSeconds.ToString();
            yield return new WaitForSeconds(1f);
        }
        if (curentSeconds == maxSeconds)
        {
            if (GameIndicators.points >= GameIndicators.pointsWinByTime)
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
