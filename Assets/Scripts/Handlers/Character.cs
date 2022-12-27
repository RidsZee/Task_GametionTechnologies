using UnityEngine;

public class Character : MonoBehaviour, IPlayerAttack, IPlayerDefend, IPlayerWalk, IPlayerOccupyCell
{
    public enum Character_State
    {
        Idle,
        Selected,
        Moving
    }

    public CharacterProperties characterProperties;
    public GridConfiguration gridConfig;

    public CustomDataStructures.CellIndex CurrentCell;
    public Character_State CharacterState;

    public int CharacterID;

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected += OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected += AllCharactersDeSelected;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected -= OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected -= AllCharactersDeSelected;
    }

    void Start()
    {
        Vector3 mySize = Vector3.one * (gridConfig.TileSize * gridConfig.CharacterRelativeSize);
        transform.localScale = mySize;
    }

    void AllCharactersDeSelected()
    {
        OnCharacterDeselected(this);
    }

    void OnCharacterSelected(Character _character)
    {
        if (characterProperties)
        {
            if(_character.CharacterID == CharacterID)
            {
                if (characterProperties.doAttack)
                {
                    ActionsContainer.OnAttack += DoAttack;
                }

                if (characterProperties.doDefend)
                {
                    ActionsContainer.OnDefend += DoDefend;
                }

                if (characterProperties.doWalk)
                {
                    ActionsContainer.OnWalk += DoWalk;
                    ActionsContainer.OnOccupyCell += DoOccupyCell;
                }

                CharacterState = Character_State.Selected;
            }
            else
            {
                OnCharacterDeselected(this);
            }
        }
    }

    void OnCharacterDeselected(Character _character)
    {
        if (characterProperties)
        {
            if(_character.CharacterID == CharacterID && CharacterState == Character_State.Selected)
            {
                if (characterProperties.doAttack)
                {
                    ActionsContainer.OnAttack -= DoAttack;
                }

                if (characterProperties.doDefend)
                {
                    ActionsContainer.OnDefend -= DoDefend;
                }

                if (characterProperties.doWalk)
                {
                    ActionsContainer.OnWalk -= DoWalk;
                    ActionsContainer.OnOccupyCell -= DoOccupyCell;
                }

                CharacterState = Character_State.Idle;

                GridManager.Instance.GetCellData_From_CellIndex(CurrentCell).SetDefaultColor();
            }
        }
    }


    public void DoAttack()
    {
        // Define attack mechanism
    }

    public void DoDefend()
    {
        // Define defence mechanism
    }

    // Occupy cell whenever the character moves to a new cell / destination
    public void DoOccupyCell(CustomDataStructures.CellIndex _cellIndex)
    {
        CurrentCell = _cellIndex;
        CellData cellData = GridManager.Instance.GetCellData_From_CellIndex(_cellIndex);
        cellData.isOccupied = true;
    }

    public void DoWalk(CellData _targetDestinationCell)
    {
        ActionsContainer.OnBeginWalking?.Invoke(_targetDestinationCell);
    }
}