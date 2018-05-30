using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private void Awake()
    {

    }

    private void OnEnable()
    {
        print("скрипт включился");
        //Time.timeScale = 0;
    }
    private void OnDisable()
    {
        print("скрипт вЫключился");
        //Time.timeScale = 1;
    }


    private void OnGUI()
    {
        GamePauseGUI();
    }

  private  void GamePauseGUI()
    {
        GUI.Box(new Rect(Screen.width * 0.5f - 120f, Screen.height * 0.5f - 180f, 240, 180),
                "ПАУЗА");
        if (GUI.Button(new Rect(Screen.width * 0.5f - 110f, Screen.height * 0.5f - 120f, 220, 50),
        "ПРОДОЛЖИТЬ"))
        {
            Time.timeScale = 1f;
        }
        if (GUI.Button(new Rect(Screen.width * 0.5f - 110f, Screen.height * 0.5f - 60f, 220, 50),
        "ВЫХОД В МЕНЮ"))
        {
            Time.timeScale = 1f;
        }
    }
}
