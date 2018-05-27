using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisToolsRelativePosition : MonoBehaviour
{
    public delegate void MoveTool(int cellID, int newCellID, float newX, float newY);
    public static event MoveTool MoveToolEvent;
    public delegate void DestroyTool(int cellID, int toolType);
    public static event DestroyTool DestroyToolEvent;
    public delegate void CreateToolsGroup(int cellID, ToolType toolType);
    public static event CreateToolsGroup CreateToolsGroupEvent;

    private static int[] exchangeTools;

    private int[] tipsOnField;

    private void Awake()
    {

    }
    private void OnEnable()
    {
        MouseController.SelectCellEvent += CheckChangeTools;
        MouseController.SwipeEvent += Swipe;
        Tool.ExchangeMarkEvent += ExchangeMark;
        SetExchangeTools(out exchangeTools);
    }

    private void OnDisable()
    {
        MouseController.SelectCellEvent -= CheckChangeTools;
        Tool.ExchangeMarkEvent -= ExchangeMark;
    }

    private void Start()
    {
        SetExchangeTools(out exchangeTools);
        FindTipsInField(out tipsOnField);
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
            exchangeTools[id] = (int)Cell.IsNotExchange;
        }
    }

    private void SetExchangeTools(out int[] exchangeTools)
    {
        int fieldSize = Field.FieldSize;
        exchangeTools = new int[fieldSize];
        for (int i = 0; i < fieldSize; i++)
        {
            exchangeTools[i] = (int)Cell.IsNotExchange;
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
                Field.toolsOnField[cellID] = 1;
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

    private void FindTipsInField(out int[] tipsOnField)
    {
        int fieldSize = Field.FieldSize;
        tipsOnField = new int[fieldSize];
        for (int i = 0; i < fieldSize; i++)
        {
            FindTipInCell(i);
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

    private void CheckChangeTools(int selectedSellID, int lastSelectedСellID)
    {
        if (lastSelectedСellID >= 0 && selectedSellID != lastSelectedСellID)
        {
            if (exchangeTools[selectedSellID] == (int)Cell.IsNotExchange && exchangeTools[lastSelectedСellID] == (int)Cell.IsNotExchange)
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
                        MoveToolEvent(lastSelectedСellID, selectedSellID, Field.cellsXYCoord[selectedSellID, (int)Cell.x], Field.cellsXYCoord[selectedSellID, (int)Cell.y]);
                        MoveToolEvent(selectedSellID, lastSelectedСellID, Field.cellsXYCoord[lastSelectedСellID, (int)Cell.x], Field.cellsXYCoord[lastSelectedСellID, (int)Cell.y]);
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
        if (exchangeTools[swipeCellID] == (int)Cell.IsNotExchange)
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
            if (exchangeTools[cellToExchangeID] == (int)Cell.IsNotExchange)
            {
                int swipeCellToolType;
                int cellToExchangeToolType;

                if (MoveToolEvent != null)
                {
                    MoveToolEvent(cellToExchangeID, swipeCellID, Field.cellsXYCoord[swipeCellID, (int)Cell.x], Field.cellsXYCoord[swipeCellID, (int)Cell.y]);
                    MoveToolEvent(swipeCellID, cellToExchangeID, Field.cellsXYCoord[cellToExchangeID, (int)Cell.x], Field.cellsXYCoord[cellToExchangeID, (int)Cell.y]);
                }

                swipeCellToolType = Field.toolsOnField[swipeCellID];
                cellToExchangeToolType = Field.toolsOnField[cellToExchangeID];
            }
        }
    }

    /*
    private void CheckMatches(int row, int column, int targetToolType)
    {
        int matchInRow = 0;
        int matchInColumn = 0;
        int[] matchInRowID = new int[5] { -1, -1, -1, -1, -1 };
        int[] matchInColumnID = new int[5] { -1, -1, -1, -1, -1 };
        int j = 0;
        for (int i = column; i < Field.CurentFieldWidth; i++)
        {
        //    print("toolsOnField["+row+", "+i+", (int)Cell.toolsType] " + toolsOnField[row, i, (int)Cell.toolsType]);
            if (toolsOnField[row, i, (int)Cell.toolsType] == targetToolType)
            {
                
  //              cellsAndToolsInfo[row, i, (int)Cell.isMatch] = 1;
                matchInRowID[j] = row * Field.CurentFieldWidth + i;
                j++;
                    matchInRow++;
         //       print("matchInRow " + matchInRow + " targetToolType "+ targetToolType+" id "+ matchInRowID[j-1]);
            }
            else
            {
                break;
            }
        }
        for (int i = column-1; i >=0; i--)
        {
        //    print("toolsOnField[" + row + ", " + i + ", (int)Cell.toolsType] " + toolsOnField[row, i, (int)Cell.toolsType]);

            if (toolsOnField[row, i, (int)Cell.toolsType] == targetToolType)
            {
   //             cellsAndToolsInfo[row, i, (int)Cell.isMatch] = 1;
                matchInRowID[j] = row * Field.CurentFieldWidth + i;
                j++;
                matchInRow++;
        //        print("matchInRow " + matchInRow + " targetToolType " + targetToolType + " id " + matchInRowID[j - 1]);
            }
            else
            {
                break;
            }
        }
        j = 0;
        for (int i = row; i < Field.CurentFieldHeight; i++)
        {
  //          print("toolsOnField["+i+", "+column+", (int)Cell.toolsType] " + toolsOnField[i, column, (int)Cell.toolsType]);
            if (toolsOnField[i, column, (int)Cell.toolsType] == targetToolType)
            {
  //              cellsAndToolsInfo[i, column, (int)Cell.isMatch] = 1;
                matchInColumnID[j] = i * Field.CurentFieldWidth + column;
                j++;
                matchInColumn++;
        //        print("matchInColumn " + matchInColumn);
            }
            else
            {
                break;
            }
        }
        for (int i = row-1; i >=0; i--)
        {
      //      print("toolsOnField[" + i + ", " + column + ", (int)Cell.toolsType] " + toolsOnField[i, column, (int)Cell.toolsType]);

            if (toolsOnField[i, column, (int)Cell.toolsType] == targetToolType)
            {
 //               cellsAndToolsInfo[i, column, (int)Cell.isMatch] = 1;
                matchInColumnID[j] = i * Field.CurentFieldWidth + column;
                j++;
                matchInColumn++;
         //       print("matchInColumn " + matchInColumn);
            }
            else
            {
                break;
            }
        }
        if (matchInColumn > 2)
        {
            switch(matchInColumn)
            {
                case 3:
                    for(int i=0;i<3;i++)
                    {
                        DestroyToolEvent(matchInColumnID[i], targetToolType);
                        // добавить в массив тип инструмента -1 чтобы по этому флагу спускать вышестоящие фигуры
                    }
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                    {
                        DestroyToolEvent(matchInColumnID[i], targetToolType);
                    }
                    break;
                case 5:
                    for (int i = 0; i < 5; i++)
                    {
                        DestroyToolEvent(matchInColumnID[i], targetToolType);
                    }
                    break;
                default:
                    break;
            }
        }
        if (matchInRow > 2)
        {
            switch (matchInRow)
            {
                case 3:
                    for (int i = 0; i < 3; i++)
                    {
                        DestroyToolEvent(matchInRowID[i], targetToolType);
                        // добавить в массив тип инструмента -1 чтобы по этому флагу спускать вышестоящие фигуры
                    }
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                    {
                        DestroyToolEvent(matchInRowID[i], targetToolType);
                    }
                    break;
                case 5:
                    for (int i = 0; i < 5; i++)
                    {
                        DestroyToolEvent(matchInRowID[i], targetToolType);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    */

}
