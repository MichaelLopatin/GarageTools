using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolMoveType
{
    firstExgange = 1,
    secondExgenge,
    moveDown
}


public class AnalysisToolsRelativePosition : MonoBehaviour
{
    //private enum Pair
    //{
    //    first = 0,
    //    second = 1,
    //}

    public delegate void MoveTool(int curentCellID, int newCellID, float newX, float newY, ToolMoveType toolMoveType);
    public static event MoveTool MoveToolEvent;
    public delegate void GatherTool(int cellID);
    public static event GatherTool GatherToolEvent;
    public delegate void MoveDownTool(int cellID);
    public static event MoveDownTool MoveDownToolEvent;
    public delegate void ShakeUp();
    public static event ShakeUp ShakeUpEvent;
    public delegate void VisibleTip(int cellId);
    public static event VisibleTip VisibleTipEvent;
    public delegate void InvisibleTip(int cellId);
    public static event InvisibleTip InvisibleTipEvent;
    public delegate void PlayerMove();
    public static event PlayerMove PlayerMoveEvent;
    //public delegate void CreateToolsGroup(int cellID, ToolType toolType);
    //public static event CreateToolsGroup CreateToolsGroupEvent;

    private static int[] exchangeTools;

    //public static bool isChanging = false;
    //public static bool isMovingDown = false;
    //public static bool isGathering = false;
    //public static bool isMatchManipulation = false;

    private int[] tipsOnField;
    private int[] haveMatchOnField;
    private int[] columnsWithEmptyCells;

    private int[] columnsWithFallingTools;

    private bool fallingToolsExist = false;
    private bool wasFalling = false;
    private bool matchesAllField = false;
    private bool tipIsVisible = false;
    private int tipCellId = -1;


    private float controlTime;

    private void Awake()
    {

    }
    private void OnEnable()
    {
        MouseController.SelectCellEvent += CheckChangeTools;
        MouseController.SwipeEvent += Swipe;
        Tool.ExchangeMarkEvent += ExchangeMark;
        Tool.EndToolMovementEvent += CheckMatchesInCell;
        Tool.ZeroControlTimeEvent += ZeroControlTime;
        InvisibleTipEvent += ResetTip;
  


    }

    private void OnDisable()
    {
        MouseController.SelectCellEvent -= CheckChangeTools;
        MouseController.SwipeEvent -= Swipe;
        Tool.ExchangeMarkEvent -= ExchangeMark;
        Tool.EndToolMovementEvent -= CheckMatchesInCell;
        Tool.ZeroControlTimeEvent -= ZeroControlTime;
        InvisibleTipEvent -= ResetTip;
    }

    private void Start()
    {
        SetExchangeTools(out exchangeTools);

        SetTipsAndMatchAndColumnsOnField(out tipsOnField, out haveMatchOnField, out columnsWithEmptyCells);

        FindTipsInField();
        columnsWithFallingTools = new int[Field.CurentFieldWidth];
        for (int i = 0; i < Field.CurentFieldWidth; i++)
        {
            columnsWithFallingTools[i] = 0;
            StartCoroutine(EmptyCellsManipulationCoroutine(i));
        }
    }

    private void Update()
    {
        controlTime += Time.deltaTime;
        if (controlTime > 4f && !tipIsVisible)
        {
            tipIsVisible = true;
            ChooseTip();
        }
        if (wasFalling && !fallingToolsExist && !matchesAllField)
        {
            StartCoroutine(CheckMatchesAfterFallAllField());
        }

        if (Input.GetMouseButtonDown(1))
        {
            ShakeUpEvent();
        }
    }

    private void ResetTip(int tipId)
    {
        tipIsVisible = false;
        tipCellId = -1;
        controlTime = 0f;
    }

