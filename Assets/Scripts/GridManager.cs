using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField]
    GridConfiguration gridConfig;

    Vector3[] MovementSteps;

    int MaxAmountOfSteps;
    
    public int CurrentIndex;

    int GridLength;
    int GridWidth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(!gridConfig)
        {
            Debug.LogWarning("Grid config not found in " + name);
            return;
        }

        GridLength = gridConfig.GridLength;
        GridWidth = gridConfig.GridWidth;

        MaxAmountOfSteps = gridConfig.GridLength;
        MovementSteps = new Vector3[MaxAmountOfSteps];
    }

    public void DrawPath(int _currentCellIndex, CharacterProperties.Character_Type _characterType, CharacterProperties.Movement_Type _movementType)
    {
        
    }


    bool CheckMovementPossibility(bool CheckVertical)
    {
        if(CurrentIndex < GridLength - 1)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    bool CheckMovementPossibility(bool CheckVertical, bool CheckHorizontal)
    {
        return false;
    }

    bool CheckMovementPossibility(bool CheckVertical, bool CheckHorizontal, int StepsCount)
    {
        return false;
    }
}