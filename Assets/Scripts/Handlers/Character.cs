using UnityEngine;

public class Character : MonoBehaviour, IPlayerAttack, IPlayerDefend, IPlayerWalk, IPlayerOccupyCell
{
    public enum Character_State
    {
        Idle,
        Selected,
        Moving
    }

    public PhotonNetworkManager.Player_Identity PlayerType;
    public CharacterProperties characterProperties;
    public GridConfiguration gridConfig;
    public CustomDataStructures.CellIndex CurrentCell;
    public Character_State CharacterState;
    public int CharacterID;
    public CapsuleCollider CharacterCollider;
    [SerializeField]
    Renderer MyRenderer;

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected += OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected += AllCharactersDeSelected;
        ActionsContainer.OnPlayerSideSwitch += OnPlayerSideSwitched;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected -= OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected -= AllCharactersDeSelected;
        ActionsContainer.OnPlayerSideSwitch -= OnPlayerSideSwitched;
    }

    void Start()
    {
        Vector3 mySize = Vector3.one * (gridConfig.TileSize * gridConfig.CharacterRelativeSize);
        transform.localScale = mySize;
        SetDimColor();
    }

    void AllCharactersDeSelected()
    {
        OnCharacterDeselected(CharacterID);
    }

    void OnCharacterSelected(int _characterID)
    {
        if (characterProperties)
        {
            if(_characterID == CharacterID)
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
                    //
                    UIManager.Instance.UpdateMultiplayerLogs("R");
                    ActionsContainer.OnWalk += DoWalk;
                    ActionsContainer.OnOccupyCell += DoOccupyCell;
                }

                CharacterState = Character_State.Selected;
            }
            else
            {
                OnCharacterDeselected(CharacterID);
            }
        }
    }

    void OnCharacterDeselected(int _characterID)
    {
        if (characterProperties)
        {
            if(_characterID == CharacterID && CharacterState == Character_State.Selected || CharacterState == Character_State.Moving)
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


    void OnPlayerSideSwitched(PhotonNetworkManager.Player_Identity _currentPlayer)
    {
        if(PhotonNetworkManager.Instance.MyIdentity == PlayerType && _currentPlayer == PlayerType)
        {
            SetHighlightColor();
        }
        else
        {
            SetDimColor();
        }
    }

    void SetHighlightColor()
    {
        if(MyRenderer)
        {
            MyRenderer.material = characterProperties.MatHighlight;
        }
    }

    void SetDimColor()
    {
        if (MyRenderer)
        {
            MyRenderer.material = characterProperties.MatDim;
        }
    }
}