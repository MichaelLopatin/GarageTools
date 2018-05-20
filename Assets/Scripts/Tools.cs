using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tools : MonoBehaviour
{
    private float[,,] toolsXYCoordinates;

    private float[,,] tools;

    private float curentUnitScale = 0f;
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;


    private void Awake()
    {

    }
    private void OnEnable()
    {
        Field.ChangeCellsCoordinatesEvent += SetToolsXYCoordinates;
        Field.ChangeFieldParametersEvent += SetFieldParameters;
    }
    private void OnDisable()
    {
        Field.ChangeCellsCoordinatesEvent -= SetToolsXYCoordinates;
        Field.ChangeFieldParametersEvent -= SetFieldParameters;
    }

    private void SetToolsXYCoordinates(float[,,] cellsXYCoordinates)
    {
        toolsXYCoordinates = (float[,,])cellsXYCoordinates.Clone();
    }

    private void SetFieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
        curentUnitScale = scale;
    }

}
