using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Create New Character")]
public class CharacterProperties : ScriptableObject
{
    public enum Character_Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    [HideInInspector] public Vector3 CurrentTileIndex;
    [HideInInspector] public Vector3 TargetTileIndex;
    [HideInInspector] public Vector3 TargetPosition;

    public Character_Type CharacterType;
    public MovementProperties MovementData;

    public bool doAttack;
    public bool doDefend;
    public bool doWalk;

    public void OnnReached_Destination()
    {
        CurrentTileIndex = TargetTileIndex;
    }
}