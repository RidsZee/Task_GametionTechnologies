using UnityEngine;

[CreateAssetMenu(fileName = "Direction List", menuName = "Create New Direction List")]
public class Direction_List : ScriptableObject
{
    public enum ListOfDirections
    {
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight,
        LSpahe_Forward_Long,
        LSpahe_Forward_Short,
        LSpahe_Backward_Long,
        LSpahe_Backward_Short,
        LSpahe_Right_Long,
        LSpahe_Right_Short,
        LSpahe_Left_Long,
        LSpahe_Left_Short
    }

    public ListOfDirections[] Directions;
    public int TotalDirections;
}