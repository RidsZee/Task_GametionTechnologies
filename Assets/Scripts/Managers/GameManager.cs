using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Range(0.5f, 1.0f)]
    public float CharacterRelativeSize = 0.85f;

    [SerializeField]
    GridConfiguration gridConfig;

    int GridLength;
    int GridWidth;

    Character SelectedCharacter;
    CustomDataStructures.CellIndex SelectedCell;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void OnCharacterSelected(Character _character)
    {
        SelectedCharacter = _character;
    }

    void OnAllCharactersDeSelected()
    {
        SelectedCharacter = null;
    }

    void Start()
    {
        if(!gridConfig)
        {
            Debug.LogWarning("Grid config not found in " + name);
            return;
        }

        GridLength = gridConfig.GridLength;
        GridWidth = gridConfig.GridWidth;
    }
}