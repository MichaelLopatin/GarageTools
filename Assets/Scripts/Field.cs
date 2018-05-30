using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Cell
{
    x = 0,
    y = 1,
    isNotExchange = 0,
    isExchange = 1,
    isNotMatch = 0,
    isMatch = 1,
    isNotTip = 0,
    isTip = 1,
    isEmpty = -1,
    isNotEmpty = 0,
    justCreate=-50,
}

public class Field : MonoBehaviour
{
    private enum Row
    {
        first,
        allMiddle,
        last
    }

    [SerializeField] private Camera mainCam;

    private static float curentUnitScale = 0;
    private static int curentFieldWidth = 0;
    private static int curentFieldHeight = 0;
    private static int fieldSize = 0;

    private Vector3 gameFieldCentrePosition;
    private static Vector3 indicatorsCentrePosition;
    private Vector3 firstCellPosition;

    public static float[,] cellsXYCoord;
    public static float[,] startCellsXYCoord;
    public static int[] toolsOnField;
    public static int[] emptyOnField;
    public static int emptyCells=0;

    private void Awake()
    {
        SettingFieldParameters();
    }
    private void OnEnable()
    {

    }

    private void Start()
    {
        SetField(ref CellsPool.blueCellsReservedListOfStacks, ref CellsPool.blueCellsOnFieldListOfStacks, CellColour.blue, GameIndicators.level, firstCellPosition);
        SetField(ref CellsPool.greenCellsReservedListOfStacks, ref CellsPool.greenCellsOnFieldListOfStacks, CellColour.green, GameIndicators.level, firstCellPosition);
        SetField(ref CellsPool.yellowCellsReservedListOfStacks, ref CellsPool.yellowCellsOnFieldListOfStacks, CellColour.yellow, GameIndicators.level, firstCellPosition);
    }

    /*  private void Update()
      {
          count++;
          if(count>600)
          {
              DisableField(ref CellsPool.yellowCellsReservedListOfStacks, ref CellsPool.yellowCellsOnFieldListOfStacks);
          }
          else if (count > 400)
          {
              DisableField(ref CellsPool.greenCellsReservedListOfStacks, ref CellsPool.greenCellsOnFieldListOfStacks);
          }
          else if (count > 200)
          {
                DisableField(ref CellsPool.blueCellsReservedListOfStacks, ref CellsPool.blueCellsOnFieldListOfStacks);

          }
      }*/

    private void SettingFieldParameters()
    {
        SetFieldsCentrePositions(out gameFieldCentrePosition);
        curentFieldWidth = SetFieldWidth(GameIndicators.level);
        curentFieldHeight = SetFieldHeight(GameIndicators.level);
        curentUnitScale = SetUnitScale(GameIndicators.level);
        fieldSize = curentFieldWidth * curentFieldHeight;
        firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
        SetCellsCoordinates(out cellsXYCoord,out startCellsXYCoord, curentFieldWidth, curentFieldHeight, curentUnitScale, firstCellPosition);
        toolsOnField = new int[fieldSize];
        emptyOnField = new int[fieldSize];
    }

    private void SetCellsCoordinates(out float[,] cellsXYCoord, out float[,] startCellsXYCoord, int curentFieldWidth, int curentFieldHeight, float curentUnitScale, Vector3 firstCellPosition)
    {
        Vector3 curentCellCoord = firstCellPosition;

        cellsXYCoord = new float[curentFieldWidth * curentFieldHeight, 2];
        startCellsXYCoord = new float[curentFieldWidth, 2];

        for (int i = 0; i < FieldSize; i++)
        {
            cellsXYCoord[i, (int)Cell.x] = curentCellCoord.x;
            cellsXYCoord[i, (int)Cell.y] = curentCellCoord.y;
            curentCellCoord.x += curentUnitScale;
            if (i % (curentFieldWidth) == curentFieldWidth - 1)
            {
                curentCellCoord.x = firstCellPosition.x;
                curentCellCoord.y -= curentUnitScale;
            }
        }
        for (int i = 0; i < curentFieldWidth; i++)
        {
            startCellsXYCoord[i, (int)Cell.x] = cellsXYCoord[i, (int)Cell.x];
            startCellsXYCoord[i, (int)Cell.y] = cellsXYCoord[i, (int)Cell.y] + curentUnitScale;
        }
    }

    private void SetInitialCellColourByIndex(out int[] cells, int width, int heigth)
    {
        int cellsNumber = width * heigth;
        cells = new int[cellsNumber];
        for (int i = 0; i < cellsNumber; i++)
        {
            cells[i] = (int)CellColour.blue;
        }
    }

    private void DisableField(ref List<Stack<GameObject>> listReservCellsStack, ref List<Stack<GameObject>> listOnFieldCellsStack)
    {
        GameObject cell;
        int cellsQuantity = 0;
        for (int i = 0; i < (int)CellType.quantity; i++)
        {
            cellsQuantity = listOnFieldCellsStack[i].Count;
            for (int j = 0; j < cellsQuantity; j++)
            {
                cell = listOnFieldCellsStack[i].Pop();
                cell.SetActive(false);
                listReservCellsStack[i].Push(cell);
            }
        }
    }

    private void SetField(ref List<Stack<GameObject>> listReservCellsStack, ref List<Stack<GameObject>> listOnFieldCellsStack, CellColour colour, int level, Vector3 firstCellPosition)
    {
        Vector3 curentCellPosition = firstCellPosition;
        switch (colour)
        {
            case CellColour.blue:
                curentCellPosition.z = 3;
                break;
            case CellColour.green:
                curentCellPosition.z = 4;
                break;
            case CellColour.yellow:
                curentCellPosition.z = 5;
                break;
            default:
                curentCellPosition.z = 3;
                break;
        }
        Vector3 unitScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);

