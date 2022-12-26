using UnityEngine;

[CreateAssetMenu(fileName = "Grid Configuration", menuName = "Grid Configuration")]
public class GridConfiguration : ScriptableObject
{
    [Range(5, 50)]
    public int GridWidth = 5;

    [Range(5, 50)]
    public int GridLength = 5;

    [Range(0.5f, 2.0f)]
    public float TileSize = 0.5f;

    [Range(0, 1.0f)]
    public float GridGap = 0.1f;

    public bool AlternateCellColor;
    public Material Mat1;
    public Material Mat2;
    public Color HighlightColor;
}