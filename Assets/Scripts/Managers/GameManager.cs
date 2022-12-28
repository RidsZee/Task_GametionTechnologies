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
        ActionsContainer.OnGameStart += On_RPCReceived_GameStart;
        ActionsContainer.OnPlayerSideSwitch += On_RPCReceived_SwitchSides;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected -= OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking -= OnCharacterWalkStarted;
        ActionsContainer.OnCharacterReachedTarget -= OnCharacterReachedTarget;
        ActionsContainer.OnGameStart -= On_RPCReceived_GameStart;
        ActionsContainer.OnPlayerSideSwitch -= On_RPCReceived_SwitchSides;
    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
        SelectedCell = _character.CurrentCell;
        CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(SelectedCell);

        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetHighlightColor(gridConfig.MatHighlight);
        UIManager.Instance.gameStatus.Update_SelectedCharacter(_character.characterProperties.CharacterType);
    }

    void OnAllCharactersDeSelected()
    {
        SelectedCharacter = null;
        GridManager.Instance.GetCellData_From_CellIndex(SelectedCell).SetDefaultColor();
        UIManager.Instance.gameStatus.SelectedCharacter = None;
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

    void MoveCharacterToDestination(CellData _destinationCell)
    {
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.MovingCharacter);

        PathPoints = GridManager.Instance.GetPathPoints(SelectedCell, _destinationCell.CellIndex, InputDetectedDirection, CellDistance);

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

        ActionsContainer.OnCharacterReachedTarget?.Invoke();
    }

    void OnCharacterReachedTarget()
    {
        CurrentCellData.SetDefaultColor();
        CurrentCellData.isOccupied = false;
        TargetCellData.SetDefaultColor();
        SelectedCharacter.DoOccupyCell(TargetCellData.CellIndex);
        SelectedCharacter.CharacterState = Character.Character_State.Idle;

        if(PhotonNetworkManager.Instance.isMaster)
        {
            PhotonNetworkManager.Instance.Call_SwitchSides();
        }
    }

    void On_RPCReceived_SwitchSides(PhotonNetworkManager.Player_Identity CurrentPlayer)
    {
        if(CurrentPlayer == PhotonNetworkManager.Instance.MyIdentity)
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
        }
        else
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OtherPlayerTurn);
        }

        UIManager.Instance.gameStatus.Update_CurrentPlayer(CurrentPlayer);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    void On_RPCReceived_GameStart()
    {
        if (PhotonNetworkManager.Instance.MyIdentity == PhotonNetworkManager.Player_Identity.Player1)
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
        }
        else
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OtherPlayerTurn);
        }
    }
}