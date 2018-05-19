using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CellType
{
    centreBottom,
    centreCentre,
    centreTop,
    leftBottom,
    leftCentre,
    leftTop,
    rightBottom,
    rightCentre,
    rightTop,
    quantity
}
public enum CellColour
{
    blue = 0,
    green,
    yellow,
    quantity
}

public class CellsPool : MonoBehaviour
{
    [SerializeField] private GameObject[] blueCells = new GameObject[(int)CellType.quantity];
    [SerializeField] private GameObject[] greenCells = new GameObject[(int)CellType.quantity];
    [SerializeField] private GameObject[] yellowCells = new GameObject[(int)CellType.quantity];

    public static List<Stack<GameObject>> blueDisableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);
    public static List<Stack<GameObject>> greenDisableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);
    public static List<Stack<GameObject>> yellowDisableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);

    public static List<Stack<GameObject>> blueEnableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);
    public static List<Stack<GameObject>> greenEnableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);
    public static List<Stack<GameObject>> yellowEnableCellsListOfStacks = new List<Stack<GameObject>>((int)CellType.quantity);


    private int feildWidth = 16;
    private int feildHieght = 16;
    private int[] numberOfPiecesEachCellType;

    [SerializeField] private Transform[] parentTransform = new Transform[(int)CellColour.quantity];

    private void Awake()
    {
        SetNumberOfPiecesEachCellType(out numberOfPiecesEachCellType, feildWidth, feildHieght);

        FillStack(ref blueDisableCellsListOfStacks, blueCells, numberOfPiecesEachCellType, parentTransform[(int)CellColour.blue]);
        FillStack(ref greenDisableCellsListOfStacks, greenCells, numberOfPiecesEachCellType, parentTransform[(int)CellColour.green]);
        FillStack(ref yellowDisableCellsListOfStacks, yellowCells, numberOfPiecesEachCellType, parentTransform[(int)CellColour.yellow]);
        SetEnableCellsStack(ref blueEnableCellsListOfStacks, numberOfPiecesEachCellType);
        SetEnableCellsStack(ref greenEnableCellsListOfStacks, numberOfPiecesEachCellType);
        SetEnableCellsStack(ref yellowEnableCellsListOfStacks, numberOfPiecesEachCellType);
    }


    private void FillStack(ref List<Stack<GameObject>> stackList, GameObject[] Cells, int[] numberOfPiecesEachCellType, Transform parentTransform)
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        Vector3 position = Vector3.zero;
        GameObject instanceObject;
        for (int i = 0; i < (int)CellType.quantity; i++)
        {
            for (int j = 0; j < numberOfPiecesEachCellType[i]; j++)
            {
                stackList.Add(new Stack<GameObject>(numberOfPiecesEachCellType[i]));
                instanceObject = Instantiate(Cells[i], position, quaternion, parentTransform);

                stackList[i].Push(instanceObject);
                instanceObject.SetActive(false);
            }
        }
    }

    private void SetEnableCellsStack(ref List<Stack<GameObject>> stackList, int[] numberOfPiecesEachCellType)
    {
        for (int i = 0; i < (int)CellType.quantity; i++)
        {
            for (int j = 0; j < numberOfPiecesEachCellType[i]; j++)
            {
                stackList.Add(new Stack<GameObject>(numberOfPiecesEachCellType[i]));
            }
        }
    }

    private void SetNumberOfPiecesEachCellType(out int[] numberOfPiecesEachCellType,int feildWidth, int feildHieght)
    {
        numberOfPiecesEachCellType = new int[(int)CellType.quantity];

        numberOfPiecesEachCellType[(int)CellType.leftBottom] = 1;
        numberOfPiecesEachCellType[(int)CellType.leftTop] = 1;
        numberOfPiecesEachCellType[(int)CellType.rightBottom] = 1;
        numberOfPiecesEachCellType[(int)CellType.rightTop] = 1;


        numberOfPiecesEachCellType[(int)CellType.centreBottom] = feildWidth - numberOfPiecesEachCellType[(int)CellType.leftBottom] - numberOfPiecesEachCellType[(int)CellType.rightBottom];
        numberOfPiecesEachCellType[(int)CellType.centreTop] = feildWidth - numberOfPiecesEachCellType[(int)CellType.leftTop] - numberOfPiecesEachCellType[(int)CellType.rightTop];
        numberOfPiecesEachCellType[(int)CellType.leftCentre] = feildHieght - numberOfPiecesEachCellType[(int)CellType.leftTop] - numberOfPiecesEachCellType[(int)CellType.leftBottom];
        numberOfPiecesEachCellType[(int)CellType.rightCentre] = feildHieght - numberOfPiecesEachCellType[(int)CellType.rightTop] - numberOfPiecesEachCellType[(int)CellType.rightBottom];

        numberOfPiecesEachCellType[(int)CellType.centreCentre] = feildWidth * feildHieght - numberOfPiecesEachCellType[(int)CellType.leftBottom] -
        numberOfPiecesEachCellType[(int)CellType.leftTop] - numberOfPiecesEachCellType[(int)CellType.rightBottom] -
        numberOfPiecesEachCellType[(int)CellType.rightTop] - numberOfPiecesEachCellType[(int)CellType.centreBottom] -
        numberOfPiecesEachCellType[(int)CellType.centreTop] - numberOfPiecesEachCellType[(int)CellType.leftCentre] -
        numberOfPiecesEachCellType[(int)CellType.rightCentre];
    }

}
