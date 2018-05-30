using System.Collections;
using UnityEngine;

public class YellowCell : MonoBehaviour
{
    private Transform cellTransform;
    private Vector3 basicPosition;
    private Vector3 selectedPosition;
    private int cellID;

    private void OnEnable()
    {
        MouseController.SelectCellEvent += SelectCell;
        MouseController.DeselectCellEvent += DeselectCell;
        Tool.DeselectCellEvent += DeselectCell;

        cellTransform = this.transform;
        basicPosition = cellTransform.position;
        selectedPosition = basicPosition + new Vector3(0, 0, -4);
        StartCoroutine(SetCellID());
    }

    private void OnDisable()
    {
        MouseController.SelectCellEvent -= SelectCell;
        MouseController.DeselectCellEvent -= DeselectCell;
        Tool.DeselectCellEvent -= DeselectCell;
    }


    private void SelectCell(int selectedCellID, int lastSelectedСellID)
    {
        if (CellID == selectedCellID)
        {
            cellTransform.position = selectedPosition;
        }
    }

    private void DeselectCell(int id)
    {
        if (CellID == id)
        {
            cellTransform.position = basicPosition;
        }
    }


    private IEnumerator SetCellID()
    {
        yield return null;
        for (int i = 0; i < Field.FieldSize; i++)
        {
            if (basicPosition.x == Field.cellsXYCoord[i, (int)Cell.x] && basicPosition.y == Field.cellsXYCoord[i, (int)Cell.y])
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
