using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

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

    const string Warning1 = "Required fields not assigned in <CharacterManager> to place characters";

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

        Instance = this;
    }

    void OnEnable()
    {
        ActionsContainer.OnGridGenerationCompleted += OnGridGenerationCompleted;
        ActionsContainer.OnIdentitySet += OnPlayerIdentitySet;
    }

    void OnDisable()
    {
        ActionsContainer.OnGridGenerationCompleted -= OnGridGenerationCompleted;
        ActionsContainer.OnIdentitySet -= OnPlayerIdentitySet;
    }

    void OnGridGenerationCompleted()
    {
        if(!gridConfig || P1Characters.Length == 0 || P2Characters.Length == 0)
        {
            UIManager.Instance.ShowWarning(Warning1);
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

    void OnPlayerIdentitySet(PhotonNetworkManager.Player_Identity _identity)
    {
        if (_identity == PhotonNetworkManager.Player_Identity.Player1)
        {
            foreach (Character _character in P1Characters)
            {
                _character.CharacterCollider.enabled = true;
            }

            foreach (Character _character in P2Characters)
            {
                _character.CharacterCollider.enabled = false;
            }
        }
        else if (_identity == PhotonNetworkManager.Player_Identity.Player2)
        {
            foreach (Character _character in P1Characters)
            {
                _character.CharacterCollider.enabled = false;
            }

            foreach (Character _character in P2Characters)
            {
                _character.CharacterCollider.enabled = true;
            }
        }    
    }

    public Character GetCharacterFromCharacterID(int _characterID)
    {
        if(_characterID <= 5)
        {
            return P1Characters[_characterID];
        }
        else
        {
            return P1Characters[_characterID - 6];
        }
    }
}