using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInfo : MonoBehaviour
{
    private int cellID = -1;

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


