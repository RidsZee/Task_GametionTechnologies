using UnityEngine;

public class CellData : MonoBehaviour
{
    public int CellIndex_Horizontal;
    public int CellIndex_Vertical;
    public int SelectionIndex;
    public bool isOccupied;
    public bool isSelected;

    [SerializeField] Renderer MyRenderer;

    MaterialPropertyBlock _PropertyBlock;
    const string _ColorPropertyName = "_Color";
    Material DefaultMaterial;

    public Direction_List.ListOfDirections PreferredDirection;

    void Awake()
    {
        if(!MyRenderer)
        {
            GetComponent<Renderer>();
        }

        //_PropertyBlock = new MaterialPropertyBlock();
    }

    public void SelectCell()
    {
        isSelected = true;
    }

    public void DeSelectCell()
    {
        isSelected = false;
    }

    public void SetIndex(int _indexHorizontal, int _indexVertical)
    {
        CellIndex_Horizontal = _indexHorizontal;
        CellIndex_Vertical = _indexVertical;
    }

    public void SetSelectionIndex(int _selectionIndex)
    {
        SelectionIndex = _selectionIndex;
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