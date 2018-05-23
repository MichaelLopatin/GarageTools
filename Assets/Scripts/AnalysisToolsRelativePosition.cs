using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisToolsRelativePosition : MonoBehaviour
{
    private enum Cell
    {
        id = 0,
        toolsType,
        isMatch,
        isTip,
    }
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;

    private int[,,] toolsOnField;
    private int[,,] cellsAndToolsInfo;

    //   int count = 0;
    private int toolToChange = 0;

    // задать cellsAndToolsInfo

    private void Awake()
    {

    }
    private void OnEnable()
    {
        Tools.ChangeGameFieldEvent += SetToolsOnFieldArray;
        Field.ChangeFieldParametersEvent += SetFieldParameters;
    }
    private void OnDisable()
    {
        Tools.ChangeGameFieldEvent -= SetToolsOnFieldArray;
        Field.ChangeFieldParametersEvent -= SetFieldParameters;
    }

    private void Start()
    {
        InitializecellsAndToolsInfo(out cellsAndToolsInfo);
        FindTipsInField();
 
        //PrintField(toolsOnField, curentFieldWidth, curentFieldHeight, (int)Cell.toolsType);
        //PrintField(cellsAndToolsInfo, curentFieldWidth, curentFieldHeight, (int)Cell.toolsType);
        //PrintField(cellsAndToolsInfo, curentFieldWidth, curentFieldHeight, (int)Cell.isTip);
    }

    private void Update()
    {

    }

    private void InitializecellsAndToolsInfo(out int[,,] cellsAndToolsInfo)
    {
        cellsAndToolsInfo = new int[curentFieldWidth, curentFieldHeight, (int)Cell.isTip + 1];
        for (int i=0,k=0;i< curentFieldWidth;i++)
        {
            for (int j = 0; j < curentFieldHeight; j++,k++)
            {
                cellsAndToolsInfo[i, j, (int)Cell.id] = k;
                cellsAndToolsInfo[i, j, (int)Cell.toolsType] = toolsOnField[i, j, (int)Cell.toolsType];// нужен или нет 
                cellsAndToolsInfo[i, j, (int)Cell.isMatch] = -1;
                cellsAndToolsInfo[i, j, (int)Cell.isTip] = 0;
            }
        }
    }

    private void SetToolsOnFieldArray(int[,,] array)
    {
        toolsOnField = (int[,,])array.Clone();
    }

    private void SetFieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
    }


    private void ChangeToolsOnField(int id, int column, int row, int toolsType) // для подписки на событие изменения ячейки
    {
        toolsOnField[column, row, (int)Cell.toolsType] = toolsType;
    }

    private void CheckMatches(int column, int row)
    {
        int targetToolType = toolsOnField[column, row, (int)Cell.toolsType];

        for (int i = column; i >= 0; i--)
        {
            if (toolsOnField[i, row, (int)Cell.toolsType] == targetToolType)
            {
                cellsAndToolsInfo[i, row, (int)Cell.isMatch] = 1;
            }
        }
        for (int i = column; i < curentFieldWidth; i++)
        {
            if (toolsOnField[i, row, (int)Cell.toolsType] == targetToolType)
            {
                cellsAndToolsInfo[i, row, (int)Cell.isMatch] = 1;
            }

        }
        for (int i = row; i >= 0; i--)
        {
            if (toolsOnField[column, i, (int)Cell.toolsType] == targetToolType)
            {
                cellsAndToolsInfo[column, i, (int)Cell.isMatch] = 1;
            }
        }
        for (int i = row; i < curentFieldHeight; i++)
        {
            if (toolsOnField[column, i, (int)Cell.toolsType] == targetToolType)
            {
                cellsAndToolsInfo[column, i, (int)Cell.isMatch] = 1;
            }
        }
    }

    private void FindTipInCell(int column, int row)
    {
        int targetToolType = toolsOnField[column, row, (int)Cell.toolsType];
        // массив с индексами строк и рядов для проверки возможных комбинаций
       const int combinationQuantity = 16;
        const int comparedCellsQuantity = 2;
        int[,] checkIndex = new int[combinationQuantity, comparedCellsQuantity * 2]
         { { -2, -1, -1, -1 }, { 1, 2, -1, -1 }, { -2, -1, 1, 1 }, { 1, 2, 1, 1 },
         { -1, -1, -2, -1 }, { 1, 1, -2, -1 }, { -1, -1, 1, 2 }, { 1, 1, 1, 2 },
         { -1, -1, -1, 1 }, { 1, 1, -1, 1 }, { -1, 1, -1, -1 }, { -1, 1, 1, 1 },
         { -3, -2, 0, 0 }, { 2, 3, 0, 0 }, { 0, 0, -3, -2 }, { 0, 0, 2, 3 } };
        for (int i = 0; i < combinationQuantity; i++)
        {
             if (ReturnToolType(column + checkIndex[i, 0], row + checkIndex[i, 2]) == targetToolType && ReturnToolType(column + checkIndex[i, 1], row + checkIndex[i, 3]) == targetToolType)
            {
                cellsAndToolsInfo[column, row, (int)Cell.isTip] = 1;
                return;
            }
        }
    }

    private int ReturnToolType(int i, int j)
    {
        if ((i >= 0 && j >= 0) && (i < curentFieldWidth && j < curentFieldHeight))
        {
            return toolsOnField[i, j, (int)Cell.toolsType];
        }
        else
        {
            return -1;
        }
    }

    private void FindTipsInField() // оформить в корутину и запускать по истечении времени
    {
        for (int i = 0; i < curentFieldWidth; i++)
        {
            for (int j = 0; j < curentFieldHeight; j++)
            {
                FindTipInCell(i, j);
            }
        }
    }

    private void PrintField(int[,,] field, int width, int height, int k)
    {
        string s = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                s = s + field[i, j, k].ToString() + " ";
            }

            s = s + "\n";
        }
        print(s);
    }


    
}
