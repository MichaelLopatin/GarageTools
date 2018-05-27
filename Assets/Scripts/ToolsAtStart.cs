using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsAtStart : MonoBehaviour
{
    private void Awake()
    {

    }

    private void Start()
    {
        FillGameField(ref Field.toolsOnField, Field.CurentFieldWidth, Field.CurentFieldWidth);
        PrintField(Field.toolsOnField, Field.CurentFieldWidth, Field.CurentFieldHeight);
        SetToolsOnFieldInStart(Field.toolsOnField, ref ToolsPool.toolsReservedListOfStacks, ref ToolsPool.toolsOnFieldListOfStacks, Field.CurentFieldWidth, Field.CurentFieldHeight, Field.CurentUnitScale);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void FillGameField(ref int[] toolsOnField, int width, int height)
    {
        int[] toolTypesNumber = new int[(int)((int)ToolType.toolBox * 0.5f)];
        int typesSingleToolsQuantity = 0;
        int fieldSize = Field.FieldSize;
        for (int i = 0, j = 0; i < (int)ToolType.toolBox; i += 2, j++)
        {
            toolTypesNumber[j] = i;
            typesSingleToolsQuantity++;
        }
        for (int i = 0; i < fieldSize; i++)
        {
            Field.toolsOnField[i] = toolTypesNumber[Random.Range(0, typesSingleToolsQuantity)];
        }
        PrintField(Field.toolsOnField, Field.CurentFieldWidth, Field.CurentFieldHeight);
        CheckAndRemoveMatchesInStart(ref Field.toolsOnField, toolTypesNumber, typesSingleToolsQuantity, width, height);
    }

    private void CheckAndRemoveMatchesInStart(ref int[] toolsOnField, int[] toolTypesNumber, int typesSingleToolsQuantity, int width, int height)
    {
        int curentCellType = -1;
        int lastCellType = -1;
        int match = 0;
        int fieldSize = Field.FieldSize;

        for (int i = 0, k = 0; i < height; i++)
        {
            match = 0;
            lastCellType = -1;
            for (int j = 0; j < width; j++, k++)
            {
                curentCellType = Field.toolsOnField[k];
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
                    RemoveMatchesInStart(ref Field.toolsOnField, i, j, typesSingleToolsQuantity);
                    curentCellType = Field.toolsOnField[k];
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
                curentCellType = Field.toolsOnField[j * height + i];
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
                    RemoveMatchesInStart(ref Field.toolsOnField, j, i, typesSingleToolsQuantity);
                    curentCellType = Field.toolsOnField[j * height + i];
                    match = 0;
                }
                lastCellType = curentCellType;
            }
        }
    }

    private void RemoveMatchesInStart(ref int[] toolsOnField, int column, int row, int typesSingleToolsQuantity)
    {
        //   1
        //0  C  2
        //   3
        int id = row * Field.CurentFieldWidth + column;
        int[] neighbours = new int[4];
        int minTypeNumber = 0;
        int maxTypeNumber = (typesSingleToolsQuantity - 1) * 2;

        if (column == 0)
        {
            neighbours[0] = -1;
        }
        else
        {
            neighbours[0] = Field.toolsOnField[id - 1];
        }

        if (column == Field.CurentFieldWidth - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = Field.toolsOnField[id + 1];
        }

        if (row == 0)
        {
            neighbours[1] = -1;
        }
        else
        {
            neighbours[1] = Field.toolsOnField[id - Field.CurentFieldWidth];
        }

        if (row == Field.CurentFieldHeight - 1)
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = Field.toolsOnField[id + Field.CurentFieldWidth];
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
            Field.toolsOnField[id] = minTypeNumber;
        }
        else
        {
            Field.toolsOnField[id] = maxTypeNumber;
        }
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

    private void SetToolsOnFieldInStart(int[] toolsOnField, ref List<Stack<GameObject>> toolsReservedListOfStacks, ref List<Stack<GameObject>> toolsOnFieldListOfStacks, int width, int height, float curentUnitScale)
    {
        GameObject tool;
        Vector3 curentToolPosition = Vector3.zero;
        Vector3 unitScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);
        for (int i = 0, k = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++, k++)
            {
                tool = toolsReservedListOfStacks[Field.toolsOnField[k]].Pop();
                tool.GetComponent<Tool>().CellID = k;
                curentToolPosition.x = Field.cellsXYCoord[k, (int)Cell.x];
                curentToolPosition.y = Field.cellsXYCoord[k, (int)Cell.y];
                tool.transform.position = curentToolPosition;
                tool.transform.localScale = unitScale;
                toolsOnFieldListOfStacks[Field.toolsOnField[k]].Push(tool);
                tool.SetActive(true);
            }
        }
    }

    private void PrintField(int[] field, int width, int height)
    {
        string s = "";
        for (int i = 0, k = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++, k++)
            {
                s = s + field[k].ToString() + " ";
            }

            s = s + "\n";
        }
        print(s);
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
}