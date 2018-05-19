using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{

    private enum CellType
    {
        centreBottom,
        centreCentre,
        centreTop,
        leftBottom,
        leftCentre,
        leftTop,
        rightBottom,
        rightCentre,
        rightTop,
        quantity
    }

    public enum CellColour
    {
        blue = 0,
        green,
        yellow,
        quantity
    }

    [SerializeField] private GameObject[] centreBottomCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] centreCentreCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] centreTopCells = new GameObject[(int)CellColour.quantity];

    [SerializeField] private GameObject[] leftBottomCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] leftCentreCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] leftTopCells = new GameObject[(int)CellColour.quantity];

    [SerializeField] private GameObject[] rightBottomCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] rightCentreCells = new GameObject[(int)CellColour.quantity];
    [SerializeField] private GameObject[] rightTopCells = new GameObject[(int)CellColour.quantity];

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
        SetFieldsCentrePositions(out gameFieldCentrePosition, out indicatorsCentrePosition);
        curentFieldWidth = SetFieldWidth(level);
        curentFieldHeight = SetFieldHeight(level);
        curentUnitScale = SetUnitScale(level);

        firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
        SetField(level);
    }

    private void Update()
    {
        if (lastLevel != level)
        {
            lastLevel = level;
            curentFieldWidth = SetFieldWidth(level);
            curentFieldHeight = SetFieldHeight(level);
            curentUnitScale = SetUnitScale(level);

            firstCellPosition = SetFirstCellPosition(curentFieldWidth, curentFieldHeight, curentUnitScale, gameFieldCentrePosition);
            SetField(level);
        }
    }

    private void SetField(int level)
    {
        GameObject cell;
        for (int j = 0; j < curentFieldHeight; j++)
        {
            for (int i = 0; i < curentFieldWidth; i++)
            {
                cell = Instantiate(centreCentreCells[1], new Vector3(firstCellPosition.x + i * curentUnitScale, firstCellPosition.y - j * curentUnitScale, 2 - level), transform.rotation);
                cell.transform.localScale = new Vector3(curentUnitScale, curentUnitScale, curentUnitScale);
            }
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
