using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCell : MonoBehaviour
{
    private Transform cellTransform;
    private Vector3 basicPosition;
    private Vector3 selectedPosition;
    [SerializeField] private int cellID;

    private void OnEnable()
    {
           //MouseController.SelectCellEvent += SelectCell;
        //MouseController.DeselectCellEvent += DeselectCell;

        cellTransform = this.transform;
        basicPosition = cellTransform.position;
        selectedPosition = basicPosition + new Vector3(0, 0, -2);
        StartCoroutine(SetCellID());
    }

    private void OnDisable()
    {

    }


    //private void SelectCell(int selectedCellID, int lastSelectedСellID)
    //{
    //    if (CellID == selectedCellID)
    //    {
    //        cellTransform.position = selectedPosition;
    //        isSelected = true;
    //    }
    //}
    //private void DeselectCell(int id)
    //{
    //    if (CellID == id)
    //    {
    //        cellTransform.position = basicPosition;
    //        isSelected = false;
    //    }
    //}

  
    private IEnumerator SetCellID()
    {
        yield return null;
        for (int i = 0; i < Field.FieldSize; i++)
        {
            if (basicPosition.x == Field.cellsXYCoord[i,(int)Cell.x] && basicPosition.y == Field.cellsXYCoord[i, (int)Cell.y])
            {
                CellID = i;
                yield break;
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
