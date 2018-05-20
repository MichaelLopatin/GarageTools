using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tools : MonoBehaviour
{
    private enum Cell
    {
        id=0,
        toolsType,
        isDelete
   }
    private float[,,] toolsXYCoordinates;

    private float[,,] toolsInCellsInfo;

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

    private void SetToolsFieldInfo()
    {

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
