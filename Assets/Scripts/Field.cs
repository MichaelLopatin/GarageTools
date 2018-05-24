using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public delegate void ChangeCellsCoordinates(float[,,] cellsXYCoordinates);
    public static event ChangeCellsCoordinates ChangeCellsCoordinatesEvent;
    public delegate void ChangeFieldParameters(int curentFieldWidth, int curentFieldHeight, float curentUnitScale);
    public static event ChangeFieldParameters ChangeFieldParametersEvent;

    private enum Row
    {
        first,
        allMiddle,
        last
    }

    private enum CellInfo
    {
        id = 0,
        x,
        y
    }

    private enum CellColour
    {
        blue = 0,
        green,
        yellow
    }

    [SerializeField] private Camera mainCam;

    private int[] cellColourByIndex;

    public int level = 1;

    private float curentUnitScale;
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;

    private Vector3 gameFieldCentrePosition;
    private Vector3 indicatorsCentrePosition;
    private Vector3 firstCellPosition;

    private float[,,] cellsXYCoordinates;

    //   int count = 0;

    private void Awake()
    {
        SettingFieldParameters();
    }
    private void OnEnable()
    {
        
    }

    private void Start()
    {
        SetField(ref CellsPool.blueCellsReservedListOfStacks, ref CellsPool.blueCellsOnFieldListOfStacks, CellColour.blue, level, firstCellPosition);
        SetField(ref CellsPool.greenCellsReservedListOfStacks, ref CellsPool.greenCellsOnFieldListOfStacks, CellColour.green, level, firstCellPosition);
        SetField(ref CellsPool.yellowCellsReservedListOfStacks, ref CellsPool.yellowCellsOnFieldListOfStacks, CellColour.yellow, level, firstCellPosition);
        if (ChangeCellsCoordinatesEvent != null)
        {
            ChangeCellsCoordinatesEvent(cellsXYCoordinates);
        }
        if (ChangeFieldParametersEvent != null)
        {
            ChangeFieldParametersEvent(curentFieldWidth, curentFieldHeight, curentUnitScale);
        }
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
        SetFieldsCentrePositions(out gameFieldCentrePosition, out indicatorsCentrePosition);
        curentFieldWidth = SetFieldWidth(level);
        curentFieldHeight = SetFieldHeight(level);
        curentUnitScale = SetUnitScale(level);
        firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
        SetCellsCoordinates(out cellsXYCoordinates, curentFieldWidth, curentFieldHeight, curentUnitScale, firstCellPosition);
    }

    private void SetCellsCoordinates(out float[,,] cellsXYCoordinates, int curentFieldWidth, int curentFieldHeight, float curentUnitScale, Vector3 firstCellPosition)
    {
        Vector3 curentCellCoord = firstCellPosition;
        cellsXYCoordinates = new float[curentFieldWidth, curentFieldHeight, 3];
        for (int i = 0, k = 0; i < curentFieldWidth; i++)
        {
            for (int j = 0; j < curentFieldHeight; j++, k++)
            {
                cellsXYCoordinates[i, j, (int)CellInfo.id] = k;
                cellsXYCoordinates[i, j, (int)CellInfo.x] = curentCellCoord.x;
                cellsXYCoordinates[i, j, (int)CellInfo.y] = curentCellCoord.y;
                curentCellCoord.x += curentUnitScale;
             }
            curentCellCoord.x = firstCellPosition.x;
            curentCellCoord.y -= curentUnitScale;
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

    private void SetFieldsCentrePositions(out Vector3 gameFieldCentrePosition, out Vector3 indicatorsCentrePosition)
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



}
