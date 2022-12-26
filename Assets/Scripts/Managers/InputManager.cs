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
                    ActionsContainer.OnTargetCellSelected?.Invoke(cellData.CellIndex);
                }
            }
            else if (SelectedGameObject.CompareTag(TagCharacters))
            {
                Character character = SelectedGameObject.GetComponent<Character>();

                if(character.characterProperties.CharacterState == CharacterProperties.Character_State.Idle)
                {
                    ActionsContainer.OnCharacterSelected?.Invoke(character);
                }
                else if (character.characterProperties.CharacterState == CharacterProperties.Character_State.Selected)
                {
                    ActionsContainer.OnCharacterDeSelected?.Invoke(character);
                }

                print(SelectedGameObject.name + " || " + SelectedGameObject.tag);
            }
        }
    }
}