using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIndicators : MonoBehaviour
{
    //public delegate void ChangeLevel(int level); // не подписаны
    //public static event ChangeLevel ChangeLevelEvent;

    public static int level=9;
    private int lastLevel= level;

    private int points = 0;

    public static Vector3 boxPointPosition = new Vector3(-3, -3, -2);

    private float levelTime;

    private int lives;

    private void Update()
    {
        //if(lastLevel!=level)
        //{
        //    if(ChangeLevelEvent!=null)
        //    {
        //        ChangeLevelEvent(level);
        //    }
        //    lastLevel = level;
        //}
    }

}
