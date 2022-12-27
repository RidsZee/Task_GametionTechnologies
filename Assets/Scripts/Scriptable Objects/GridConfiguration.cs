using UnityEngine;

[CreateAssetMenu(fileName = "Grid Configuration", menuName = "Grid Configuration")]
public class GridConfiguration : ScriptableObject
{
    [Range(6, 50)]
    public int GridWidth = 6;

    [Range(5, 50)]
    public int GridLength = 6;

    [Range(0.5f, 2.0f)]
    public float TileSize = 0.5f;

    [Range(0, 1.0f)]
    public float GridGap = 0.1f;

    [Range(0.2f, 2.0f)]
    public float HighlightWrongCell = 0.5f;

    [Range(0.5f, 1.0f)]
    public float CharacterRelativeSize = 0.85f;

    [Range(0.1f, 1.0f)]
    public float CharacterMovementSpeed = 0.5f;

    public bool AlternateCellColor;
    public Material Mat1;
    public Material Mat2;
    public Material MatHighlight;
    public Material MatCorrectCell;
    public Material MatWrongCell;
}