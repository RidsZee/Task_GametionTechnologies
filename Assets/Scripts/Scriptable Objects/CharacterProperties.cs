using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Create New Character")]
public class CharacterProperties : ScriptableObject
{
    [System.Serializable]

    public enum Character_State
    {
        Idle,
        Selected,
        Moving
    }

    public enum Character_Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum Movement_Type
    {
        Horizontal,
        Vertical,
        Diagonal,
        LShape,
        None
    }

    public enum Maximum_Steps
    {
        One = 1,
        Two = 2,
        Three = 3,
        Infinite = 0
    }

    public enum Steps_Count
    {
        Fixed,
        Dynamic
    }


    public Character_Type CharacterType;
    //public DirectionList DirectionList;

    public bool doAttack;
    public bool doDefend;
    public bool doWalk;

    public Movement_Type[] Movements;
    public Maximum_Steps MaxSteps;
    public Steps_Count StepsCount;

    public CustomDataStructures.CellIndex CurrentCell;
    public CustomDataStructures.CellIndex TargetCell;
    [HideInInspector] public Vector3 TargetPosition;
    public Character_State CharacterState;

    public void OnReached_Destination()
    {
        CurrentCell = TargetCell;
    }
}