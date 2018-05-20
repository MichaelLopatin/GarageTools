using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    hacksaw,
    hacksawGroup,
    hammer,
    hammerGroup,
    measureInstrument,
    measureInstrumentGroup,
    pliers,
    pliersGroup,
    screwdriver,
    screwdriverGroup,
    spanner,
    spannerGroup,
    toolBox,
    quantity
}


public class ToolsPool : MonoBehaviour
{

    [SerializeField] private GameObject[] toolTypes = new GameObject[(int)ToolType.quantity];
    [SerializeField] private Transform[] parentTransforms = new Transform[(int)ToolType.quantity];
    private int feildWidth = 16;
    private int feildHieght = 16;


    public static List<Stack<GameObject>> toolsReservedListOfStacks = new List<Stack<GameObject>>((int)ToolType.quantity);
    public static List<Stack<GameObject>> toolsOnFieldListOfStacks = new List<Stack<GameObject>>((int)ToolType.quantity);

    private int[] numberOfPiecesEachToolType;

    private void Awake()
    {
        SetNumberOfPiecesEachToolType(out numberOfPiecesEachToolType, feildWidth, feildHieght);
        FillToolsReservedList(ref toolsReservedListOfStacks, toolTypes, numberOfPiecesEachToolType, parentTransforms);
        SetToolsOnFieldList(ref toolsOnFieldListOfStacks, numberOfPiecesEachToolType);
    }

    private void FillToolsReservedList(ref List<Stack<GameObject>> listOfStacks, GameObject[] toolTypes, int[] numberOfPiecesEachToolType, Transform[] parentTransform)
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        Vector3 position = Vector3.zero;
        GameObject tool;
        for (int i = 0; i < (int)ToolType.quantity; i++)
        {
            listOfStacks.Add(new Stack<GameObject>(numberOfPiecesEachToolType[i]));
            for (int j = 0; j < numberOfPiecesEachToolType[i]; j++)
            {
                tool = Instantiate(toolTypes[i], position, quaternion, parentTransform[i]);
                listOfStacks[i].Push(tool);
                tool.SetActive(false);
            }
        }
    }

    private void SetToolsOnFieldList(ref List<Stack<GameObject>> stackList, int[] numberOfPiecesEachToolType)
    {
        for (int i = 0; i < (int)CellType.quantity; i++)
        {
            stackList.Add(new Stack<GameObject>(numberOfPiecesEachToolType[i]));
        }
    }

    private void SetNumberOfPiecesEachToolType(out int[] numberOfPiecesEachToolType, int feildWidthint, int feildHieght)
    {
        int maxToolsNumber = (int)(feildWidth * feildHieght * 0.67f);
        int toolsGroupsNumber = (int)(maxToolsNumber * 0.3f);
        numberOfPiecesEachToolType = new int[(int)ToolType.quantity];

        for (int i = 0; i < (int)ToolType.quantity; i++)
        {
            if(i < (int)ToolType.toolBox)
            {
                if (i % 2 == 0)
                {
                    numberOfPiecesEachToolType[i] = maxToolsNumber;
                }
                else
                {
                    numberOfPiecesEachToolType[i] = toolsGroupsNumber;
                }
            }
            else
            {
                numberOfPiecesEachToolType[i] = (int)(toolsGroupsNumber * 0.25f);
            }
        }
    }

}
