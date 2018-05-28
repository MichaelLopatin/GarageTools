using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveCount
{
    first = 1,
    second
}


public class AnalysisToolsRelativePosition : MonoBehaviour
{
    private enum Pair
    {
        first = 0,
        second = 1,
    }

    public delegate void MoveTool(int curentCellID, int newCellID, float newX, float newY, MoveCount moveCount);
    public static event MoveTool MoveToolEvent;
    public delegate void GatherTool(int cellID);
    public static event GatherTool GatherToolEvent;
    //public delegate void CreateToolsGroup(int cellID, ToolType toolType);
    //public static event CreateToolsGroup CreateToolsGroupEvent;

    private static int[] exchangeTools;

    private int[] tipsOnField;
    private int[] haveMatchOnField;

    private void Awake()
    {

    }
    private void OnEnable()
    {
        MouseController.SelectCellEvent += CheckChangeTools;
        MouseController.SwipeEvent += Swipe;
        Tool.ExchangeMarkEvent += ExchangeMark;
        Tool.EndToolMovementEvent += CheckMatchesInCell;
        SetExchangeTools(out exchangeTools);

        SetTipsAndMatchOnField(out tipsOnField, out haveMatchOnField);
        SetExchangeTools(out exchangeTools);
        FindTipsInField();
    }

