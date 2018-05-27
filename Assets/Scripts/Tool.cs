using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public delegate void ExchangeMark(int cellID, bool isExchange);
    public static event ExchangeMark ExchangeMarkEvent;
    public delegate void DeselectCell(int cellID);
    public static event DeselectCell DeselectCellEvent;

    private int cellID = -1;
    private float timeForExchange = 0.25f;
    private Transform toolTransform;
    private bool isExchange=false;

    private void Awake()
    {
        toolTransform = this.transform;
    }
    private void OnEnable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent += MoveTool;
        AnalysisToolsRelativePosition.DestroyToolEvent += DeactivateTool;
    }

    private void OnDisable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent -= MoveTool;
        AnalysisToolsRelativePosition.DestroyToolEvent -= DeactivateTool;
    }

    private void MoveTool(int id, int newId,float newX, float newY)
    {
        if (cellID == id && !isExchange)
        {
            StartCoroutine(MoveCoroutine(id, newId,newX, newY));
        }
    }

    private IEnumerator MoveCoroutine(int id, int newId, float newX, float newY)
    {
        isExchange = true;
        if(ExchangeMarkEvent!=null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        Vector3 curentPosition = toolTransform.position;
        Vector3 targetPosition = new Vector3(newX, newY, curentPosition.z);
        float frequency = 1 / timeForExchange;
        float moveTime = 0f;
        yield return null;
        if (DeselectCellEvent != null)
        {
            DeselectCellEvent(id);
        }
        while (moveTime < timeForExchange)
        {
            moveTime += Time.deltaTime;
            if (moveTime > timeForExchange)
            {
                moveTime = timeForExchange;
            }
            
            toolTransform.position=Vector3.Lerp(curentPosition, targetPosition, moveTime* frequency);
            yield return null;
        }

        isExchange = false;
        if (ExchangeMarkEvent != null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        cellID = newId;
    }

    private void DeactivateTool(int id, int toolType)
    {
        ToolsPool.toolsOnFieldListOfStacks[toolType].Pop();
        ToolsPool.toolsReservedListOfStacks[toolType].Push(this.gameObject);
        this.gameObject.SetActive(false);
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

