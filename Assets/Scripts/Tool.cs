using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public delegate void ExchangeMark(int cellID, bool isExchange);
    public static event ExchangeMark ExchangeMarkEvent;
    public delegate void DeselectCell(int cellID);
    public static event DeselectCell DeselectCellEvent;
    public delegate void EndToolMovement(int cellID, int lastCellID, MoveCount moveCount);
    public static event EndToolMovement EndToolMovementEvent;

    private int cellID = -1;
    private float timeForExchange = 0.25f;
    private float timeMoveToTheBox = 0.5f;
    private Transform toolTransform;
    private bool isExchange=false;

    private void Awake()
    {
        toolTransform = this.transform;
    }
    private void OnEnable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent += MoveTool;
        AnalysisToolsRelativePosition.GatherToolEvent += DeactivateTool;
    }

    private void OnDisable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent -= MoveTool;
       AnalysisToolsRelativePosition.GatherToolEvent -= DeactivateTool;
    }

    private void MoveTool(int id, int newId,float newX, float newY, MoveCount moveCount)
    {
        if (cellID == id && !isExchange)
        {
            StartCoroutine(MoveCoroutine(id, newId,newX, newY, moveCount));
        }
    }

    private IEnumerator MoveCoroutine(int id, int newId, float newX, float newY, MoveCount moveCount)
    {
        isExchange = true;
        if(ExchangeMarkEvent!=null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        int toolType = Field.toolsOnField[id];
        Field.toolsOnField[id] = -1;
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
        Field.toolsOnField[newId] = toolType;
        cellID = newId;
        if (ExchangeMarkEvent != null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        if (EndToolMovementEvent != null)
        {
            EndToolMovementEvent(newId, id, moveCount);
        }

    }

    private void DeactivateTool(int id)
    {
        if(cellID==id)
        {
        print("Вызванный DeactivateTool id= " + id);
        ToolsPool.toolsOnFieldListOfStacks[Field.toolsOnField[id]].Pop();
        ToolsPool.toolsReservedListOfStacks[Field.toolsOnField[id]].Push(this.gameObject);
            Field.toolsOnField[cellID] = (int)ToolType.isEmpty;
            StartCoroutine(MoveToTheBoxCoroutine());
        }

    }
private IEnumerator MoveToTheBoxCoroutine()
    {
        float moveTime = 0f;
        Vector3 curentPosition = toolTransform.position;
        float frequency = 1 / timeMoveToTheBox;
        float startScale = Field.CurentUnitScale*1.2f;
        float targetScale = Field.CurentUnitScale * 0.4f;


        while (moveTime < timeMoveToTheBox)
        {
            moveTime += Time.deltaTime;
            if (moveTime > timeMoveToTheBox)
            {
                moveTime = timeMoveToTheBox;
            }

            toolTransform.localScale = Vector3.one* Mathf.Lerp(startScale, targetScale, moveTime * frequency);
            toolTransform.position = Vector3.Lerp(curentPosition, GameIndicators.boxPointPosition, moveTime * frequency);
            yield return null;
        }
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

