using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GridConfiguration gridConfig;

    Character SelectedCharacter;
    CustomDataStructures.CellIndex SelectedCell;
    int CellDistance;
    CharacterProperties.Movement_Type InputDetectedDirection;
    CellData CurrentCellData;
    CellData TargetCellData;
    Vector3[] PathPoints;
    Coroutine Coroutine_MoveCharacter;

    int currentCellIndex_H;
    int currentCellIndex_V;
    int targetCellIndex_H;
    int targetCellIndex_V;

    const string Warning1 = "Destination is out of character's reach!";
    const string Warning2 = "Invalid Cell for the selected character";
    const string None = "None";

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected += OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking += OnCharacterWalkStarted;
        ActionsContainer.OnCharacterReachedTarget += OnCharacterReachedTarget;
        ActionsContainer.OnGameStart += RPC_Receive_GameStart;
        ActionsContainer.OnPlayerSideSwitch += RPC_Receive_SwitchSides;
        ActionsContainer.OnSyncCharacterMovement += RPC_Receive_MoveCharacterToDestination;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected -= OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking -= OnCharacterWalkStarted;
        ActionsContainer.OnCharacterReachedTarget -= OnCharacterReachedTarget;
        ActionsContainer.OnGameStart -= RPC_Receive_GameStart;
        ActionsContainer.OnPlayerSideSwitch -= RPC_Receive_SwitchSides;
        ActionsContainer.OnSyncCharacterMovement -= RPC_Receive_MoveCharacterToDestination;
    }

    void OnCharacterSelected(int _characterID)
    {
        SelectedCharacter = CharacterManager.Instance.GetCharacterFromCharacterID(_characterID);
        SelectedCell = SelectedCharacter.CurrentCell;
        CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(SelectedCell);

        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetHighlightColor(gridConfig.MatHighlight);
        UIManager.Instance.gameStatus.Update_SelectedCharacter(SelectedCharacter.characterProperties.CharacterType);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    void OnAllCharactersDeSelected()
    {
        SelectedCharacter = null;
        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetDefaultColor();
        UIManager.Instance.gameStatus.SelectedCharacter = None;
        UIManager.Instance.UpdateGameStatsInUI();
    }

    void OnCharacterWalkStarted(CellData _cellData)
    {
        TargetCellData = _cellData;

        // Calculate the direction between current selected cell and selected character
        InputDetectedDirection = GridManager.Instance.GetInputDirection(SelectedCell, _cellData.CellIndex);

        // Calculate the cell-distance between selected character and selected target cell
        CellDistance = GridManager.Instance.GetCellDistance(SelectedCharacter.CurrentCell, _cellData.CellIndex, InputDetectedDirection);

        // Check if the direction of input is matching with character's movements
        
        bool doesMovementMatch = false;
        
        foreach(CharacterProperties.Movement_Type _movement in SelectedCharacter.characterProperties.Movements)
        {
            if(_movement == InputDetectedDirection)
            {
                doesMovementMatch = true;
            }
        }

        // Check if the maximum number of steps match with character's properties

        bool doesCellDistanceMatch = false;

        if(doesMovementMatch)
        {
            int CharacterMaxSteps = (int)SelectedCharacter.characterProperties.MaxSteps;

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

            if(doesCellDistanceMatch)
            {
                // Calculate the list of CellData as a path to destination
                TargetCellData.SetCorrectColor(gridConfig.MatCorrectCell);
                MoveCharacterToDestination(_cellData);
            }
            else
            {
                TargetCellData.SetWrongColor(gridConfig.MatWrongCell, gridConfig.HighlightWrongCell);
                UIManager.Instance.ShowWarning(Warning1);
            }
        }
        else
        {
            TargetCellData.SetWrongColor(gridConfig.MatWrongCell, gridConfig.HighlightWrongCell);
            UIManager.Instance.ShowWarning(Warning2);
        }
    }


    #region Character movement

    void MoveCharacterToDestination(CellData _destinationCell)
    {
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.MovingCharacter);

        PathPoints = GridManager.Instance.GetPathPoints(SelectedCell, _destinationCell.CellIndex, InputDetectedDirection, CellDistance);

        RPCManager.Instance.SendRPC_SyncPlayerMovement(SelectedCharacter.CharacterID, PathPoints, CellDistance, SelectedCell.Horizontal, SelectedCell.Vertical, _destinationCell.CellIndex.Horizontal, _destinationCell.CellIndex.Vertical);
    }

    IEnumerator MoveCharacter()
    {
        int Distance = 0;
        float Timer = gridConfig.CharacterMovementSpeed;

        while(Distance < CellDistance)
        {
            while(Timer >= 0)
            {
                Timer -= Time.deltaTime;
                yield return null;
            }

            Timer = gridConfig.CharacterMovementSpeed;

            SelectedCharacter.transform.position = PathPoints[Distance];
            Distance++;
        }

        ActionsContainer.OnCharacterReachedTarget?.Invoke();
        StopCoroutine(Coroutine_MoveCharacter);
    }

    void OnCharacterReachedTarget()
    {
        if (PhotonNetworkManager.Instance.CurrentTurnPlayer == PhotonNetworkManager.Instance.MyIdentity)
        {
            PhotonNetworkManager.Instance.Call_SwitchSides();
        }
        else // Opponent's character reached target
        {
            CustomDataStructures.CellIndex currentCellDataIndex;
            currentCellDataIndex.Horizontal = currentCellIndex_H;
            currentCellDataIndex.Vertical = currentCellIndex_V;
            CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(currentCellDataIndex);

            CustomDataStructures.CellIndex targetCellDataIndex;
            targetCellDataIndex.Horizontal = targetCellIndex_H;
            targetCellDataIndex.Vertical = targetCellIndex_V;
            TargetCellData = GridManager.Instance.GetCellData_From_CellIndex(targetCellDataIndex);
        }

        ActionsContainer.OnCharacterDeSelected(SelectedCharacter.CharacterID);

        CurrentCellData.SetDefaultColor();
        CurrentCellData.isOccupied = false;
        TargetCellData.SetDefaultColor();
        SelectedCharacter.DoOccupyCell(TargetCellData.CellIndex);
        SelectedCharacter.CharacterState = Character.Character_State.Idle;

        UIManager.Instance.gameStatus.SelectedCharacter = None;
        UIManager.Instance.UpdateGameStatsInUI();
    }

    #endregion


    #region RPC Receivers

    void RPC_Receive_MoveCharacterToDestination(int _characterID, Vector3[] _pathPoints, int _cellDistance, int _currentCellIndex_H, int _currentCellIndex_V, int _targetCellIndex_H, int _targetCellIndex_V)
    {
        SelectedCharacter = CharacterManager.Instance.GetCharacterFromCharacterID(_characterID);

        currentCellIndex_H = _currentCellIndex_H;
        currentCellIndex_V = _currentCellIndex_V;
        targetCellIndex_H = _targetCellIndex_H;
        targetCellIndex_V = _targetCellIndex_V;

        CellDistance = _cellDistance;
        PathPoints = _pathPoints;

        Coroutine_MoveCharacter = StartCoroutine(MoveCharacter());
    }

    void RPC_Receive_SwitchSides(PhotonNetworkManager.Player_Identity _currentPlayer)
    {
        if (_currentPlayer == PhotonNetworkManager.Instance.MyIdentity)
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
        }
        else
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OtherPlayerTurn);
        }

        PhotonNetworkManager.Instance.CurrentTurnPlayer = _currentPlayer;
        UIManager.Instance.gameStatus.Update_CurrentPlayer(_currentPlayer);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    void RPC_Receive_GameStart()
    {
        if (PhotonNetworkManager.Instance.MyIdentity == PhotonNetworkManager.Player_Identity.Player1)
        {
            RPCManager.Instance.SendRPC_SideSwitch(PhotonNetworkManager.Player_Identity.Player1);
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
        }
        else
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OtherPlayerTurn);
        }
    }

    #endregion
}