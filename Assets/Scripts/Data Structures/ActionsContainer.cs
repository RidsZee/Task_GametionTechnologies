using System;

public class ActionsContainer
{
    public static Action OnAttack;
    public static Action OnDefend;
    public static Action OnWalk;
    public static Action<Character> OnCharacterSelected;
    public static Action<Character> OnCharacterDeSelected;
    public static Action OnAllCharactersDeSelected;
    public static Action<CustomDataStructures.CellIndex> OnTargetCellSelected;
    public static Action OnCharacterWalkingFinished;
}