using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;


public class Tools : MonoBehaviour
{
    private enum Cell
    {
        id = 0,
        toolsType,
        isDelete
    }
    private float[,,] toolsXYCoordinates;

    private float[,,] toolsInCellsInfo;

    private int[,] gameField;

    private float curentUnitScale = 0f;
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;

    private void Awake()
    {

    }

    private void Start()
    {
        FillGameField(out gameField, curentFieldWidth, curentFieldWidth);
        PrintField(gameField, curentFieldWidth, curentFieldHeight);
        SetToolsOnFieldInStart(gameField, ref ToolsPool.toolsReservedListOfStacks, ref ToolsPool.toolsOnFieldListOfStacks, curentFieldWidth, curentFieldHeight, curentUnitScale);

    }

    private void OnEnable()
    {
        Field.TestDelegEvent += TestPr;
        //     Field.ChangeFieldParametersEvent += SetFieldParameters;
        Field.CoordinatesEvent += SetXYCoordinates;
        Field.FieldParametrsEvent += FieldParameters;
    }
    private void OnDisable()
    {
        Field.TestDelegEvent -= TestPr;

    }

    private void SetToolsFieldInfo()
    {

    }
    private void SetXYCoordinates(float[,,] cellXYCoordinates)
    {
        print("SetXYCoordinates сработало");
        toolsXYCoordinates = (float[,,])cellXYCoordinates.Clone();
    }

    //private void SetToolsXYCoordinates(float[,,] cellsXYCoordinates)
    //{
    //    toolsXYCoordinates = (float[,,])cellsXYCoordinates.Clone();
    //}

    private void FieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
        curentUnitScale = scale;
    }

    //private void SetFieldParameters(int width, int height, float scale)
    //{
    //    curentFieldWidth = width;
    //    curentFieldHeight = height;
    //    curentUnitScale = scale;
    //}


    private void FillGameField(out int[,] gameField, int width, int height)
    {
        int[] toolTypesNumber = new int[(int)((int)ToolType.toolBox * 0.5f)];
        int typesSingleToolsQuantity = 0;
        for (int i = 0, j = 0; i < (int)ToolType.toolBox; i += 2, j++)
        {
            toolTypesNumber[j] = i;
            typesSingleToolsQuantity++;
        }
        gameField = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gameField[i, j] = toolTypesNumber[Random.Range(0, typesSingleToolsQuantity)];
            }
        }
        PrintField(gameField, curentFieldWidth, curentFieldHeight);
        CheckAndRemoveMatchesInStart(ref gameField, toolTypesNumber, typesSingleToolsQuantity, width, height);
    }

    private void CheckAndRemoveMatchesInStart(ref int[,] gameField, int[] toolTypesNumber, int typesSingleToolsQuantity, int width, int height)
    {
        int curentCellType = -1;
        int lastCellType = -1;
        int match = 0;

        for (int i = 0; i < width; i++)
        {
            match = 0;
            for (int j = 0; j < height; j++)
            {
                curentCellType = gameField[i, j];
                if (lastCellType == curentCellType)
                {
                    match++;
                }
                else
                {
                    match = 0;
                }
                if (match == 2)
                {
                    RemoveMatchesInStart(ref gameField, i, j, typesSingleToolsQuantity);
                    curentCellType = gameField[j, i];
                    match = 0;
                }
                lastCellType = curentCellType;
            }
        }
        curentCellType = -1;
        lastCellType = -1;
       
        for (int i = 0; i < width; i++)
        {
            match = 0;
            for (int j = 0; j < height; j++)
            {
                curentCellType = gameField[j, i];
                if (lastCellType == curentCellType)
                {
                    match++;
                }
                else
                {
                    match = 0;
                }
                if (match == 2)
                {
                    RemoveMatchesInStart(ref gameField, j, i, typesSingleToolsQuantity);
                    curentCellType = gameField[j, i];
                    match = 0;
                }
                lastCellType = curentCellType;
            }
        }
    }


    private void RemoveMatchesInStart(ref int[,] gameField, int column, int row, int typesSingleToolsQuantity)
    {
        //   1
        //0  C  2
        //   3
        int[] neighbours = new int[4];
        int minTypeNumber = 0;
        int maxTypeNumber = (typesSingleToolsQuantity-1)*2;

        if (column == 0)
        {
            neighbours[0] = -1;
        }
        else
        {
            neighbours[0] = gameField[column - 1, row];
        }

        if (column == curentFieldWidth - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = gameField[column + 1, row];
        }

        if (row == 0)
        {
            neighbours[1] = -1;
        }
        else
        {
            neighbours[1] = gameField[column, row - 1];
        }

        if (row == curentFieldHeight - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = gameField[column, row + 1];
        }
        print("i= " + column + " j= " + row);
        print(" n0= " + neighbours[0] + " n1= " + neighbours[1] + " n2= " + neighbours[2] + " n3= " + neighbours[3]);
        SimpleSort(ref neighbours);
        print(" n0= " + neighbours[0] + " n1= " + neighbours[1] + " n2= " + neighbours[2] + " n3= " + neighbours[3]);
        for (int i = 0; i < 4; i++)
        {
            if (minTypeNumber == neighbours[i])
            {
                minTypeNumber += 2;
            }
            if (maxTypeNumber == neighbours[3 - i])
            {
                maxTypeNumber -= 2;
            }
        }
        print("min= " + minTypeNumber + " max= " + maxTypeNumber);
        if (Random.Range(0, 2) == 0)
        {
            gameField[column, row] = minTypeNumber;
        }
        else
        {
            gameField[column, row] = maxTypeNumber;
        }
    }

    private void PrintField(int[,] field, int width, int height)
    {
        string s = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                s = s + gameField[i, j].ToString() + " ";
            }

            s = s + "\n";
        }
        print(s);
    }

    private void SimpleSort(ref int[] arr)
    {
        int temp;
        for(int i= arr.Length - 1; i>=0;i--)
        {
            for (int j = 0; j < i; j++)
            {
                if (arr[j] > arr[j+1])
                {
                    temp = arr[j+1];
                    arr[j+1] = arr[j];
                    arr[j] = temp;
                }
            }
        }

    }

    private void PrintArr(int[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
                s = s + arr[i].ToString() + " ";
        }
        print(s);
    }
    private void TestPr()
    {
        print("Тестовое соытие сработало");
    }

    private void SetToolsOnFieldInStart(int[,] gameField, ref List<Stack<GameObject>> toolsReservedListOfStacks, ref List<Stack<GameObject>> toolsOnFieldListOfStacks, int width, int height, float curentUnitScale)
    {
        GameObject tool;
        Vector3 curentToolPosition = Vector3.zero;
        Vector3 unitScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);
        //    print("SetToolsOnFieldInStart запуск");
        //     print("width " + width + " height " + height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tool = toolsReservedListOfStacks[gameField[i, j]].Pop();
                //        print("pop " + tool);
              //  tool.GetComponent<ToolInfo>().CellID = gameField[i, j, (int)Cell.id];
                curentToolPosition.x = toolsXYCoordinates[i, j, 1];// заменить toolsXYCoordinates
                curentToolPosition.y = toolsXYCoordinates[i, j, 2];
                tool.transform.position = curentToolPosition;
                //       print("x= " + curentToolPosition.x+ "y= " + curentToolPosition.y);
                tool.transform.localScale = unitScale;
                tool.SetActive(true);
                //       print("tool set activ");
                //           toolsOnFieldListOfStacks[gameField[i, j, (int)Cell.toolsType]].Push(tool);
            }
        }
        //    print("SetToolsOnFieldInStart конец");
    }

}
