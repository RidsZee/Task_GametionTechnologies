using System;
using UnityEngine;

public class ActionsContainer
{
    public static Action OnAttack;
    public static Action OnDefend;
    public static Action OnWalk;
    public static Action OnPathDrawn;
    public static Action<CharacterProperties.Character_Type> OnCharacterSelected;
    public static Action<int> OnTargetCellSelected;
}