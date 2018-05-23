using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIndicators : MonoBehaviour
{
    public delegate void ChangeLevel(int level); // не подписаны
    public static event ChangeLevel ChangeLevelEvent;

    [SerializeField]private int level=1;
    private int lastLevel=1;

    private int points = 0;

    private float levelTime;

    private int lives;

    private void Update()
    {
        if(lastLevel!=level)
        {
            if(ChangeLevelEvent!=null)
            {
                ChangeLevelEvent(level);
            }
            lastLevel = level;
        }
    }

}
