using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowCell : MonoBehaviour
{
    private enum CellInfo
    {
        id = 0,
        x,
        y
    }

    private Transform cellTransform;
    private Vector3 basicPosition;
    private Vector3 selectedPosition;
   [SerializeField] private int cellID;
   private float[,,] cellsXYCoordinates;
    private bool isSelected;

    private void OnEnable()
    {
        Field.ChangeCellsCoordinatesEvent += SetCellsXYCoordinates;
        MouseController.SelectCellEvent += SelectCell;
        MouseController.DeselectCellEvent += DeselectCell;

        cellTransform = this.transform;
        basicPosition = cellTransform.position;
        selectedPosition = basicPosition + new Vector3(0, 0, -4);
        isSelected = false;
        StartCoroutine(SetCellID());

    }

    private void OnDisable()
    {
        Field.ChangeCellsCoordinatesEvent -= SetCellsXYCoordinates;
        MouseController.SelectCellEvent -= SelectCell;
        MouseController.DeselectCellEvent -= DeselectCell;
    }


    private void SelectCell(int id)
    {
        if (CellID == id)
        {
                cellTransform.position = selectedPosition;
                isSelected = true;
        }
    }
    private void DeselectCell(int id)
    {
        if (CellID == id)
        {
            cellTransform.position = basicPosition;
            isSelected = false;
        }
    }

    //private void SelectOrDeselectCell(int id)
    //{
    //    if(CellID==id)
    //    {
    //        if(isSelected)
    //        {
    //            cellTransform.position = basicPosition;
    //            isSelected = false;
    //        }
    //        else
    //        {
    //            cellTransform.position = selectedPosition;
    //            isSelected = true;
    //        }
    //    }
    //}

    private void SetCellsXYCoordinates(float[,,] coordinates)
    {
        cellsXYCoordinates = (float[,,])coordinates.Clone();
    }

    private IEnumerator SetCellID()
    {
        yield return null;
        int rowLength = cellsXYCoordinates.GetLength(0);
        int columnLength = cellsXYCoordinates.GetLength(1);
        for(int i=0;i< rowLength;i++)
        {
            for(int j=0;j<columnLength;j++)
            {
                if (basicPosition.x == cellsXYCoordinates[i, j, (int)CellInfo.x] && basicPosition.y == cellsXYCoordinates[i, j, (int)CellInfo.y])
                {
                    CellID = (int)cellsXYCoordinates[i, j, (int)CellInfo.id];
                    yield break;
                }
            }
        }
    }

    public int CellID
    {
        get
        {
            return cellID;
        }
        set
        {
            cellID = value;
        }
    }

}
