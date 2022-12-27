using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    LayerMask InputLayerMask;

    [SerializeField]
    float RayDistance;

    RaycastHit HitInfo;

    const string TagCells = "Cell";
    const string TagCharacters = "Character";

    public void OnTouchClick()
    {
        if(Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out HitInfo, RayDistance, InputLayerMask))
        {
            GameObject SelectedGameObject = HitInfo.collider.gameObject;

            if (SelectedGameObject.CompareTag(TagCells))
            {
                CellData cellData = SelectedGameObject.GetComponent<CellData>();

                if(!cellData.isOccupied)
                {
                    ActionsContainer.OnWalk?.Invoke(cellData);
                }
                else
                {
                    Debug.LogWarning("Target cell is occupied");
                }
            }
            else if (SelectedGameObject.CompareTag(TagCharacters))
            {
                Character character = SelectedGameObject.GetComponent<Character>();

                if(character.CharacterState == Character.Character_State.Idle)
                {
                    ActionsContainer.OnCharacterSelected?.Invoke(character);
                    GameStateManager.Instance.GameState = GameStateManager.Game_State.CharacterSelected;
                }
                else if (character.CharacterState == Character.Character_State.Selected)
                {
                    ActionsContainer.OnCharacterDeSelected?.Invoke(character);
                    GameStateManager.Instance.GameState = GameStateManager.Game_State.Idle;
                }

                print(SelectedGameObject.name + " || " + SelectedGameObject.tag);
            }
        }
    }
}