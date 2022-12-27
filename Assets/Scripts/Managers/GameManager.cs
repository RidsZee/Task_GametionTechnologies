using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Range(0.5f, 1.0f)]
    public float CharacterRelativeSize = 0.85f;

    [Range(0.1f, 1.0f)]
    public float CharacterMovementSpeed = 0.5f;

    [SerializeField]
    GridConfiguration gridConfig;

    Character SelectedCharacter;
    CustomDataStructures.CellIndex SelectedCell;
    int CellDistance;
    CharacterProperties.Movement_Type InputDetectedDirection;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected += OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking += OnCharacterWalkStarted;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected -= OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking += OnCharacterWalkStarted;
    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
        SelectedCell = _character.CurrentCell;

        print("Character cell : " + SelectedCell.Horizontal + ", " + SelectedCell.Vertical);

        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetHighlightColor(gridConfig.MatHighlight);
    }

    void OnAllCharactersDeSelected()
    {
        SelectedCharacter = null;
        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetDefaultColor();
    }

    void OnCharacterWalkStarted(CellData _cellData)
    {
        print("Target cell : " + _cellData.CellIndex.Horizontal + ", " + _cellData.CellIndex.Vertical);

        // Calculate the direction between current selected cell and selected character
        InputDetectedDirection = GridManager.Instance.GetInputDirection(SelectedCell, _cellData.CellIndex);

        print("Direction found : " + InputDetectedDirection);

        // Calculate the cell-distance between selected character and selected target cell
        CellDistance = GridManager.Instance.GetCellDistance(SelectedCharacter.CurrentCell, _cellData.CellIndex, InputDetectedDirection);

        print("Cell distance : " + CellDistance);

        // Check if the direction of input is matching with character's movements
        
        bool doesMovementMatch = false;
        
        foreach(CharacterProperties.Movement_Type _movement in SelectedCharacter.characterProperties.Movements)
        {
            print("Selected character movements : " + _movement);

            if(_movement == InputDetectedDirection)
            {
                doesMovementMatch = true;
            }
        }

        print("Does movement match? " + doesMovementMatch);

        // Check if the maximum number of steps match with character's properties

        bool doesCellDistanceMatch = false;

        if(doesMovementMatch)
        {
            int CharacterMaxSteps = (int)SelectedCharacter.characterProperties.MaxSteps;

            print("Selected character max steps : " + CharacterMaxSteps);

            if(CharacterMaxSteps == 0) // Which is infinite
            {
                doesCellDistanceMatch = true;
            }
            else if(CellDistance <= CharacterMaxSteps) // Character with finite movement steps fit into the target destination
            {
                doesCellDistanceMatch = true;
            }
            else // Destination is out of character's reach
            {
                doesCellDistanceMatch = false;
            }

            print("Does distance match? " + doesCellDistanceMatch);

            if(doesCellDistanceMatch)
            {
                // Calculate the list of CellData as a path to destination
                MoveCharacterToDestination(_cellData);
            }
            else
            {
                Debug.LogWarning("Destination is out of character's reach | Character's max steps : " + CharacterMaxSteps + ", Input distance : " + CellDistance);
            }
        }
        else
        {
            Debug.LogWarning("Movement does not match | Current input movement : " + InputDetectedDirection);
        }
    }

    void MoveCharacterToDestination(CellData _destinationCell)
    {
        GameStateManager.Instance.GameState = GameStateManager.Game_State.MovingCharacter;
        print("Wohoooo!!! Moving character to the destination");

        Vector3[] NewArray = GridManager.Instance.GetPathPoints(SelectedCell, _destinationCell.CellIndex, InputDetectedDirection, CellDistance);
    }
}