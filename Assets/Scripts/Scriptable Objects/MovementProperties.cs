using UnityEngine;

[CreateAssetMenu(fileName = "Movement", menuName = "Create New Movement Properties")]
public class MovementProperties : ScriptableObject
{
    public enum Movement_Type
    {
        Forward,
        LShape,
        Diagonal,
        PlusSign,
        CrossSign,
        PlusAndCrossSign,
        OneStep
    }

    public enum Direction_Type
    {
        OnlyForward = 1,
        FourDirections = 4,
        LShape_8 = 8,
        OmniDirection = 8
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

    public Movement_Type Movement;
    public Direction_Type Direction;
    public Maximum_Steps MaxSteps;
    public Steps_Count StepsCount;
}