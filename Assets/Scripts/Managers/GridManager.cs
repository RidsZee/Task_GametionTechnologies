/// <Sumery>
/// This class is responsible for:
/// 1. All grid related operations like calculating distance, finding route, checking directions, forming paths etc
/// 2. Processing and returning cell data <CellData> based on cell identities like CellIndex
/// </Summery>

using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Variables

    public static GridManager Instance;
    public CellDataContainer cellDataContainer;

    [SerializeField] GridConfiguration gridConfig;

    Vector3[] PathPoints;
    int Diff_Horizontal;
    int Diff_Vertical;
    int ListIndex;
    int TotalSteps;

    CustomDataStructures.CellIndex CurrentCellIndex;
    int CurrentDistance;
    CustomDataStructures.CellIndex Incrementals;

    #endregion


    #region Initialization

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(gridConfig)
        {
            // Set the array length to maximum possible grid distance
            PathPoints = new Vector3[gridConfig.GridWidth > gridConfig.GridLength ? gridConfig.GridWidth : gridConfig.GridLength];
        }
    }

    #endregion


    // Return CellData associated with CellIndex
    public CellData GetCellData_From_CellIndex(CustomDataStructures.CellIndex _cellIndex)
    {
        ListIndex = (_cellIndex.Vertical * gridConfig.GridWidth) + _cellIndex.Horizontal;
        return cellDataContainer.AllCells[ListIndex];
    }


    // Calculate path of user input to the selected character
    public CharacterProperties.Movement_Type GetInputDirection(CustomDataStructures.CellIndex _characterPos, CustomDataStructures.CellIndex _inputPosition)
    {
        Diff_Horizontal = _inputPosition.Horizontal - _characterPos.Horizontal;
        Diff_Vertical = _inputPosition.Vertical - _characterPos.Vertical;

        if (Diff_Horizontal == 0 && Diff_Vertical != 0) // Vertical
        {
            if(Diff_Vertical > 0)
            {
                return CharacterProperties.Movement_Type.Forward;
            }
            else
            {
                return CharacterProperties.Movement_Type.Backward;
            }
        }
        else if (Diff_Horizontal != 0 && Diff_Vertical == 0) // Horizontal
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


    // Calculate cell distance between character position and selected target cell
    public int GetCellDistance(CustomDataStructures.CellIndex _characterPos, CustomDataStructures.CellIndex _inputPosition, CharacterProperties.Movement_Type _movement)
    {
        Diff_Horizontal = _inputPosition.Horizontal - _characterPos.Horizontal;
        Diff_Vertical = _inputPosition.Vertical - _characterPos.Vertical;

        TotalSteps = 0;

        if(_movement == CharacterProperties.Movement_Type.Horizontal || _movement == CharacterProperties.Movement_Type.Forward || _movement == CharacterProperties.Movement_Type.Backward)
        {
            TotalSteps = Diff_Horizontal + Diff_Vertical;
        }
        else if(_movement == CharacterProperties.Movement_Type.Diagonal)
        {
            TotalSteps = Diff_Horizontal;
        }
        else if(_movement == CharacterProperties.Movement_Type.LShape)
        {
            TotalSteps = 3;
        }
        
        if(TotalSteps < 0)
        {
            TotalSteps *= -1;
        }

        return TotalSteps;
    }


    // Calculate path points to travel to the destination cell from selected player cell
    public Vector3[] GetPathPoints(CustomDataStructures.CellIndex _startPoint, CustomDataStructures.CellIndex _destination, CharacterProperties.Movement_Type _direction, int _distance)
    {
        Diff_Horizontal = _destination.Horizontal - _startPoint.Horizontal;
        Diff_Vertical = _destination.Vertical - _startPoint.Vertical;

        // Calculate incrementals

        CurrentDistance = 0;

        Incrementals.Horizontal = 0;
        Incrementals.Vertical = 0;

        if (Diff_Horizontal > 0)
        {
            Incrementals.Horizontal = 1;
        }
        else if (Diff_Horizontal < 0)
        {
            Incrementals.Horizontal = -1;
        }

        if (Diff_Vertical > 0)
        {
            Incrementals.Vertical = 1;
        }
        else if (Diff_Vertical < 0)
        {
            Incrementals.Vertical = -1;
        }

        // Calculate path points

        if (_direction != CharacterProperties.Movement_Type.LShape)
        {
            while (CurrentDistance < _distance)
            {
                CurrentDistance++;

                CurrentCellIndex.Horizontal = _startPoint.Horizontal + (CurrentDistance * Incrementals.Horizontal);
                CurrentCellIndex.Vertical = _startPoint.Vertical + (CurrentDistance * Incrementals.Vertical);

                PathPoints[CurrentDistance - 1] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
            }
        }
        else if (_direction == CharacterProperties.Movement_Type.LShape)
        {
            CustomDataStructures.CellIndex _TempCellIndex;

            if(Diff_Vertical == 1 || Diff_Vertical == -1)
            {
                CurrentCellIndex.Vertical = _startPoint.Vertical + Diff_Vertical;
                CurrentCellIndex.Horizontal = _startPoint.Horizontal;

                PathPoints[0] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
                _TempCellIndex = CurrentCellIndex;

                CurrentCellIndex.Vertical = _TempCellIndex.Vertical;
                CurrentCellIndex.Horizontal = _TempCellIndex.Horizontal + Incrementals.Horizontal;

                PathPoints[1] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
                
                CurrentCellIndex.Vertical = _TempCellIndex.Vertical;
                CurrentCellIndex.Horizontal = _TempCellIndex.Horizontal + Incrementals.Horizontal + Incrementals.Horizontal;

                PathPoints[2] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
            }
            else if(Diff_Horizontal == 1 || Diff_Horizontal == -1)
            {
                CurrentCellIndex.Horizontal = _startPoint.Horizontal + Diff_Horizontal;
                CurrentCellIndex.Vertical = _startPoint.Vertical;

                PathPoints[0] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
                _TempCellIndex = CurrentCellIndex;
                
                CurrentCellIndex.Vertical = _TempCellIndex.Vertical + Incrementals.Vertical;
                CurrentCellIndex.Horizontal = _TempCellIndex.Horizontal;

                PathPoints[1] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
                
                CurrentCellIndex.Vertical = _TempCellIndex.Vertical + Incrementals.Vertical + Incrementals.Vertical;
                CurrentCellIndex.Horizontal = _TempCellIndex.Horizontal;

                PathPoints[2] = GetCellData_From_CellIndex(CurrentCellIndex).transform.position;
            }
        }

        return PathPoints;
    }
}