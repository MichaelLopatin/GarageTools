using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestText : MonoBehaviour
{
    private TextMesh textMesh;
    private string str = "";
    public static int count=0;

        private void Awake()
    {
        textMesh = this.gameObject.GetComponent<TextMesh>();
    }
    private void Update()
    {

        //str = "isChanging " + AnalysisToolsRelativePosition.isChanging.ToString() + "\n" +
        //    " isMovingDown " + AnalysisToolsRelativePosition.isMovingDown.ToString() + "\n" +
        //    " isGathering " + AnalysisToolsRelativePosition.isGathering.ToString() + "\n" +
        //    " isMatchManipulation " + AnalysisToolsRelativePosition.isMatchManipulation.ToString();
        textMesh.text = count.ToString();
    }
}
