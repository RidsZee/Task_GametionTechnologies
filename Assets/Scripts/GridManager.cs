using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField]
    GridConfiguration gridConfig;

    Vector3[] MovementSteps;

    int MaxAmountOfSteps;
    int CurrentNumbeOfSteps;

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

        MaxAmountOfSteps = gridConfig.GridLength;
        MovementSteps = new Vector3[MaxAmountOfSteps];
    }

    public void DrawPath(CharacterProperties.Character_Type _characterType, CharacterProperties.Movement_Type _movementType)
    {
        
    }
}