    private void ZeroControlTime()
    {
        controlTime = 0f;
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
        int row = cellID / Field.CurentFieldWidth;
        int column = cellID - row * Field.CurentFieldWidth;
        int targetToolType = Field.toolsOnField[cellID];
    //    print("сравнение ID " + cellID + ", row " + row + ", column " + column + " type ID " + Field.toolsOnField[cellID] + " typeFunc " + ReturnToolType(row, column));

        // массив с индексами строк и рядов для проверки возможных комбинаций {ряд1, строка1, ряд2, строка2}
        const int combinationQuantity = 16;
        const int comparedCellsQuantity = 2;
        int[,] checkIndex = new int[combinationQuantity, comparedCellsQuantity * 2]
         { {-1,-2,-1,-1}, {-1,1,-1,2}, {1,-2,1,-1}, {1,1,1,2},
{-2,-1,-1,-1}, {-2,1,-1,1}, {1,-1,2,-1}, {1,1,2,1},
{-1,-1,1,-1}, {-1,1,1,1}, {-1,-1,-1,1}, {1,-1,1,1},
{0,-3,0,-2}, {0,2,0,3}, {-3,0,-2,0}, {2,0,3,0 } };

        for (int i = 0; i < combinationQuantity; i++)
        {
            if (ReturnToolType(row + checkIndex[i, 0], column + checkIndex[i, 1]) == targetToolType &&
                ReturnToolType(row + checkIndex[i, 2], column + checkIndex[i, 3]) == targetToolType)
            {
   //             print("совпадение checkIndex " + i + " id " + cellID + " type " + targetToolType + " " +
  //   (row + checkIndex[i, 2]) + " " + (column + checkIndex[i, 0]) + " t " + ReturnToolType(column + checkIndex[i, 0], row + checkIndex[i, 2]) + (row + checkIndex[i, 3]) + " " + (column + checkIndex[i, 1]) + " t " + ReturnToolType(column + checkIndex[i, 1], row + checkIndex[i, 3]));
                tipsOnField[cellID] = (int)Cell.isTip;
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

    private void ChooseTip()
    {
        int fieldSize = Field.FieldSize;
        int[] tips = new int[fieldSize];
        int tipsNumber = 0;
        for (int i = 0; i < fieldSize; i++)
        {
            tipsOnField[i] = 0;
        }
        FindTipsInField();
        for (int i = 0; i < fieldSize; i++)
        {
            if (tipsOnField[i] > 0)
            {
                tips[tipsNumber] = i;
                tipsNumber++;
            }
        }
        //  print("tipsNumber " + tipsNumber);
        // PrintArr(tips);
        tipCellId = tips[Random.Range(0, tipsNumber)];
        //print("tipCellId " + tipCellId);
        if (VisibleTipEvent != null)
        {
            VisibleTipEvent(tipCellId);
        }

    }

    private void FindTipsInField()
    {
        int fieldSize = Field.FieldSize;
        int tips = 0;

        for (int i = 0; i < fieldSize; i++)
        {
            FindTipInCell(i);
        }
     //   PrintArr(tipsOnField);
      //  PrintArr(Field.toolsOnField);
        for (int i = 0; i < fieldSize; i++)
        {
            tips += tipsOnField[i];
        }
        if (tips < 0)
        {
            if (ShakeUpEvent != null)
            {
                ShakeUpEvent();
            }
        }
    }

    private void SetTipsAndMatchAndColumnsOnField(out int[] tipsOnField, out int[] haveMatchOnField, out int[] columnsWithEmptyCells)
    {
        print("SetTipsAndMatchAndColumnsOnField  fieldSize");
        int size = Field.FieldSize;
        int width = Field.CurentFieldWidth;
        tipsOnField = new int[size];
        haveMatchOnField = new int[size];
        for (int i = 0; i < size; i++)
        {
            tipsOnField[i] = (int)Cell.isNotTip;
            haveMatchOnField[i] = (int)Cell.isNotMatch;
        }
        columnsWithEmptyCells = new int[width];
        for (int i = 0; i < width; i++)
        {
            columnsWithEmptyCells[i] = 0;
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
                    //isChanging = true;
                    if (MoveToolEvent != null)
                    {
                        MoveToolEvent(lastSelectedСellID, selectedSellID, Field.cellsXYCoord[selectedSellID, (int)Cell.x], Field.cellsXYCoord[selectedSellID, (int)Cell.y], ToolMoveType.firstExgange);
                        MoveToolEvent(selectedSellID, lastSelectedСellID, Field.cellsXYCoord[lastSelectedСellID, (int)Cell.x], Field.cellsXYCoord[lastSelectedСellID, (int)Cell.y], ToolMoveType.firstExgange);
                    }
                    selectedToolType = Field.toolsOnField[selectedSellID];
                    lastSelectedToolType = Field.toolsOnField[lastSelectedСellID];
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
                //   isChanging = true;
                int swipeCellToolType;
                int cellToExchangeToolType;

                if (MoveToolEvent != null)
                {
                    MoveToolEvent(cellToExchangeID, swipeCellID, Field.cellsXYCoord[swipeCellID, (int)Cell.x], Field.cellsXYCoord[swipeCellID, (int)Cell.y], ToolMoveType.firstExgange);
                    MoveToolEvent(swipeCellID, cellToExchangeID, Field.cellsXYCoord[cellToExchangeID, (int)Cell.x], Field.cellsXYCoord[cellToExchangeID, (int)Cell.y], ToolMoveType.firstExgange);
                }

                swipeCellToolType = Field.toolsOnField[swipeCellID];
                cellToExchangeToolType = Field.toolsOnField[cellToExchangeID];
            }
        }
    }

    private void CheckMatchesInCell(int id, int lastId, ToolMoveType toolMoveType)
    {

        //    isMatchManipulation = true;
        //   isChanging = false;

        if (toolMoveType == ToolMoveType.secondExgenge)
        {
            //       isMatchManipulation = false;
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
            for (int i = 1; i < height - row; i++)
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
        if (column != 0)
        {
            for (int i = 1; i <= column; i++)
            {
                if (Field.toolsOnField[id - i] == toolType)
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
        if (column != width - 1)
        {
            for (int i = 1; i < width - column; i++)
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
            if(InvisibleTipEvent!=null)
            {
                InvisibleTipEvent(tipCellId);
            }
            if (PlayerMoveEvent != null)
            {
                PlayerMoveEvent();
            }
            haveMatchOnField[id] = (int)Cell.isMatch;
            //         print("haveMatchOnField[" + id + "] = " + haveMatchOnField[id]);
        }
        StartCoroutine(CheckReturnExgangeCoroutine(id, lastId, matchInRowID, matcheInRow, matchInColumnID, matcheInColumn, toolMoveType));

    }

    private IEnumerator CheckReturnExgangeCoroutine(int checkId, int secondId, int[] matchInRowID, int matcheInRow, int[] matchInColumnID, int matcheInColumn, ToolMoveType toolMoveType)
    {
        //       print("CheckReturnExgangeCoroutine");
        yield return null;

        if (haveMatchOnField[checkId] == (int)Cell.isNotMatch && haveMatchOnField[secondId] == (int)Cell.isNotMatch)
        {
            if (toolMoveType == ToolMoveType.firstExgange)
            {
                if (MoveToolEvent != null)
                {
                    MoveToolEvent(checkId, secondId, Field.cellsXYCoord[secondId, (int)Cell.x], Field.cellsXYCoord[secondId, (int)Cell.y], ToolMoveType.secondExgenge);
                }
            }
        }
        else if (haveMatchOnField[checkId] == (int)Cell.isMatch)
        {
            if (matcheInRow > 1)
            {
                MatchManipulation(matchInRowID, checkId, matcheInRow);
            }
            if (matcheInColumn > 1)
            {
                MatchManipulation(matchInColumnID, checkId, matcheInColumn);
            }
        }
        yield return null;
        //     isChanging = false;
        haveMatchOnField[checkId] = (int)Cell.isNotMatch;
    }

    private void MatchManipulation(int[] matchID, int basicId, int matchCount)
    {
        //print("matchCount= " + matchCount);
        //PrintArr(matchID);
        //print("basicId= " + basicId + " Field.toolsOnField[" + basicId + "] = " + Field.toolsOnField[basicId]);
        //    isGathering = true;
        if (GatherToolEvent != null)
        {
            //          print("GatherToolEvent basicId= " + basicId);
            GatherToolEvent(basicId);
        }
        switch (matchCount)
        {
            case 2:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        //          print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        //            print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < matchCount; i++)
                {
                    if (GatherToolEvent != null)
                    {
                        //           print("GatherToolEvent id= " + matchID[i]);
                        GatherToolEvent(matchID[i]);
                    }
                }
                break;
            default:
                break;
        }

        //      isMatchManipulation = false;
    }


    private IEnumerator EmptyCellsManipulationCoroutine(int column)
    {
        //   EmptyCellsManipulationIsRuning = true;
        bool emptyCellExist = false;
        int cellsToMoveNumber = 0;
        int curentId = -1;
        int width = Field.CurentFieldWidth;
        int height = Field.CurentFieldHeight;
        int fieldSize = Field.FieldSize;
        int[] cellsIdToMoveDown = new int[height];
        //     int[] cellsToolsType = new int[height]; // для информации, потом удалить
        yield return null;

        while (true)
        {
            emptyCellExist = false;
            cellsToMoveNumber = 0;
            curentId = -1;

            for (int i = 0; i < height; i++)
            {
                cellsIdToMoveDown[i] = -2;
                //cellsToolsType[i] = -2;
            }

            //string s1 = "id to down ";
            //string s2 = "ToolsType  ";

            if (Field.toolsOnField[column] == (int)ToolType.noTool)
            {
                AddTool(column);

                wasFalling = true;
            }
            yield return null;

            for (int i = 0; i < height; i++)
            {
                curentId = column + i * width;
                if (Field.toolsOnField[curentId] == (int)ToolType.noTool)
                {
                    emptyCellExist = true;
                    break;
                }
                else if (Field.toolsOnField[curentId] != (int)ToolType.noTool)
                {
                    if (!emptyCellExist)
                    {
                        cellsIdToMoveDown[cellsToMoveNumber] = curentId;
                        //cellsToolsType[cellsToMoveNumber] = Field.toolsOnField[curentId];
                        //s1 = s1 + cellsIdToMoveDown[cellsToMoveNumber].ToString() + " ";
                        //s2 = s2 + cellsToolsType[cellsToMoveNumber].ToString() + " ";
                        cellsToMoveNumber++;
                    }
                }
            }
            //print(s1);
            //print(s2);
            //print("cellsToMoveNumber " + cellsToMoveNumber);
            if (emptyCellExist)
            {
                wasFalling = true;
                columnsWithFallingTools[column] = 1;
                fallingToolsExist = true;
                if (cellsToMoveNumber > 0)
                {
                    for (int i = 0; i < cellsToMoveNumber; i++)
                    {
                        if (MoveDownToolEvent != null)
                        {
                            MoveDownToolEvent(cellsIdToMoveDown[i]);
                        }
                    }
                }
            }
            else
            {
                columnsWithFallingTools[column] = 0;
                fallingToolsExist = false;
            }
            // yield return null;
            yield return new WaitForSeconds(Tool.TimeMoveDown);
        }
        //   EmptyCellsManipulationIsRuning = false;
    }





    private void AddTool(int column)
    {
        GameObject tool;
        Vector3 curentToolPosition = Vector3.zero;
        Vector3 unitScale = new Vector3(Field.CurentUnitScale, Field.CurentUnitScale, Field.CurentUnitScale);
        int toolType = Random.Range(1, 6);
        toolType *= 2;
        tool = ToolsPool.toolsReservedListOfStacks[toolType].Pop();
        tool.GetComponent<Tool>().CellID = column;

        curentToolPosition.x = Field.cellsXYCoord[column, (int)Cell.x];
        curentToolPosition.y = Field.cellsXYCoord[column, (int)Cell.y];
        tool.transform.position = curentToolPosition;
        tool.transform.localScale = unitScale;
        ToolsPool.toolsOnFieldListOfStacks[toolType].Push(tool);
        tool.SetActive(true);
        Field.toolsOnField[column] = toolType;
    }


    private IEnumerator CheckMatchesAfterFallAllField()
    {
        yield return null;
        matchesAllField = true;
        int fieldSize = Field.FieldSize;
        for (int i = fieldSize - 1; i >= 0; i--)
        {
            //yield return null;
            CheckMatchesAfterFall(i);
        }
        wasFalling = false;
        matchesAllField = false;
    }
    private void CheckMatchesAfterFallColumn()
    {
        int fieldSize = Field.FieldSize;
        for (int i = fieldSize - 1; i >= 0; i--)
        {
            CheckMatchesAfterFall(i);
        }
    }

    private void CheckMatchesAfterFall(int id)
    {
        if (id < 0)
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
        int[] matchInRowID = new int[height];
        int[] matchInColumnID = new int[width];
        for (int i = 0; i < width; i++)
        {
            matchInColumnID[i] = -1;
        }
        for (int i = 0; i < height; i++)
        {
            matchInRowID[i] = -1;
        }
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
            for (int i = 1; i < height - row; i++)
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
        if (column != 0)
        {
            for (int i = 1; i <= column; i++)
            {
                if (Field.toolsOnField[id - i] == toolType)
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
        if (column != width - 1)
        {
            for (int i = 1; i < width - column; i++)
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

        if (matcheInRow > 1 || matcheInColumn > 1)
        {
            if (GatherToolEvent != null)
            {
                GatherToolEvent(id);
            }
        }

        if (matcheInColumn > 1)
        {
            for (int i = 0; i < matcheInColumn; i++)
            {
                if (GatherToolEvent != null)
                {
                    GatherToolEvent(matchInColumnID[i]);
                }
            }
        }
        if (matcheInRow > 1)
        {
            for (int i = 0; i < matcheInRow; i++)
            {
                if (GatherToolEvent != null)
                {
                    GatherToolEvent(matchInRowID[i]);
                }
            }
        }
    }

}
