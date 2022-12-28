/// <Sumery>
/// This class is responsible for:
/// 1. Detecting user input and raising further Action Events to start the flow
/// </Summery>

using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Variables

    [SerializeField] Camera MainCamera;
    [SerializeField] LayerMask InputLayerMask;
    [SerializeField] float RayDistance;

    RaycastHit HitInfo;

    const string TagCells = "Cell";
    const string TagCharacters = "Character";
    const string TargetCell = "Target cell is occupied";

    #endregion


    #region Input Detection

    public void OnTouchClick()
    {
        if(GameStateManager.Instance.GameState == GameStateManager.Game_State.Idle || GameStateManager.Instance.GameState == GameStateManager.Game_State.CharacterSelected)
        {
            if (Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out HitInfo, RayDistance, InputLayerMask))
            {
                GameObject SelectedGameObject = HitInfo.collider.gameObject;

                if (SelectedGameObject.CompareTag(TagCells))
                {
                    if(GameStateManager.Instance.GameState == GameStateManager.Game_State.CharacterSelected)
                    {
                        CellData cellData = SelectedGameObject.GetComponent<CellData>();

                        if (!cellData.isOccupied)
                        {
                            ActionsContainer.OnWalk?.Invoke(cellData);
                        }
                        else
                        {
                            UIManager.Instance.ShowWarning(TargetCell);
                        }
                    }
                }
                else if (SelectedGameObject.CompareTag(TagCharacters))
                {
                    Character character = SelectedGameObject.GetComponent<Character>();

                    if (character.CharacterState == Character.Character_State.Idle)
                    {
                        ActionsContainer.OnCharacterSelected?.Invoke(character.CharacterID);
                        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.CharacterSelected);
                    }
                    else if (character.CharacterState == Character.Character_State.Selected)
                    {
                        ActionsContainer.OnCharacterDeSelected?.Invoke(character.CharacterID);
                        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Idle);
                    }
                }
            }
        }
    }

    #endregion
}