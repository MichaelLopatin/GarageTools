using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIndicators : MonoBehaviour
{
    public enum GameMode
    {
        time,
        moves
    }

    public static int level = 1; // поле генерируется до 10-го (включительно) уровня
    public static GameMode gameMode = GameMode.time;
    //private int lastLevel= level;
    //private int lives;

    [SerializeField] private GameObject timeBarObj;
    [SerializeField] private GameObject movesBarObj;
    [SerializeField] private GameObject gameScripts;
    [SerializeField] private GameObject shakeText;
    private Transform shakeTextTransform;
    [SerializeField] private GameObject pointsObj;
    [SerializeField] private GameObject winLoseText;
    private Transform winLoseTextTransform;
    private TextMesh winLoseTextMesh;
    [SerializeField] private GameObject minPointsText;
    private TextMesh minPointsTextMesh;
    [SerializeField] private GameObject pauseButtonObj;
    private PauseButton pauseButtonComponent;

    public static int points = 0;
    public static int pointsForTool = 1;
    public static int pointsWinByTime = 70;
    public static int pointsWinByMoves = 34;

    public static Vector3 pointsObjOffset = new Vector3(0, -3, 0);
    public static Vector3 boxPointPosition;

    private void Awake()
    {
        pauseButtonComponent = pauseButtonObj.GetComponent<PauseButton>();
        pauseButtonComponent.enabled = false;
        shakeTextTransform = shakeText.transform;
        shakeText.SetActive(false);
        winLoseTextMesh = winLoseText.GetComponent<TextMesh>();
        winLoseTextTransform = winLoseText.transform;
        winLoseText.SetActive(false);
        minPointsTextMesh = minPointsText.GetComponent<TextMesh>();
    }

    private void Start()
    {
        boxPointPosition = Field.IndicatorsCentrePosition + pointsObjOffset;
        pointsObj.transform.position = boxPointPosition;
    }

    private void OnEnable()
    {
        AnalysisToolsRelativePosition.ShakeUpEvent += Shake;
        if (gameMode == GameMode.moves)
        {
            minPointsTextMesh.text = "/" + pointsWinByMoves.ToString();
            MovesBar.WinLevelEvent += Win;
            MovesBar.LoseLevelEvent += Lose;
            timeBarObj.SetActive(false);
        }
        if (gameMode == GameMode.time)
        {
            minPointsTextMesh.text = "/" + pointsWinByTime.ToString();
            TimeBar.WinLevelEvent += Win;
            TimeBar.LoseLevelEvent += Lose;
            movesBarObj.SetActive(false);
        }
        MouseController.PressPauseButtonEvent += GameOnPause;
    }

    private void OnDisable()
    {
        AnalysisToolsRelativePosition.ShakeUpEvent -= Shake;
        if (gameMode == GameMode.moves)
        {
            MovesBar.WinLevelEvent -= Win;
            MovesBar.LoseLevelEvent -= Lose;
        }
        if (gameMode == GameMode.time)
        {
            TimeBar.WinLevelEvent -= Win;
            TimeBar.LoseLevelEvent -= Lose;
        }
        MouseController.PressPauseButtonEvent -= GameOnPause;
    }

    private void GameOnPause()
    {
        pauseButtonComponent.enabled = true;
    }

    private void Shake()
    {
        StartCoroutine(ShakeTextCoroutine());
    }

    private IEnumerator ShakeTextCoroutine()
    {
        shakeText.SetActive(true);
        float timeForScaling = 0.5f;
        float frequency = 1 / timeForScaling;
        float scaleTime = 0f;
        Vector3 beginScale = shakeTextTransform.localScale;
        Vector3 targetScale = beginScale;
        targetScale.y = 4f;
        while (scaleTime < timeForScaling)
        {
            scaleTime += Time.deltaTime;
            if (scaleTime > timeForScaling)
            {
                scaleTime = timeForScaling;
            }

            shakeTextTransform.localScale = Vector3.Lerp(beginScale, targetScale, scaleTime * frequency);
            yield return null;
        }
        shakeText.SetActive(false);
        shakeTextTransform.localScale = beginScale;
    }

    private void Win()
    {
        winLoseTextMesh.text = "YOU WIN :)";
        gameScripts.GetComponent<MouseController>().enabled = false;
        StartCoroutine(WinLoseTextCoroutine());
    }

    private void Lose()
    {
        winLoseTextMesh.text = "YOU LOSE :(";
        gameScripts.GetComponent<MouseController>().enabled = false;
        StartCoroutine(WinLoseTextCoroutine());
    }

    private IEnumerator WinLoseTextCoroutine()
    {
        winLoseText.SetActive(true);
        float timeForScaling = 1f;
        float frequency = 1 / timeForScaling;
        float scaleTime = 0f;
        Vector3 beginScale = winLoseTextTransform.localScale;
        Vector3 targetScale = beginScale;
        targetScale.y = 4f;
        while (scaleTime < timeForScaling)
        {
            scaleTime += Time.deltaTime;
            if (scaleTime > timeForScaling)
            {
                scaleTime = timeForScaling;
            }
            winLoseTextTransform.localScale = Vector3.Lerp(beginScale, targetScale, scaleTime * frequency);
            yield return null;
        }
        scaleTime = 0f;
        while (scaleTime < timeForScaling)
        {
            scaleTime += Time.deltaTime;
            if (scaleTime > timeForScaling)
            {
                scaleTime = timeForScaling;
            }

            winLoseTextTransform.localScale = Vector3.Lerp(targetScale, beginScale, scaleTime * frequency);
            yield return null;
        }
        winLoseTextTransform.localScale = beginScale;
        ResetProgress();
        SceneManager.LoadScene("StartScene");
    }

    private static void ResetProgress()
    {
        points = 0;
    }
}
