/// <Sumery>
/// This class is responsible for:
/// 1. Processing selected character related operations like selecting, deselecting, occupying cell, attack, defence and moving
/// </Summery>

using UnityEngine;

public class Character : MonoBehaviour, IPlayerAttack, IPlayerDefend, IPlayerWalk, IPlayerOccupyCell
{
    public enum Character_State
    {
        Idle,
        Selected,
        Moving
    }


    #region Variables

    public GridConfiguration gridConfig;
    public CharacterProperties characterProperties;
    public PhotonNetworkManager.Player_Identity PlayerType;
    public CustomDataStructures.CellIndex CurrentCell;
    public Character_State CharacterState; 
    public CapsuleCollider CharacterCollider;

    public int CharacterID;
    [SerializeField] Renderer MyRenderer;

    #endregion


    #region Initialization

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected += OnCharacterDeselected;
        ActionsContainer.OnPlayerSideSwitch += OnPlayerSideSwitched;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected -= OnCharacterDeselected;
        ActionsContainer.OnPlayerSideSwitch -= OnPlayerSideSwitched;
    }

    void Start()
    {
        Vector3 mySize = Vector3.one * (gridConfig.CellSize * gridConfig.CharacterRelativeSize);
        transform.localScale = mySize;

        if (MyRenderer)
        {
            MyRenderer.material = characterProperties.MatDim;
        }
    }

    #endregion


    #region Assign Action Callbacks

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

    #endregion


    #region Interface implementation

    public void DoAttack()
    {
        // Define attack mechanism
    }

    public void DoDefend()
    {
        // Define defence mechanism
    }

    // Occupy the cell whenever the character reaches the destination cell
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

    #endregion


    // Highlight character colors based on current player turn
    void OnPlayerSideSwitched(PhotonNetworkManager.Player_Identity _currentPlayer)
    {
        if(PhotonNetworkManager.Instance.MyIdentity == PlayerType && _currentPlayer == PlayerType)
        {
            if (MyRenderer)
            {
                MyRenderer.material = characterProperties.MatHighlight;
            }
        }
        else
        {
            if (MyRenderer)
            {
                MyRenderer.material = characterProperties.MatDim;
            }
        }
    }
}