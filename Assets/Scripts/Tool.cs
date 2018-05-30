using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public delegate void ExchangeMark(int cellID, bool isExchange);
    public static event ExchangeMark ExchangeMarkEvent;
    public delegate void DeselectCell(int cellID);
    public static event DeselectCell DeselectCellEvent;
    public delegate void EndToolMovement(int cellID, int lastCellID, ToolMoveType moveCount);
    public static event EndToolMovement EndToolMovementEvent;
    public delegate void ChangePoints();
    public static event ChangePoints ChangePointsEvent;
    public delegate void ZeroControlTime();
    public static event ZeroControlTime ZeroControlTimeEvent;

    private static float timeMoveDown = 0.1f;

    private Transform toolTransform;

    private int cellID = -1;
    private float timeForExchange = 0.25f;
    private float timeMoveToTheBox = 0.3f;
    private bool isExchange = false;
    private bool isMovingDown = false;

    private void Awake()
    {
        toolTransform = this.transform;
    }

    private void OnEnable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent += MoveTool;
        AnalysisToolsRelativePosition.GatherToolEvent += DeactivateTool;
        AnalysisToolsRelativePosition.MoveDownToolEvent += MoveDownTool;
        AnalysisToolsRelativePosition.ShakeUpEvent += SleepTool;
    }

    private void OnDisable()
    {
        AnalysisToolsRelativePosition.MoveToolEvent -= MoveTool;
        AnalysisToolsRelativePosition.GatherToolEvent -= DeactivateTool;
        AnalysisToolsRelativePosition.MoveDownToolEvent -= MoveDownTool;
        AnalysisToolsRelativePosition.ShakeUpEvent -= SleepTool;
    }

    private void MoveTool(int id, int newId, float newX, float newY, ToolMoveType moveCount)
    {
        if (cellID == id && !isExchange)
        {
            StartCoroutine(MoveCoroutine(id, newId, newX, newY, moveCount));
        }
    }

    private IEnumerator MoveCoroutine(int id, int newId, float newX, float newY, ToolMoveType toolMoveType)
    {
        isExchange = true;
        if (ExchangeMarkEvent != null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        int toolType = Field.toolsOnField[id];
        Field.toolsOnField[id] = (int)ToolType.exchangeTool;
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
            toolTransform.position = Vector3.Lerp(curentPosition, targetPosition, moveTime * frequency);
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
            EndToolMovementEvent(newId, id, toolMoveType);
        }
    }

    private void SleepTool()
    {
        ToolsPool.toolsOnFieldListOfStacks[Field.toolsOnField[cellID]].Pop();
        ToolsPool.toolsReservedListOfStacks[Field.toolsOnField[cellID]].Push(this.gameObject);
        this.gameObject.SetActive(false);
    }

    private void DeactivateTool(int id)
    {
        if (cellID == id && cellID >= 0 && Field.toolsOnField[id] >= 0)
        {
            if (!isMovingDown)
            {
                if (ZeroControlTimeEvent != null)
                {
                    ZeroControlTimeEvent();
                }

                GameIndicators.points += GameIndicators.pointsForTool;
                if (ChangePointsEvent != null)
                {
                    ChangePointsEvent();
                }
                SleepTool();
                Field.emptyOnField[cellID] = (int)Cell.isEmpty;
                Field.toolsOnField[cellID] = (int)ToolType.noTool;
                //     StartCoroutine(MoveToTheBoxCoroutine());
            }
        }
    }

    private IEnumerator MoveToTheBoxCoroutine() // создавать другие объекты и их уже "кидать" в коробку
    {
        float moveTime = 0f;
        Vector3 curentPosition = toolTransform.position;
        float frequency = 1 / timeMoveToTheBox;
        float startScale = Field.CurentUnitScale * 1.5f;
        float targetScale = Field.CurentUnitScale * 0.4f;
        int lastId = cellID;
        cellID = -1;

        while (moveTime < timeMoveToTheBox)
        {
            moveTime += Time.deltaTime;
            if (moveTime > timeMoveToTheBox)
            {
                moveTime = timeMoveToTheBox;
            }
            toolTransform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, moveTime * frequency);
            toolTransform.position = Vector3.Lerp(curentPosition, GameIndicators.boxPointPosition, moveTime * frequency);
            yield return null;
        }
        Field.emptyOnField[lastId] = (int)Cell.isEmpty;
        Field.toolsOnField[lastId] = (int)ToolType.noTool;
        this.gameObject.SetActive(false);
    }

    private void MoveDownTool(int id)
    {
        if (cellID == id && !isExchange)
        {
            isMovingDown = true;
            StartCoroutine(MoveDownCoroutine(id));
        }
    }

    private IEnumerator MoveDownCoroutine(int id)
    {
        isExchange = true;
        if (ExchangeMarkEvent != null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        int newId = id + Field.CurentFieldWidth;
        int toolType = Field.toolsOnField[id];
        Field.toolsOnField[id] = (int)ToolType.noTool;
        Vector3 curentPosition = toolTransform.position;
        Vector3 targetPosition = new Vector3(Field.cellsXYCoord[newId, (int)Cell.x], Field.cellsXYCoord[newId, (int)Cell.y], curentPosition.z);
        float frequency = 1 / timeMoveDown;
        float moveTime = 0f;
        yield return null;
        if (DeselectCellEvent != null)
        {
            DeselectCellEvent(id);
        }
        while (moveTime < timeMoveDown)
        {
            moveTime += Time.deltaTime;
            if (moveTime > timeMoveDown)
            {
                moveTime = timeMoveDown;
            }
            toolTransform.position = Vector3.Lerp(curentPosition, targetPosition, moveTime * frequency);
            yield return null;
        }
        isExchange = false;
        Field.toolsOnField[newId] = toolType;
        cellID = newId;
        if (ExchangeMarkEvent != null)
        {
            ExchangeMarkEvent(id, isExchange);
        }
        isMovingDown = false;
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

    public static float TimeMoveDown
    {
        get
        {
            return timeMoveDown;
        }
        private set
        {
            timeMoveDown = value;
        }
    }
}

