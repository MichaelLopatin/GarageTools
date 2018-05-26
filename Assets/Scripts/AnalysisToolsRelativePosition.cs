using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisToolsRelativePosition : MonoBehaviour
{
    public delegate void MoveTool(int cellID, int newCellID, float newX, float newY);
    public static event MoveTool MoveToolEvent;
    //public delegate void DeselectCell(int cellID);
    //public static event DeselectCell DeselectCellEvent;


    private enum Cell
    {
        id = 0,
        toolsType = 1,
        isMatch = 1,
        isTip = 2,
        x = 1,
        y = 2,
        IsNotExchange = 0,
        isExchange = 1
    }
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;

    private int[,,] toolsOnField;
    private int[,,] cellsAndToolsInfo;
    private float[,,] toolsXYCoordinates;
    private int[] exchangeTools;


    //   int count = 0;
    private int toolToChange = 0;

    // задать cellsAndToolsInfo

    private void Awake()
    {

    }
    private void OnEnable()
    {
        ToolsAtStart.ChangeGameFieldEvent += SetToolsOnFieldArray;
        Field.ChangeFieldParametersEvent += SetFieldParameters;
        Field.ChangeCellsCoordinatesEvent += SetCellsXYCoordinates;
        MouseController.SelectCellEvent += CheckChangeTools;
        MouseController.SwipeEvent += Swipe;
        Tool.ExchangeMarkEvent += ExchangeMark;
    }

    private void OnDisable()
    {
        ToolsAtStart.ChangeGameFieldEvent -= SetToolsOnFieldArray;
        Field.ChangeFieldParametersEvent -= SetFieldParameters;
        Field.ChangeCellsCoordinatesEvent -= SetCellsXYCoordinates;
        MouseController.SelectCellEvent -= CheckChangeTools;
        Tool.ExchangeMarkEvent -= ExchangeMark;
    }

    private void Start()
    {
        InitializeCellsAndToolsInfo(out cellsAndToolsInfo);
        FindTipsInField();

        //PrintField(toolsOnField, curentFieldWidth, curentFieldHeight, (int)Cell.toolsType);
        //PrintField(cellsAndToolsInfo, curentFieldWidth, curentFieldHeight, (int)Cell.toolsType);
        //PrintField(cellsAndToolsInfo, curentFieldWidth, curentFieldHeight, (int)Cell.isTip);
    }

    private void Update()
    {

    }

    private void InitializeCellsAndToolsInfo(out int[,,] cellsAndToolsInfo)
    {
        cellsAndToolsInfo = new int[curentFieldWidth, curentFieldHeight, (int)Cell.isTip + 1];
        for (int i = 0, k = 0; i < curentFieldWidth; i++)
        {
            for (int j = 0; j < curentFieldHeight; j++, k++)
            {
                cellsAndToolsInfo[i, j, (int)Cell.id] = k;
                cellsAndToolsInfo[i, j, (int)Cell.isMatch] = -1;
                cellsAndToolsInfo[i, j, (int)Cell.isTip] = 0;
            }
        }
    }

    private void SetCellsXYCoordinates(float[,,] cellXYCoordinates)
    {
        toolsXYCoordinates = (float[,,])cellXYCoordinates.Clone();
    }

    private void SetToolsOnFieldArray(int[,,] array)
    {
        toolsOnField = (int[,,])array.Clone();
    }

    private void SetFieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
        int maxId = width * height - 1;
        exchangeTools = new int[maxId + 1];
        for (int i = 0; i < maxId + 1; i++)
        {
            exchangeTools[i] = (int)Cell.IsNotExchange;
        }
    }

    private void ExchangeMark(int id, bool isExchange)
    {
        if (isExchange)
        {
            exchangeTools[id] = (int)Cell.isExchange;
        }
        else
        {
            exchangeTools[id] = (int)Cell.IsNotExchange;
        }
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

    private void CheckChangeTools(int selectedSellID, int lastSelectedСellID)
    {
        if (lastSelectedСellID >= 0 && selectedSellID != lastSelectedСellID)
        {
            if (exchangeTools[selectedSellID] == (int)Cell.IsNotExchange && exchangeTools[lastSelectedСellID] == (int)Cell.IsNotExchange)
            {
                int selectedToolType;
                int lastSelectedToolType;
                int selectedRow;
                int selectedColumn;
                int lastSelectedRow;
                int lastSelectedColumn;

                selectedRow = selectedSellID / curentFieldHeight;
                selectedColumn = selectedSellID - selectedRow * curentFieldWidth;
                //     print(" selectedColumn "+selectedColumn + " selectedRow "+ selectedRow);
                lastSelectedRow = lastSelectedСellID / curentFieldHeight;
                lastSelectedColumn = lastSelectedСellID - lastSelectedRow * curentFieldWidth;
                if (selectedRow == lastSelectedRow)
                {
                    if ((selectedColumn - lastSelectedColumn) == 1 || (selectedColumn - lastSelectedColumn) == -1)
                    {

                        if (MoveToolEvent != null)
                        {
                            MoveToolEvent(lastSelectedСellID, selectedSellID, toolsXYCoordinates[selectedRow, selectedColumn, (int)Cell.x], toolsXYCoordinates[selectedRow, selectedColumn, (int)Cell.y]);
                            MoveToolEvent(selectedSellID, lastSelectedСellID, toolsXYCoordinates[lastSelectedRow, lastSelectedColumn, (int)Cell.x], toolsXYCoordinates[lastSelectedRow, lastSelectedColumn, (int)Cell.y]);
                        }

                        selectedToolType = toolsOnField[selectedRow, selectedColumn, (int)Cell.toolsType];
                        lastSelectedToolType = toolsOnField[lastSelectedRow, lastSelectedColumn, (int)Cell.toolsType];
                        //  print("selectedToolType "+ selectedToolType);
                        // тут проверка на совпадения!!!
                    }
                }
                else if (selectedColumn == lastSelectedColumn)
                {
                    if ((selectedRow - lastSelectedRow) == 1 || (selectedRow - lastSelectedRow) == -1)
                    {

                        if (MoveToolEvent != null)
                        {
                            MoveToolEvent(lastSelectedСellID, selectedSellID, toolsXYCoordinates[selectedRow, selectedColumn, (int)Cell.x], toolsXYCoordinates[selectedRow, selectedColumn, (int)Cell.y]);
                            MoveToolEvent(selectedSellID, lastSelectedСellID, toolsXYCoordinates[lastSelectedRow, lastSelectedColumn, (int)Cell.x], toolsXYCoordinates[lastSelectedRow, lastSelectedColumn, (int)Cell.y]);
                        }

                        selectedToolType = toolsOnField[selectedRow, selectedColumn, (int)Cell.toolsType];
                        lastSelectedToolType = toolsOnField[lastSelectedRow, lastSelectedColumn, (int)Cell.toolsType];
                        // тут проверка на совпадения!!!
                        //  print("selectedToolType "+ selectedToolType);
                    }
                }
            }
        }
    }

    private void Swipe(int swipeCellID, SwipeDirection swipeDirection)
    {

        if (exchangeTools[swipeCellID] == (int)Cell.IsNotExchange)
        {
            int cellToExchangeID = -1;
            print("swipeDirection " + swipeDirection);
            switch (swipeDirection)
            {
                case SwipeDirection.up:
                    cellToExchangeID = swipeCellID - curentFieldHeight;
                    break;
                case SwipeDirection.right:
                    cellToExchangeID = swipeCellID + 1;
                    break;
                case SwipeDirection.down:
                    cellToExchangeID = swipeCellID + curentFieldHeight;
                    break;
                case SwipeDirection.left:
                    cellToExchangeID = swipeCellID - 1;
                    break;
                default:
                    break;
            }
            if (exchangeTools[cellToExchangeID] == (int)Cell.IsNotExchange)
            {
                int swipeCellToolType;
                int cellToExchangeToolType;
                int swipeCellRow;
                int swipeCellColumn;
                int cellToExchangeRow;
                int cellToExchangeColumn;
                swipeCellRow = swipeCellID / curentFieldHeight;
                swipeCellColumn = swipeCellID - swipeCellRow * curentFieldWidth;

                cellToExchangeRow = cellToExchangeID / curentFieldHeight;
                cellToExchangeColumn = cellToExchangeID - cellToExchangeRow * curentFieldWidth;
                if (MoveToolEvent != null)
                {
                    MoveToolEvent(cellToExchangeID, swipeCellID, toolsXYCoordinates[swipeCellRow, swipeCellColumn, (int)Cell.x], toolsXYCoordinates[swipeCellRow, swipeCellColumn, (int)Cell.y]);
                    MoveToolEvent(swipeCellID, cellToExchangeID, toolsXYCoordinates[cellToExchangeRow, cellToExchangeColumn, (int)Cell.x], toolsXYCoordinates[cellToExchangeRow, cellToExchangeColumn, (int)Cell.y]);
                }

                swipeCellToolType = toolsOnField[swipeCellRow, swipeCellColumn, (int)Cell.toolsType];
                cellToExchangeToolType = toolsOnField[cellToExchangeRow, cellToExchangeColumn, (int)Cell.toolsType];

            }
        }
    }

}