    private void OnDisable()
    {
        MouseController.SelectCellEvent -= CheckChangeTools;
        Tool.ExchangeMarkEvent -= ExchangeMark;
        Tool.EndToolMovementEvent -= CheckMatchesInCell;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void ExchangeMark(int id, bool isExchange)
    {
        if (isExchange)
        {
            exchangeTools[id] = (int)Cell.isExchange;
        }
        else
        {
            exchangeTools[id] = (int)Cell.isNotExchange;
        }
    }

    private void SetExchangeTools(out int[] exchangeTools)
    {
        int fieldSize = Field.FieldSize;
        exchangeTools = new int[fieldSize];
        for (int i = 0; i < fieldSize; i++)
        {
            exchangeTools[i] = (int)Cell.isNotExchange;
        }
    }
    private void FindTipInCell(int cellID)
    {
        int row = (int)(cellID / Field.CurentFieldWidth);
        int column = cellID - row * Field.CurentFieldWidth;
        int targetToolType = Field.toolsOnField[cellID];
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
                tipsOnField[cellID] = 1;
                return;
            }
        }
    }

    private int ReturnToolType(int i, int j)
    {
        if ((i >= 0 && j >= 0) && (i < Field.CurentFieldWidth && j < Field.CurentFieldHeight))
        {
            return Field.toolsOnField[(i * Field.CurentFieldWidth + j)];
        }
        else
        {
            return -1;
        }
    }

    private void FindTipsInField()
    {
        int fieldSize = Field.FieldSize;
        for (int i = 0; i < fieldSize; i++)
        {
            FindTipInCell(i);
        }
    }

    private void SetTipsAndMatchOnField(out int[] tipsOnField, out int[] haveMatchOnField)
    {
        int size = Field.FieldSize;
        tipsOnField = new int[size];
        haveMatchOnField = new int[size];
        for (int i = 0; i < size; i++)
        {
            tipsOnField[i] = (int)Cell.isNotTip;
            haveMatchOnField[i] = (int)Cell.isNotMatch;
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

    private void CheckChangeTools(int selectedSellID, int lastSelectedСellID)
    {
        if (lastSelectedСellID >= 0 && selectedSellID != lastSelectedСellID)
        {
            if (exchangeTools[selectedSellID] == (int)Cell.isNotExchange && exchangeTools[lastSelectedСellID] == (int)Cell.isNotExchange)
            {
                int selectedToolType;
                int lastSelectedToolType;

                if (selectedSellID - lastSelectedСellID == -1 ||
                    selectedSellID - lastSelectedСellID == 1 ||
                    selectedSellID - lastSelectedСellID == -Field.CurentFieldWidth ||
                    selectedSellID - lastSelectedСellID == Field.CurentFieldWidth)
                {
                    if (MoveToolEvent != null)
                    {
                        MoveToolEvent(lastSelectedСellID, selectedSellID, Field.cellsXYCoord[selectedSellID, (int)Cell.x], Field.cellsXYCoord[selectedSellID, (int)Cell.y], MoveCount.first);
                        MoveToolEvent(selectedSellID, lastSelectedСellID, Field.cellsXYCoord[lastSelectedСellID, (int)Cell.x], Field.cellsXYCoord[lastSelectedСellID, (int)Cell.y], MoveCount.first);
                    }
                    selectedToolType = Field.toolsOnField[selectedSellID];
                    lastSelectedToolType = Field.toolsOnField[lastSelectedСellID];
                    // тут проверка на совпадения!!!
                }
            }
        }
    }

    private void Swipe(int swipeCellID, SwipeDirection swipeDirection)
    {
        if (exchangeTools[swipeCellID] == (int)Cell.isNotExchange)
        {
            int cellToExchangeID = -1;
            switch (swipeDirection)
            {
                case SwipeDirection.up:
                    cellToExchangeID = swipeCellID - Field.CurentFieldHeight;
                    break;
                case SwipeDirection.right:
                    cellToExchangeID = swipeCellID + 1;
                    break;
                case SwipeDirection.down:
                    cellToExchangeID = swipeCellID + Field.CurentFieldHeight;
                    break;
                case SwipeDirection.left:
                    cellToExchangeID = swipeCellID - 1;
                    break;
                default:
                    break;
            }
            if (exchangeTools[cellToExchangeID] == (int)Cell.isNotExchange)
            {
                int swipeCellToolType;
                int cellToExchangeToolType;

                if (MoveToolEvent != null)
                {
                    MoveToolEvent(cellToExchangeID, swipeCellID, Field.cellsXYCoord[swipeCellID, (int)Cell.x], Field.cellsXYCoord[swipeCellID, (int)Cell.y], MoveCount.first);
                    MoveToolEvent(swipeCellID, cellToExchangeID, Field.cellsXYCoord[cellToExchangeID, (int)Cell.x], Field.cellsXYCoord[cellToExchangeID, (int)Cell.y], MoveCount.first);
                }

                swipeCellToolType = Field.toolsOnField[swipeCellID];
                cellToExchangeToolType = Field.toolsOnField[cellToExchangeID];
            }
        }
    }

    private void CheckMatchesInCell(int id, int lastId, MoveCount moveCount)
    {
        if (moveCount == MoveCount.second)
        {
            return;
        }
        int width = Field.CurentFieldWidth;
        int height = Field.CurentFieldHeight;
        int toolType = Field.toolsOnField[id];
        int matcheInRow = 0;
        int matcheInColumn = 0;
        int row = id / width;
        int column = id - row * height;
        int[] matchInRowID = new int[5] { -1, -1, -1, -1, -1 };
        int[] matchInColumnID = new int[5] { -1, -1, -1, -1, -1 };

        int index = 0;
        if (row != 0)
        {
            for (int i = 1; i <= row; i++)
            {
                if (Field.toolsOnField[id - i * width] == toolType)
                {
                    matchInColumnID[index] = id - i * width;
                    index++;
                    matcheInColumn++;
                }
                else
                {
                    break;
                }
            }
        }
        if (row != height - 1)
        {
            for (int i = 1; i < height-row; i++)
            {
                if (Field.toolsOnField[id + i * width] == toolType)
                {
                    matchInColumnID[index] = id + i * width;
                    index++;
                    matcheInColumn++;
                }
                else
                {
                    break;
                }
            }
        }

        index = 0;
        if(column!=0)
        {
            for (int i = 1; i <= column; i++)
            {
                if (Field.toolsOnField[id -i] == toolType)
                {
                    matchInRowID[index] = id - i;
                    index++;
                    matcheInRow++;
                }
                else
                {
                    break;
                }
            }
        }
        if (column != width-1)
        {
            for (int i = 1; i < width-column; i++)
            {
                if (Field.toolsOnField[id + i] == toolType)
                {
                    matchInRowID[index] = id + i;
                    index++;
                    matcheInRow++;
                }
                else
                {
                    break;
                }
            }
        }

        //    print(" matcheInRow " + matcheInRow + " matcheInColumn " + matcheInColumn);
        //PrintArr(matchInRowID);
        //PrintArr(matchInColumnID);
        if (matcheInRow < 2 && matcheInColumn < 2)
        {
            haveMatchOnField[id] = (int)Cell.isNotMatch;
        }
        else
        {
            haveMatchOnField[id] = (int)Cell.isMatch;
            print("haveMatchOnField[" + id + "] = " + haveMatchOnField[id]);
        }
        StartCoroutine(CheckReturnExgangeCoroutine(id, lastId, matchInRowID, matcheInRow, matchInColumnID, matcheInColumn));

    }

    private IEnumerator CheckReturnExgangeCoroutine(int checkId, int secondId, int[] matchInRowID, int matcheInRow, int[] matchInColumnID, int matcheInColumn)
    {
        print("StartCoroutine(CheckReturnExgangeCoroutine");
        print("matcheInRow= " + matcheInRow + " matcheInColumn= " + matcheInColumn);
        yield return null;
        if (haveMatchOnField[checkId] == (int)Cell.isNotMatch && haveMatchOnField[secondId] == (int)Cell.isNotMatch)
        {
            if (MoveToolEvent != null)
            {
                MoveToolEvent(checkId, secondId, Field.cellsXYCoord[secondId, (int)Cell.x], Field.cellsXYCoord[secondId, (int)Cell.y], MoveCount.second);
            }
        }
        else if(haveMatchOnField[checkId] == (int)Cell.isMatch)
        {
            if (matcheInRow > 1)
            {
                print("matcheInRow > 1 start MatchManipulation InRow");
                MatchManipulation(matchInRowID, checkId, matcheInRow);
            }
            if (matcheInColumn > 1)
            {
                print("matcheInColumn > 1 start MatchManipulation InColumn");
                MatchManipulation(matchInColumnID, checkId, matcheInColumn);
            }

        }
        yield return null;
       haveMatchOnField[checkId] = (int)Cell.isNotMatch;
    }

    private void MatchManipulation(int[] matchID, int basicId, int matchCount)
    {
        print("matchCount= " + matchCount);
        PrintArr(matchID);
        print("basicId= " + basicId + " Field.toolsOnField[" + basicId + "] = " + Field.toolsOnField[basicId]);
        if (GatherToolEvent != null)
        {
            print("GatherToolEvent basicId= " + basicId);
            GatherToolEvent(basicId);
        }
        switch (matchCount)
        {
            case 2:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

  
}
