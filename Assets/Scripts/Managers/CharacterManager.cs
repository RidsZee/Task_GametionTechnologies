using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Character[] P1Characters;

    [SerializeField]
    Character[] P2Characters;

    int MidCell;
    int HorizontalIndex;
    int CurrentIndex;
    CustomDataStructures.CellIndex currentCellIndex;

    void Awake()
    {
        foreach (Character _character in P1Characters)
        {
            _character.gameObject.SetActive(false);
        }

        foreach (Character _character in P2Characters)
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
        if(!gridConfig || P1Characters.Length == 0 || P2Characters.Length == 0)
        {
            Debug.LogWarning("Required fields not assigned in " + name + " to place characters");
            return;
        }

        MidCell = (gridConfig.GridWidth / 2) - 1;
        
        foreach(Character _character in P1Characters)
        {
            HorizontalIndex = MidCell - 2 + CurrentIndex;
            currentCellIndex.Vertical = 0;
            currentCellIndex.Horizontal = HorizontalIndex;

            _character.gameObject.SetActive(true);
            _character.DoOccupyCell(currentCellIndex);
            _character.transform.position = GridManager.Instance.GetCellData_From_CellIndex(currentCellIndex).transform.position;

            CurrentIndex++;
        }

        CurrentIndex = 0;

        foreach (Character _character in P2Characters)
        {
            HorizontalIndex = MidCell - 2 + CurrentIndex;
            currentCellIndex.Vertical = gridConfig.GridLength - 1;
            currentCellIndex.Horizontal = HorizontalIndex;

            _character.gameObject.SetActive(true);
            _character.DoOccupyCell(currentCellIndex);
            _character.transform.position = GridManager.Instance.GetCellData_From_CellIndex(currentCellIndex).transform.position;

            CurrentIndex++;
        }
    }
}