using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Range(0.5f, 1.0f)]
    public float CharacterRelativeSize = 0.85f;

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
        ActionsContainer.OnTargetCellSelected += OnTargetCellSelected;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected -= OnAllCharactersDeSelected;
        ActionsContainer.OnTargetCellSelected -= OnTargetCellSelected;
    }

    void Start()
    {

    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
        SelectedCell = _character.characterProperties.CurrentCell;
    }

    void OnAllCharactersDeSelected()
    {
        SelectedCharacter = null;
    }

    void OnTargetCellSelected(CustomDataStructures.CellIndex _targetCellIndex)
    {
        InputDetectedDirection = GridManager.Instance.GetInputDirection(SelectedCell, _targetCellIndex);
    }
}