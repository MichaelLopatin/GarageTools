using System.Collections;
using UnityEngine;

public class GreenCell : MonoBehaviour
{
    private Transform cellTransform;
    private Vector3 basicPosition;
    private Vector3 selectedPosition;
    private int cellID;

    private void OnEnable()
    {
        AnalysisToolsRelativePosition.VisibleTipEvent += VisibleCell;
        AnalysisToolsRelativePosition.InvisibleTipEvent += InvisibleCell;
        cellTransform = this.transform;
        basicPosition = cellTransform.position;
        selectedPosition = basicPosition + new Vector3(0, 0, -2f);
        StartCoroutine(SetCellID());
    }

    private void OnDisable()
    {
        AnalysisToolsRelativePosition.VisibleTipEvent -= VisibleCell;
        AnalysisToolsRelativePosition.InvisibleTipEvent -= InvisibleCell;
    }

    private void VisibleCell(int selectedCellID)
    {
        if (CellID == selectedCellID)
        {
            cellTransform.position = selectedPosition;
        }
    }

    private void InvisibleCell(int selectedCellID)
    {
        if (CellID == selectedCellID)
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
