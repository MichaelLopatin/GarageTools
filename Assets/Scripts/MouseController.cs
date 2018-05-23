using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Vector3 mousePosInWorld;
//    private Vector3 mousePosInScreen;
    [SerializeField] private Camera camera;

    private float curentUnitScale;
    private int curentFieldWidth = 0;
    private int curentFieldHeight = 0;
    //private int centreWidthtIndex = 0;
    //private int centreHeightIndex = 0;


    private float percentageOfUnitForSwipe = 0.3f;
 private float swipeLaunchDistance = 1f;
    private float deltaX;
    private float deltaY;
    private bool isMouseButtonDown=false;
    private bool isSwiping=false;

    private int selectedСellID;

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
        if (isMouseButtonDown && !isSwiping)
        {
            DetermineSwipe();
        }

        if (Input.GetMouseButtonDown(1))
        {
            PrintField(cellsXYCoordinates, curentFieldWidth, curentFieldHeight, 2);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!isSwiping)
            {
           mousePosInWorld = camera.ScreenToWorldPoint(Input.mousePosition);
            selectedСellID = SearchCellID(mousePosInWorld);
            isMouseButtonDown = true;
                // событие выделить клетку с ID selectedСellID
            }



        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonDown = false;
        }
    }


    private void DetermineSwipe()
    {
        deltaX = mousePosInWorld.x - camera.ScreenToWorldPoint(Input.mousePosition).x;
        deltaY = mousePosInWorld.y - camera.ScreenToWorldPoint(Input.mousePosition).y;
        if (Mathf.Abs(deltaX) > swipeLaunchDistance || Mathf.Abs(deltaY) > swipeLaunchDistance)
        {
            isSwiping = true;
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                if (deltaX < 0)
                {
                    if (selectedСellID % curentFieldWidth != 0)
                    {
                        //новое положение  ID-1
                    }
                }
                else
                {
                    if (selectedСellID % curentFieldWidth != curentFieldWidth-1)
                    {
                    //новое положение  ID+1
                    }

                }
            }
            else
            {
                if (deltaY > 0)
                {
                    if (selectedСellID > curentFieldWidth - 1)
                    {
                        //новое положение  ID-curentFieldWidth
                    }
                }
                else
                {
                    if (selectedСellID < (curentFieldWidth * (curentFieldHeight - 1) - 1))
                    {
                        //новое положение  ID+curentFieldWidth
                    }
                }
            }
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
        swipeLaunchDistance = percentageOfUnitForSwipe * curentUnitScale;
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
