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

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected += OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking += OnCharacterWalkStarted;
        ActionsContainer.OnCharacterReachedTarget += OnCharacterReachedTarget;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnAllCharactersDeSelected -= OnAllCharactersDeSelected;
        ActionsContainer.OnBeginWalking -= OnCharacterWalkStarted;
        ActionsContainer.OnCharacterReachedTarget -= OnCharacterReachedTarget;
    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
        SelectedCell = _character.CurrentCell;
        CurrentCellData = GridManager.Instance.GetCellData_From_CellIndex(SelectedCell);

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
        TargetCellData = _cellData;

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
                TargetCellData.SetCorrectColor(gridConfig.MatCorrectCell);
                MoveCharacterToDestination(_cellData);
            }
            else
            {
                TargetCellData.SetWrongColor(gridConfig.MatWrongCell, gridConfig.HighlightWrongCell);
                Debug.LogWarning("Destination is out of character's reach | Character's max steps : " + CharacterMaxSteps + ", Input distance : " + CellDistance);
            }
        }
        else
        {
            TargetCellData.SetWrongColor(gridConfig.MatWrongCell, gridConfig.HighlightWrongCell);
            Debug.LogWarning("Movement does not match | Current input movement : " + InputDetectedDirection);
        }
    }

    void MoveCharacterToDestination(CellData _destinationCell)
    {
        GameStateManager.Instance.GameState = GameStateManager.Game_State.MovingCharacter;
        print("Wohoooo!!! Moving character to the destination");

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
        GameStateManager.Instance.GameState = GameStateManager.Game_State.Idle;
    }
}