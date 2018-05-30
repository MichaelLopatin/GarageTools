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
    [SerializeField] private Camera camera;

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
//    private int selectedCellsNumber = 0;

    private void OnEnable()
    {
        Tool.DeselectCellEvent += CleanSelectedFlags;

        StartCoroutine(SetBorders());
        swipeLaunchDistance = percentageOfUnitForSwipe * Field.CurentUnitScale;
    }
    private void OnDisable()
    {
        Tool.DeselectCellEvent -= CleanSelectedFlags;
    }


    private void Update()
    {

        if (isMouseButtonDown && !isSwiping)
        {
            if (selectedСellID >= 0)
            {
                DetermineSwipe(selectedСellID);
            }
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
        if (Input.GetMouseButtonDown(1))
        {
            PrintField(Field.toolsOnField, Field.CurentFieldWidth, Field.CurentFieldHeight);
        }
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
                    if (swipeCellID % Field.CurentFieldWidth != 0)
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
                    if (swipeCellID % Field.CurentFieldWidth != Field.CurentFieldWidth - 1)
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

                    if (swipeCellID > Field.CurentFieldWidth - 1)
                    {
                        swipeDirection = SwipeDirection.up;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                            //новое положение  ID-Field.CurentFieldWidth
                            //событие другую клетку поставить на место этой клетки
                            // Event(newID, ID);
                        }

                    }
                }
                else
                {
                    if (swipeCellID < (Field.CurentFieldWidth * (Field.CurentFieldHeight - 1) - 1))
                    {
                        swipeDirection = SwipeDirection.down;
                        if (SwipeEvent != null)
                        {
                            SwipeEvent(swipeCellID, swipeDirection);
                            //новое положение  ID+Field.CurentFieldWidth
                            //событие другую клетку поставить на место этой клетки
                            // Event(newID, ID);
                        }
                    }
                }
            }
        }
    }

    private int SearchCellID(Vector3 mousePosInWorld)
    {
        float[] xCoord = new float[Field.CurentFieldWidth];
        float[] yCoord = new float[Field.CurentFieldHeight];
        for (int i = 0; i < Field.CurentFieldWidth; i++)
        {
            xCoord[i] = Field.cellsXYCoord[i, (int)Cell.x];
        }
        for (int i = 0; i < Field.CurentFieldHeight; i++)
        {
            yCoord[i] = Field.cellsXYCoord[(Field.CurentFieldHeight - i - 1) * Field.CurentFieldHeight, (int)Cell.y];
        }
        int row = Field.CurentFieldHeight - 1 - BinarySearch(yCoord, 0, (int)(Field.CurentFieldWidth * 0.5f), Field.CurentFieldWidth - 1, mousePosInWorld.y);
        int column = BinarySearch(xCoord, 0, (int)(Field.CurentFieldHeight * 0.5f), Field.CurentFieldHeight - 1, mousePosInWorld.x);
        return (row * Field.CurentFieldWidth + column);
    }

    private int BinarySearch(float[] arr, int left, int middle, int right, float coord)
    {

        if ((coord >= arr[middle] - Field.CurentUnitScale * 0.5f) &&
            (coord <= arr[middle] + Field.CurentUnitScale * 0.5f))
        {
            return middle;
        }
        else if (coord < arr[left] + Field.CurentUnitScale * 0.5f)
        {
            return left;
        }
        else if (coord > arr[right] - Field.CurentUnitScale * 0.5f)
        {
            return right;
        }
        else if (coord < arr[middle])
        {
            return BinarySearch(arr, left, left + (int)((middle - left) * 0.5f), middle, coord);
        }
        else
        {
            return BinarySearch(arr, middle, middle + (int)((right - middle) * 0.5f), right, coord);
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


    private IEnumerator SetBorders()
    {
        yield return null;
        borderTop = Field.cellsXYCoord[0, (int)Cell.y] + Field.CurentUnitScale * 0.5f;
        borderRight = Field.cellsXYCoord[Field.FieldSize - 1, (int)Cell.x] + Field.CurentUnitScale * 0.5f;
        borderBottom = Field.cellsXYCoord[Field.FieldSize - 1, (int)Cell.y] - Field.CurentUnitScale * 0.5f;
        borderLeft = Field.cellsXYCoord[0, (int)Cell.x] - Field.CurentUnitScale * 0.5f;
    }
}
