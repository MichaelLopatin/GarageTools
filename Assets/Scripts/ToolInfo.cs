using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInfo : MonoBehaviour
{
    private int cellID = -1;
  //  private ToolType type;


    private void DeactivateTool(int id)
    {

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
    //private ToolType Type
    //{
    //    get
    //    {
    //        return type;
    //    }
    //    set
    //    {
    //        type = value;
    //    }
    //}
}


