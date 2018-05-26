using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection
{
    up,
    right,
    down,
    left
}

public class MouseController : MonoBehaviour
{
    public delegate void SelectCell(int selectedSellID, int lastSelectedСellID);
    public static event SelectCell SelectCellEvent;
    public delegate void DeselectCell(int cellID);
    public static event DeselectCell DeselectCellEvent;
    public delegate void Swipe(int cellID, SwipeDirection swipeDirection);
    public static event Swipe SwipeEvent;

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
    [SerializeField] private bool isMouseButtonDown = false;
    [SerializeField] private bool isSwiping = false;
    [SerializeField] private SwipeDirection swipeDirection;

    [SerializeField] private int selectedСellID = -1;
    [SerializeField] private int lastSelectedСellID = -1;
    [SerializeField] private bool haveSelectedCell = false;
    [SerializeField] private bool justSelectedCell = false;

    private float borderTop;
    private float borderRight;
    private float borderBottom;
    private float borderLeft;


    private float[,,] cellsXYCoordinates;

    private enum Cell
    {
        id = 0,
        toolsType = 1,
        x = 1,
        y = 2
    }

    private void OnEnable()
    {
        Field.ChangeCellsCoordinatesEvent += SetCellsXYCoordinates;
        Field.ChangeFieldParametersEvent += SetFieldParameters;
        Tool.DeselectCellEvent += CleanSelectedFlags;

        StartCoroutine(SetBorders());
    }
    private void OnDisable()
    {
        Field.ChangeCellsCoordinatesEvent -= SetCellsXYCoordinates;
        Field.ChangeFieldParametersEvent -= SetFieldParameters;
        Tool.DeselectCellEvent -= CleanSelectedFlags;
    }


    private void Update()
    {

        if (isMouseButtonDown && !isSwiping)
        {
            DetermineSwipe(selectedСellID);
        }
        if (isSwiping && (haveSelectedCell || justSelectedCell))
        {
            if (DeselectCellEvent != null)
            {
                DeselectCellEvent(selectedСellID);
                selectedСellID = -1;
                lastSelectedСellID = -1;
                haveSelectedCell = false;
                justSelectedCell = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePosInWorld = camera.ScreenToWorldPoint(Input.mousePosition);
            if ((mousePosInWorld.x >= borderLeft && mousePosInWorld.x <= borderRight) &&
    (mousePosInWorld.y >= borderBottom && mousePosInWorld.y <= borderTop))
            {

                if (!isSwiping)
                {
                    lastSelectedСellID = selectedСellID;
                    selectedСellID = SearchCellID(mousePosInWorld);
                    isMouseButtonDown = true;
                    if (haveSelectedCell && lastSelectedСellID != selectedСellID)
                    {
                        if (DeselectCellEvent != null)
                        {
                            haveSelectedCell = false;
                            DeselectCellEvent(lastSelectedСellID);
                        }
                    }
                    if (!haveSelectedCell)
                    {
                        if (SelectCellEvent != null)
                        {
                            justSelectedCell = true;
                            SelectCellEvent(selectedСellID, lastSelectedСellID);
                        }
                    }
                }
            }
            else
            {
                if (DeselectCellEvent != null)
                {
                    DeselectCellEvent(selectedСellID);
                }
                selectedСellID = -1;
                lastSelectedСellID = -1;
                haveSelectedCell = false;
                justSelectedCell = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonDown = false;
            if (justSelectedCell)
            {
                justSelectedCell = false;
                haveSelectedCell = true;
            }
            else if (haveSelectedCell)
            {
                haveSelectedCell = false;
                if (DeselectCellEvent != null)
                {
                    DeselectCellEvent(selectedСellID);
                }
            }
            if (isSwiping)
            {
                isSwiping = false;
            }

        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    PrintField(cellsXYCoordinates, curentFieldWidth, curentFieldHeight, 2);
        //}
    }

    private void CleanSelectedFlags(int id)
    {
        justSelectedCell = false;
        haveSelectedCell = false;
        selectedСellID = -1;
    }
    private void DetermineSwipe(int swipeCellID)
    {
        deltaX = camera.ScreenToWorldPoint(Input.mousePosition).x - mousePosInWorld.x;
        deltaY = camera.ScreenToWorldPoint(Input.mousePosition).y - mousePosInWorld.y;
        if (Mathf.Abs(deltaX) > swipeLaunchDistance || Mathf.Abs(deltaY) > swipeLaunchDistance)
        {
            isSwiping = true;
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                if (deltaX < 0)
                {
                    if (swipeCellID % curentFieldWidth != 0)
                    {
                        swipeDirection = SwipeDirection.left;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                            //новое положение newID = ID-1
                            //событие другую клетку поставить на место этой клетки
                            // Event(newID, ID);
                        }
                    }
                }
                else
                {
                    if (swipeCellID % curentFieldWidth != curentFieldWidth - 1)
                    {
                        swipeDirection = SwipeDirection.right;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                            //новое положение  ID+1
                            //событие другую клетку поставить на место этой клетки
                            // Event(newID, ID);
                        }
                    }
                }
            }
            else
            {
                if (deltaY > 0)
                {
                    
                    if (swipeCellID > curentFieldWidth - 1)
                    {
                        swipeDirection = SwipeDirection.up;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                        //новое положение  ID-curentFieldWidth
                        //событие другую клетку поставить на место этой клетки
                        // Event(newID, ID);
                        }

                    }
                }
                else
                {
                    if (swipeCellID < (curentFieldWidth * (curentFieldHeight - 1) - 1))
                    {
                        swipeDirection = SwipeDirection.down;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                        //новое положение  ID+curentFieldWidth
                        //событие другую клетку поставить на место этой клетки
                        // Event(newID, ID);
                        }
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

        return (int)cellsXYCoordinates[row, column, (int)Cell.id];
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


    private IEnumerator SetBorders()
    {
        yield return null;
        borderTop = cellsXYCoordinates[0, 0, (int)Cell.y] + curentUnitScale * 0.5f;
        borderRight = cellsXYCoordinates[curentFieldHeight - 1, curentFieldWidth - 1, (int)Cell.x] + curentUnitScale * 0.5f;
        borderBottom = cellsXYCoordinates[curentFieldHeight - 1, curentFieldWidth - 1, (int)Cell.y] - curentUnitScale * 0.5f;
        borderLeft = cellsXYCoordinates[0, 0, (int)Cell.x] - curentUnitScale * 0.5f;
    }
}
