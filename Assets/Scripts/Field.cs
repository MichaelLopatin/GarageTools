using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private enum Row
    {
        first,
        allMiddle,
        last
    }

    public int level = 1;
    private int lastLevel = 1;

    private int maxLevel = 10;

    [SerializeField] private Camera mainCam;

    private float curentUnitScale;
    private int curentFieldWidth;
    private int curentFieldHeight;
    private Vector3 gameFieldCentrePosition;
    private Vector3 indicatorsCentrePosition;

    private Vector3 firstCellPosition;

    private void Awake()
    {
          SettingFieldParameters();
    }

    private void Start()
    {
        SetField(ref CellsPool.blueDisableCellsListOfStacks, ref CellsPool.blueEnableCellsListOfStacks, CellColour.blue, level, firstCellPosition);
        SetField(ref CellsPool.greenDisableCellsListOfStacks, ref CellsPool.greenEnableCellsListOfStacks, CellColour.green, level, firstCellPosition);
        SetField(ref CellsPool.yellowDisableCellsListOfStacks, ref CellsPool.yellowEnableCellsListOfStacks, CellColour.yellow, level, firstCellPosition);
    }
    private void Update()
    {
        //if (lastLevel != level)
        //{
        //    lastLevel = level;
        //    curentFieldWidth = SetFieldWidth(level);
        //    curentFieldHeight = SetFieldHeight(level);
        //    curentUnitScale = SetUnitScale(level);

        //    firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
        //    SetField(ref CellsPool.blueDisableCellsListOfStacks, CellColour.blue, level, firstCellPosition);
        //}
    }

    private void SettingFieldParameters()
    {
        SetFieldsCentrePositions(out gameFieldCentrePosition, out indicatorsCentrePosition);
        curentFieldWidth = SetFieldWidth(level);
        curentFieldHeight = SetFieldHeight(level);
        curentUnitScale = SetUnitScale(level);
        firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
    }

    private void DisableField()
    {

    }

    private void SetField(ref List<Stack<GameObject>> listCellsStack, ref List<Stack<GameObject>> listEnableCellsStack, CellColour colour, int level, Vector3 firstCellPosition)
    {
        Vector3 curentCellPosition = firstCellPosition;
        switch (colour)
        {
            case CellColour.blue:
                curentCellPosition.z = 5;
                break;
            case CellColour.green:
                curentCellPosition.z = 4;
                break;
            case CellColour.yellow:
                curentCellPosition.z = 3;
                break;
            default:
                curentCellPosition.z = 5;
                break;
        }

        Vector3 unitScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);

        ActivateCellsRow(ref listCellsStack, ref listEnableCellsStack, Row.first, ref curentCellPosition, unitScale);
        ActivateCellsRow(ref listCellsStack, ref listEnableCellsStack, Row.allMiddle, ref curentCellPosition, unitScale);
        ActivateCellsRow(ref listCellsStack, ref listEnableCellsStack, Row.last, ref curentCellPosition, unitScale);
    }

    private void ActivateCellsRow(ref List<Stack<GameObject>> listCellsStack, ref List<Stack<GameObject>> listEnableCellsStack, Row row, ref Vector3 curentCellPosition, Vector3 unitScale)
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
            ActivateCell(ref listCellsStack, ref listEnableCellsStack, leftCell, ref curentCellPosition, unitScale, 1);
            ActivateCell(ref listCellsStack, ref listEnableCellsStack, centreCell, ref curentCellPosition, unitScale, curentFieldWidth - 2);
            ActivateCell(ref listCellsStack, ref listEnableCellsStack, rightCell, ref curentCellPosition, unitScale, 1);
    curentCellPosition.x = firstCellPosition.x;
        curentCellPosition.y -= curentUnitScale;
    }
        
    }

    private void ActivateCell(ref List<Stack<GameObject>> listCellsStack, ref List<Stack<GameObject>> listEnableCellsStack, CellType cellType, ref Vector3 curentCellPosition, Vector3 unitScale, int cellsQuantity)
    {
        GameObject cell;
        for (int i = 0; i < cellsQuantity; i++)
        {
            cell = listCellsStack[(int)cellType].Pop();
            cell.transform.position = curentCellPosition;
            cell.transform.localScale = unitScale;
            cell.SetActive(true);
            listEnableCellsStack[(int)cellType].Push(cell);
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
        print(new Vector3(x, y, 0));
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
