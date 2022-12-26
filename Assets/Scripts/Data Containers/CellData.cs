using UnityEngine;

public class CellData : MonoBehaviour
{
    public CustomDataStructures.CellIndex CellIndex;
    public bool isOccupied;
    public bool isSelected;

    [SerializeField] Renderer MyRenderer;

    Material DefaultMaterial;

    public DirectionList.ListOfDirections PreferredDirection;

    void Awake()
    {
        if(!MyRenderer)
        {
            GetComponent<Renderer>();
        }
    }

    public void SetIndex(int _indexHorizontal, int _indexVertical)
    {
        CellIndex.Horizontal = _indexHorizontal;
        CellIndex.Vertical = _indexVertical;
    }

    public void SetColor(Material _mat)
    {
        if(MyRenderer)
        {
            DefaultMaterial = _mat;

            MyRenderer.material = _mat;
        }
    }

    public void SetDefaultColor()
    {
        if(MyRenderer)
        {
            MyRenderer.material = DefaultMaterial;
        }
    }
}