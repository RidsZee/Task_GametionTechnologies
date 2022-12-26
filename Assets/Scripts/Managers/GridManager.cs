using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    GridConfiguration gridConfig;

    public CellDataContainer cellDataContainer;

    public static GridManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public CellData GetCellData_From_CellIndex(CustomDataStructures.CellIndex _cellIndex)
    {
        int ListIndex = (_cellIndex.Vertical * gridConfig.GridWidth) + _cellIndex.Horizontal;
        return cellDataContainer.AllCells[ListIndex];
    }

    public CharacterProperties.Movement_Type GetInputDirection(CustomDataStructures.CellIndex _characterPos, CustomDataStructures.CellIndex _inputPosition)
    {
        int Diff_Horizontal = _inputPosition.Horizontal - _characterPos.Horizontal;
        int Diff_Vertical = _inputPosition.Vertical - _characterPos.Vertical;

        if (Diff_Horizontal == 0 || Diff_Vertical != 0) // Vertical
        {
            return CharacterProperties.Movement_Type.Vertical;
        }
        else if (Diff_Horizontal != 0 || Diff_Vertical == 0) // Horizontal
        {
            return CharacterProperties.Movement_Type.Horizontal;
        }
        else if (GetSignedNumber(Diff_Horizontal) == GetSignedNumber(Diff_Vertical)) // Diagonal movement
        {
            return CharacterProperties.Movement_Type.Diagonal;
        }
        else if(GetSignedNumber (Diff_Horizontal) == 2 && GetSignedNumber(Diff_Vertical) == 1) // L Shape short
        {
            return CharacterProperties.Movement_Type.LShape;
        }
        else if (GetSignedNumber(Diff_Horizontal) == 1 && GetSignedNumber(Diff_Vertical) == 2) // L Shape long
        {
            return CharacterProperties.Movement_Type.LShape;
        }
        else
        {
            return CharacterProperties.Movement_Type.None;
        }
    }

    int GetSignedNumber(int _number)
    {
        if(_number < 0)
        {
            return (_number * -1);
        }
        else
        {
            return _number;
        }
    }    

    public int GetCellDistance(CustomDataStructures.CellIndex _characterPos, CustomDataStructures.CellIndex _inputPosition)
    {
        return 0;
    }
}