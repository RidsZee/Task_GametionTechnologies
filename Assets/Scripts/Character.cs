using UnityEngine;

public class Character : MonoBehaviour, IPlayerAttack, IPlayerDefend, IPlayerWalk
{
    public CharacterProperties characterProperties;
    public GridConfiguration gridConfig;

    void OnEnable()
    {
        ActionsContainer.OnCharacterSelected += OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected += OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected += AllCharactersDeSelected;
    }

    void OnDisable()
    {
        ActionsContainer.OnCharacterSelected -= OnCharacterSelected;
        ActionsContainer.OnCharacterDeSelected -= OnCharacterDeselected;
        ActionsContainer.OnAllCharactersDeSelected -= AllCharactersDeSelected;
    }

    void Start()
    {
        Vector3 mySize = Vector3.one * (gridConfig.TileSize * GameManager.Instance.CharacterRelativeSize);
        transform.localScale = mySize;
    }

    void AllCharactersDeSelected()
    {
        OnCharacterDeselected(this);
    }

    void OnCharacterSelected(Character _character)
    {
        if (characterProperties)
        {
            if(_character == this)
            {
                if (characterProperties.doAttack)
                {
                    ActionsContainer.OnAttack += DoAttack;
                }

                if (characterProperties.doDefend)
                {
                    ActionsContainer.OnDefend += DoDefend;
                }

                if (characterProperties.doWalk)
                {
                    ActionsContainer.OnWalk += DoWalk;
                }

                characterProperties.CharacterState = CharacterProperties.Character_State.Selected;
            }
            else
            {
                OnCharacterDeselected(this);
            }
        }
    }

    void OnCharacterDeselected(Character _character)
    {
        if (characterProperties)
        {
            if(_character == this && characterProperties.CharacterState == CharacterProperties.Character_State.Selected)
            {
                if (characterProperties.doAttack)
                {
                    ActionsContainer.OnAttack -= DoAttack;
                }

                if (characterProperties.doDefend)
                {
                    ActionsContainer.OnDefend -= DoDefend;
                }

                if (characterProperties.doWalk)
                {
                    ActionsContainer.OnWalk -= DoWalk;
                }

                characterProperties.CharacterState = CharacterProperties.Character_State.Idle;
            }
        }
    }


    public void DoAttack()
    {
        // Define attack mechanism
    }

    public void DoDefend()
    {
        // Define defence mechanism
    }

    public void DoWalk()
    {
        
    }
}