using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Character[] Characters;

    int MidCell;
    int HorizontalIndex;
    int CurrentIndex;
    CustomDataStructures.CellIndex currentCellIndex;

    void Awake()
    {
        foreach (Character _character in Characters)
        {
            _character.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        ActionsContainer.OnGridGenerationCompleted += OnGridGenerationCompleted;
    }

    void OnDisable()
    {
        ActionsContainer.OnGridGenerationCompleted -= OnGridGenerationCompleted;
    }

    void OnGridGenerationCompleted()
    {
        if(!gridConfig || Characters.Length == 0)
        {
            Debug.LogWarning("Required fields not assigned in " + name + " to place characters");
            return;
        }

        MidCell = (gridConfig.GridWidth / 2) - 1;
        
        foreach(Character _character in Characters)
        {
            HorizontalIndex = MidCell - 2 + CurrentIndex;
            currentCellIndex.Vertical = 0;
            currentCellIndex.Horizontal = HorizontalIndex;

            _character.gameObject.SetActive(true);
            _character.DoOccupyCell(currentCellIndex);
            _character.transform.position = GridManager.Instance.GetCellData_From_CellIndex(currentCellIndex).transform.position;

            CurrentIndex++;
        }
    }
}