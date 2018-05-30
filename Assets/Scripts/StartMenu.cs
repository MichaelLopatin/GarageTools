using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SelectTimeGame()
    {
        GameIndicators.gameMode = GameIndicators.GameMode.time;
        SceneManager.LoadScene("Levels");
    }

    public void SelectMovesGame()
    {
        GameIndicators.gameMode = GameIndicators.GameMode.moves;
        SceneManager.LoadScene("Levels");
    }
}
