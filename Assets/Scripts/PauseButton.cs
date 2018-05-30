using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject gameScripts;
    private PauseButton pauseButtonComponent;

    [SerializeField] private GameObject audioObj;
    private AudioSource audioSource;
    private AudioClip audioClip;

    private bool soundOn = true;

    private void Awake()
    {
        pauseButtonComponent = this.GetComponent<PauseButton>();
        audioSource = audioObj.GetComponent<AudioSource>();
        audioClip = audioSource.clip;
    }

    private void OnEnable()
    {
        print("скрипт включился");
        gameScripts.GetComponent<MouseController>().enabled = false;
       Time.timeScale = 0;
    }
    private void OnDisable()
    {
      //  print("скрипт вЫключился");
      //  gameScripts.GetComponent<MouseController>().enabled = true;
      //Time.timeScale = 1;
    }


    private void OnGUI()
    {
        GamePauseGUI();
    }

  private  void GamePauseGUI()
    {
        GUIStyle stylePauseButton = GUI.skin.GetStyle("Button");
        GUIStyle stylePauseBox = GUI.skin.GetStyle("Box");
        stylePauseButton.fontSize = 30;
        stylePauseBox.fontSize = 30;
        stylePauseButton.normal.textColor = Color.HSVToRGB(0f, 0.98f, 0.51f);
        GUI.Box(new Rect(Screen.width * 0.5f - 140f, Screen.height * 0.5f - 180f, 280, 180),
            "GAME PAUSED", stylePauseBox);
        if (GUI.Button(new Rect(Screen.width * 0.5f - 120f, Screen.height * 0.5f - 120f, 240, 50),
        "ON/OFF SOUND", stylePauseButton))
        {
            if(soundOn)
            {
                soundOn = false;
                audioSource.Pause();
            }
            else
            {
                soundOn = true;
            //    audioSource.Play();
               audioSource.UnPause();
            }

        }
        if (GUI.Button(new Rect(Screen.width * 0.5f - 100f, Screen.height * 0.5f - 60f, 200, 50),
        "RESUME", stylePauseButton))
        {
            Time.timeScale = 1f;
            gameScripts.GetComponent<MouseController>().enabled = true;
            pauseButtonComponent.enabled = false;
        }
    }
}
