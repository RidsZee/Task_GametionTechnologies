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
        ActionsContainer.OnSetDefaultsAfterMovement += RPC_Receive_SetDefaultsAfterCharacterReached;
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
        ActionsContainer.OnSetDefaultsAfterMovement -= RPC_Receive_SetDefaultsAfterCharacterReached;
    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
        SelectedCell = _character.CurrentCell;
        CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(SelectedCell);

        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetHighlightColor(gridConfig.MatHighlight);
        UIManager.Instance.gameStatus.Update_SelectedCharacter(_character.characterProperties.CharacterType);
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

        RPCManager.Instance.SendRPC_SyncPlayerMovement(SelectedCharacter.CharacterID, PathPoints, CellDistance);

        StartCoroutine(MoveCharacter());
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

        if (PhotonNetworkManager.Instance.CurrentTurnPlayer == PhotonNetworkManager.Instance.MyIdentity)
        {
            ActionsContainer.OnCharacterReachedTarget?.Invoke();
        }
    }

    void OnCharacterReachedTarget()
    {
        RPCManager.Instance.SendRPC_SetDefaultsAfterCharacterMovement(CurrentCellData.CellIndex.Horizontal, CurrentCellData.CellIndex.Vertical, TargetCellData.CellIndex.Horizontal, TargetCellData.CellIndex.Vertical, SelectedCharacter.CharacterID);

        CurrentCellData.SetDefaultColor();
        CurrentCellData.isOccupied = false;
        TargetCellData.SetDefaultColor();
        SelectedCharacter.DoOccupyCell(TargetCellData.CellIndex);
        SelectedCharacter.CharacterState = Character.Character_State.Idle;

        SelectedCharacter = null;
        UIManager.Instance.gameStatus.SelectedCharacter = None;
        UIManager.Instance.UpdateGameStatsInUI();

        if (PhotonNetworkManager.Instance.CurrentTurnPlayer == PhotonNetworkManager.Instance.MyIdentity)
        {
            PhotonNetworkManager.Instance.Call_SwitchSides();
        }
    }

    #endregion


    #region RPC Receivers

    void RPC_Receive_MoveCharacterToDestination(int _characterID, Vector3[] _pathPoints, int _cellDistance)
    {
        SelectedCharacter = CharacterManager.Instance.GetCharacterFromCharacterID(_characterID);
        CellDistance = _cellDistance;
        PathPoints = _pathPoints;

        StartCoroutine(MoveCharacter());
    }

    void RPC_Receive_SetDefaultsAfterCharacterReached(int _currentCellIndex_H, int _currentCellIndex_V, int _targetCellIndex_H, int _targetCellIndex_V, int _characterIndex)
    {
        CustomDataStructures.CellIndex currentCellDataIndex;
        currentCellDataIndex.Horizontal = _currentCellIndex_H;
        currentCellDataIndex.Vertical = _currentCellIndex_V;

        CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(currentCellDataIndex);
        CurrentCellData.SetDefaultColor();
        CurrentCellData.isOccupied = false;

        CustomDataStructures.CellIndex targetCellDataIndex;
        targetCellDataIndex.Horizontal = _targetCellIndex_H;
        targetCellDataIndex.Vertical = _targetCellIndex_V;

        TargetCellData = GridManager.Instance.GetCellData_From_CellIndex(targetCellDataIndex);
        TargetCellData.SetDefaultColor();
        
        SelectedCharacter = CharacterManager.Instance.GetCharacterFromCharacterID(_characterIndex);
        SelectedCharacter.DoOccupyCell(TargetCellData.CellIndex);
        SelectedCharacter.CharacterState = Character.Character_State.Idle;
        SelectedCharacter = null;
    }

    void RPC_Receive_SwitchSides(PhotonNetworkManager.Player_Identity _currentPlayer)
    {
        if(_currentPlayer == PhotonNetworkManager.Instance.MyIdentity)
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
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
            PhotonNetworkManager.Instance.CurrentTurnPlayer = PhotonNetworkManager.Player_Identity.Player1;
        }
        else
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OtherPlayerTurn);
            PhotonNetworkManager.Instance.CurrentTurnPlayer = PhotonNetworkManager.Player_Identity.Player2;
        }
    }

    #endregion
}