        ActivateCellsRow(ref listReservCellsStack, ref listOnFieldCellsStack, Row.first, ref curentCellPosition, unitScale);
        ActivateCellsRow(ref listReservCellsStack, ref listOnFieldCellsStack, Row.allMiddle, ref curentCellPosition, unitScale);
        ActivateCellsRow(ref listReservCellsStack, ref listOnFieldCellsStack, Row.last, ref curentCellPosition, unitScale);
    }

    private void ActivateCellsRow(ref List<Stack<GameObject>> listReservCellsStack, ref List<Stack<GameObject>> listOnFieldCellsStack, Row row, ref Vector3 curentCellPosition, Vector3 unitScale)
    {
        CellType leftCell = CellType.leftTop;
        CellType centreCell = CellType.centreTop;
        CellType rightCell = CellType.rightTop;
        int rowsQuantity = 0;

        switch (row)
        {
            case Row.first:
                rowsQuantity = 1;
                leftCell = CellType.leftTop;
                centreCell = CellType.centreTop;
                rightCell = CellType.rightTop;
                break;
            case Row.allMiddle:
                rowsQuantity = curentFieldHeight - 2;
                leftCell = CellType.leftCentre;
                centreCell = CellType.centreCentre;
                rightCell = CellType.rightCentre;
                break;
            case Row.last:
                rowsQuantity = 1;
                leftCell = CellType.leftBottom;
                centreCell = CellType.centreBottom;
                rightCell = CellType.rightBottom;
                break;
            default:
                break;
        }
        for (int i = 0; i < rowsQuantity; i++)
        {
            ActivateCell(ref listReservCellsStack, ref listOnFieldCellsStack, leftCell, ref curentCellPosition, unitScale, 1);
            ActivateCell(ref listReservCellsStack, ref listOnFieldCellsStack, centreCell, ref curentCellPosition, unitScale, curentFieldWidth - 2);
            ActivateCell(ref listReservCellsStack, ref listOnFieldCellsStack, rightCell, ref curentCellPosition, unitScale, 1);
            curentCellPosition.x = firstCellPosition.x;
            curentCellPosition.y -= curentUnitScale;
        }
    }

    private void ActivateCell(ref List<Stack<GameObject>> listReservCellsStack, ref List<Stack<GameObject>> listOnFieldCellsStack, CellType cellType, ref Vector3 curentCellPosition, Vector3 unitScale, int cellsQuantity)
    {
        GameObject cell;
        for (int i = 0; i < cellsQuantity; i++)
        {
            cell = listReservCellsStack[(int)cellType].Pop();
            cell.transform.position = curentCellPosition;
            cell.transform.localScale = unitScale;
            cell.SetActive(true);
            listOnFieldCellsStack[(int)cellType].Push(cell);
            curentCellPosition.x += curentUnitScale;
        }
    }

    private void SetFieldsCentrePositions(out Vector3 gameFieldCentrePosition)
    {
        Vector3 screenSizeInUnits;
        float xFieldPos;
        float xIndicatorsPos;
        screenSizeInUnits = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        xFieldPos = screenSizeInUnits.x - screenSizeInUnits.y;
        gameFieldCentrePosition = new Vector3(xFieldPos, 0, 0);
        xIndicatorsPos = xFieldPos - screenSizeInUnits.x;
        indicatorsCentrePosition = new Vector3(xIndicatorsPos, 0, 0);
    }

    private Vector3 SetFirstCellPosition(int curentFieldWidth, int curentFieldHeight, float curentUnitScale, Vector3 fieldCentrePosition)
    {
        float x, y;
        x = fieldCentrePosition.x - (curentUnitScale * curentFieldWidth * 0.5f) + curentUnitScale * 0.5f;
        y = fieldCentrePosition.y + (curentUnitScale * curentFieldHeight * 0.5f) - curentUnitScale * 0.5f;
        return new Vector3(x, y, 0);
    }

    private int SetFieldWidth(int level)
    {
        int[] fieldWidth = new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        return fieldWidth[level - 1];
    }

    private int SetFieldHeight(int level)
    {
        int[] fieldHeight = new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        return fieldHeight[level - 1];
    }

    private float SetUnitScale(int level)
    {
        float[] unitScale = new float[] { 1.25f, 1.1f, 1f, 0.9f, 0.8f, 0.75f, 0.65f, 0.6f, 0.6f, 0.55f };
        return unitScale[level - 1];
    }

    public static float CurentUnitScale
    {
        get
        {
            return curentUnitScale;
        }
        private set
        {
            curentUnitScale = value;
        }
    }

    public static int CurentFieldWidth
    {
        get
        {
            return curentFieldWidth;
        }
        private set
        {
            curentFieldWidth = value;
        }
    }

    public static int CurentFieldHeight
    {
        get
        {
            return curentFieldHeight;
        }
        private set
        {
            curentFieldHeight = value;
        }
    }

    public static int FieldSize
    {
        get
        {
            return fieldSize;
        }
        private set
        {
            fieldSize = value;
        }
    }

    public static Vector3 IndicatorsCentrePosition
    {
        get
        {
            return indicatorsCentrePosition;
        }
       private set
        {
            indicatorsCentrePosition = value;
        }
    }
}
