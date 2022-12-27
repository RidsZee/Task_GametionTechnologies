using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Create New Character")]
public class CharacterProperties : ScriptableObject
{
    [System.Serializable]

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
        Forward,
        Backward,
        Horizontal,
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
    
    public bool doAttack;
    public bool doDefend;
    public bool doWalk;

    public Movement_Type[] Movements;
    public Maximum_Steps MaxSteps;
    public Steps_Count StepsCount;
}