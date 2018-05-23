using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Vector3 mousePosInWorld;
    [SerializeField] private Camera camera;

    private float curentUnitScale;
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;
    //private int centreWidthtIndex = 0;
    //private int centreHeightIndex = 0;

    private float[,,] cellsXYCoordinates;

    private enum Cell
    {
        id = 0,
        toolsType = 1,
        x = 1,
        y = 2
    }

    private void Awake()
    {

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

    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PrintField(cellsXYCoordinates, curentFieldWidth, curentFieldHeight, 2);
        }
        if (Input.GetMouseButtonDown(0))
        {
            mousePosInWorld = camera.ScreenToWorldPoint(Input.mousePosition);

            print("cell ID = " + SearchCellID(mousePosInWorld));
        }
    }

    private void SetCellsXYCoordinates(float[,,] coordinates)
    {
        cellsXYCoordinates = (float[,,])coordinates.Clone();
    }

    private void SetFieldParameters(int width, int height, float scale)
    {
        curentFieldWidth = width;
        curentFieldHeight = height;
        curentUnitScale = scale;
    }

    private int SearchCellID(Vector3 mousePosInWorld)
    {
        int column;
        int row;

        if (mousePosInWorld.x <= cellsXYCoordinates[0, 0, (int)Cell.x] + curentUnitScale * 0.5f)
        {
            column = 0;
        }
        else if (mousePosInWorld.x >= cellsXYCoordinates[0, curentFieldWidth - 1, (int)Cell.x] - curentUnitScale * 0.5f)
        {
            column = curentFieldWidth - 1;
        }
        else
        {
            column = BinarySearchX(0, (int)(curentFieldWidth * 0.5f) + 1, curentFieldWidth - 1, mousePosInWorld.x);
        }

        if (mousePosInWorld.y >= (cellsXYCoordinates[0, 0, (int)Cell.y] - curentUnitScale * 0.5f))
        {
            row = 0;
        }
        else if (mousePosInWorld.y <= cellsXYCoordinates[curentFieldHeight - 1, 0, (int)Cell.y] + curentUnitScale * 0.5f)
        {
            row = curentFieldHeight - 1;
        }
        else
        {
            row = BinarySearchY(0, (int)(curentFieldHeight * 0.5f) + 1, curentFieldHeight - 1, mousePosInWorld.y);
        }

        return (int)cellsXYCoordinates[row, column,(int)Cell.id];
    }

    private int BinarySearchX(int left, int middle, int right, float number)
    {
        if ((number >= cellsXYCoordinates[0, middle, (int)Cell.x] - curentUnitScale * 0.5f) &&
            (number <= cellsXYCoordinates[0, middle, (int)Cell.x] + curentUnitScale * 0.5f))
        {
            return middle;
        }

        else if (number < cellsXYCoordinates[0, middle, (int)Cell.x])
        {
            return BinarySearchX(left, left + (int)((middle - left) * 0.5f), middle, number);
        }
        else
        {
            return BinarySearchX(middle, middle + (int)((right - middle) * 0.5f), right, number);
        }
    }


    private int BinarySearchY(int left, int middle, int right, float number)
    {
        if ((number >= cellsXYCoordinates[middle, 0, (int)Cell.y] - curentUnitScale * 0.5f) &&
                (number <= cellsXYCoordinates[middle, 0, (int)Cell.y] + curentUnitScale * 0.5f))
        {
            return middle;
        }
        else if (number > cellsXYCoordinates[middle, 0, (int)Cell.y])
        {
            return BinarySearchY(left, left + (int)((middle - left) * 0.5f), middle, number);
        }
        else
        {
            return BinarySearchY(middle, middle + (int)((right - middle) * 0.5f), right, number);
        }
    }

    private void PrintField(float[,,] field, int width, int height, int k)
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
}
