using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tools : MonoBehaviour
{
    public delegate void ChangeGameField(int[,,] cellsXYCoordinates);
    public static event ChangeGameField ChangeGameFieldEvent;


    private enum Cell
    {
        id = 0,
        toolsType = 1,
        x = 1,
        y = 2
    }

    private float[,,] toolsXYCoordinates;

    private int[,,] gameField;

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
        Field.ChangeCellsCoordinatesEvent += SetCellsXYCoordinates;
        Field.ChangeFieldParametersEvent += SetFieldParameters;
    }

    private void OnDisable()
    {
        Field.ChangeCellsCoordinatesEvent -= SetCellsXYCoordinates;
        Field.ChangeFieldParametersEvent -= SetFieldParameters;
    }

    private void SetCellsXYCoordinates(float[,,] cellXYCoordinates)
    {
        toolsXYCoordinates = (float[,,])cellXYCoordinates.Clone();
    }

    private void SetFieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
        curentUnitScale = scale;
    }

    private void FillGameField(out int[,,] gameField, int width, int height)
    {
        int[] toolTypesNumber = new int[(int)((int)ToolType.toolBox * 0.5f)];
        int typesSingleToolsQuantity = 0;
        for (int i = 0, j = 0; i < (int)ToolType.toolBox; i += 2, j++)
        {
            toolTypesNumber[j] = i;
            typesSingleToolsQuantity++;
        }
        gameField = new int[width, height, 2];
        for (int i = 0, k = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++, k++)
            {
                gameField[i, j, (int)Cell.id] = k;
                gameField[i, j, (int)Cell.toolsType] = toolTypesNumber[Random.Range(0, typesSingleToolsQuantity)];
            }
        }
        PrintField(gameField, curentFieldWidth, curentFieldHeight);
        CheckAndRemoveMatchesInStart(ref gameField, toolTypesNumber, typesSingleToolsQuantity, width, height);
        if(ChangeGameFieldEvent!=null)
        {
            ChangeGameFieldEvent(gameField);
        }
    }

    private void CheckAndRemoveMatchesInStart(ref int[,,] gameField, int[] toolTypesNumber, int typesSingleToolsQuantity, int width, int height)
    {
        int curentCellType = -1;
        int lastCellType = -1;
        int match = 0;

        for (int i = 0; i < height; i++)
        {
            match = 0;
            lastCellType = -1;
            for (int j = 0; j < width; j++)
            {
                curentCellType = gameField[i, j, (int)Cell.toolsType];
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
                    curentCellType = gameField[j, i, (int)Cell.toolsType];
                    match = 0;
                }
                lastCellType = curentCellType;
            }
        }
        curentCellType = -1;
        lastCellType = -1;

        for (int i = 0; i < height; i++)
        {
            match = 0;
            lastCellType = -1;
            for (int j = 0; j < width; j++)
            {
                curentCellType = gameField[j, i, (int)Cell.toolsType];
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
                    curentCellType = gameField[j, i, (int)Cell.toolsType];
                    match = 0;
                }
                lastCellType = curentCellType;
            }
        }
    }

    private void RemoveMatchesInStart(ref int[,,] gameField, int column, int row, int typesSingleToolsQuantity)
    {
        //   1
        //0  C  2
        //   3
        int[] neighbours = new int[4];
        int minTypeNumber = 0;
        int maxTypeNumber = (typesSingleToolsQuantity - 1) * 2;

        if (column == 0)
        {
            neighbours[0] = -1;
        }
        else
        {
            neighbours[0] = gameField[column - 1, row, (int)Cell.toolsType];
        }

        if (column == curentFieldWidth - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = gameField[column + 1, row, (int)Cell.toolsType];
        }

        if (row == 0)
        {
            neighbours[1] = -1;
        }
        else
        {
            neighbours[1] = gameField[column, row - 1, (int)Cell.toolsType];
        }

        if (row == curentFieldHeight - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = gameField[column, row + 1, (int)Cell.toolsType];
        }
        SimpleSort(ref neighbours);
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
        if (Random.Range(0, 2) == 0)
        {
            gameField[column, row, (int)Cell.toolsType] = minTypeNumber;
        }
        else
        {
            gameField[column, row, (int)Cell.toolsType] = maxTypeNumber;
        }
    }

    private void PrintField(int[,,] field, int width, int height)
    {
        string s = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                s = s + field[i, j, 1].ToString() + " ";
            }

            s = s + "\n";
        }
        print(s);
    }

    private void SimpleSort(ref int[] arr)
    {
        int temp;
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    temp = arr[j + 1];
                    arr[j + 1] = arr[j];
                    arr[j] = temp;
                }
            }
        }
    }

    private void PrintArr(int[] arr) // для текстового контроля
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s = s + arr[i].ToString() + " ";
        }
        print(s);
    }

    private void SetToolsOnFieldInStart(int[,,] gameField, ref List<Stack<GameObject>> toolsReservedListOfStacks, ref List<Stack<GameObject>> toolsOnFieldListOfStacks, int width, int height, float curentUnitScale)
    {
        GameObject tool;
        Vector3 curentToolPosition = Vector3.zero;
        Vector3 unitScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tool = toolsReservedListOfStacks[gameField[i, j, (int)Cell.toolsType]].Pop();
                tool.GetComponent<ToolInfo>().CellID = gameField[i, j, (int)Cell.id];
                curentToolPosition.x = toolsXYCoordinates[i, j, (int)Cell.x];
                curentToolPosition.y = toolsXYCoordinates[i, j, (int)Cell.y];
                tool.transform.position = curentToolPosition;
                tool.transform.localScale = unitScale;
                toolsOnFieldListOfStacks[gameField[i, j, (int)Cell.toolsType]].Push(tool);
                tool.SetActive(true);
            }
        }
    }


}
