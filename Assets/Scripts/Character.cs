using UnityEngine;

public class Character : MonoBehaviour, IPlayerAttack, IPlayerDefend, IPlayerWalk
{
    public CharacterProperties characterProperties;

    void Start()
    {

    }

    public void OnCharacterSelected()
    {
        if (characterProperties)
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
        }
    }

    public void OnCharacterDeselected()
    {
        if (characterProperties)
        {
            if(characterProperties.doAttack)
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