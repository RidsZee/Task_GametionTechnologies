using UnityEngine;

public class CellData : MonoBehaviour
{
    public int CellIndex;
    public int SelectionIndex;
    public bool isOccupied;
    public bool isSelected;

    [SerializeField] Renderer MyRenderer;

    MaterialPropertyBlock _PropertyBlock;
    const string _ColorPropertyName = "_Color";
    Color DefaultColor;

    void Awake()
    {
        if(!MyRenderer)
        {
            GetComponent<Renderer>();
        }

        _PropertyBlock = new MaterialPropertyBlock();
    }

    public void SelectCell()
    {
        isSelected = true;
    }

    public void DeSelectCell()
    {
        isSelected = false;
    }

    public void SetIndex(int _index)
    {
        CellIndex = _index;
    }

    public void SetSelectionIndex(int _selectionIndex)
    {
        SelectionIndex = _selectionIndex;
    }

    public void SetColor(Color _col)
    {
        if(MyRenderer)
        {
            DefaultColor = _col;

            MyRenderer.GetPropertyBlock(_PropertyBlock);
            _PropertyBlock.SetColor(_ColorPropertyName, _col);
            MyRenderer.SetPropertyBlock(_PropertyBlock);
        }
    }

    public void SetDefaultColor()
    {
        if(MyRenderer)
        {
            MyRenderer.GetPropertyBlock(_PropertyBlock);
            _PropertyBlock.SetColor(_ColorPropertyName, DefaultColor);
            MyRenderer.SetPropertyBlock(_PropertyBlock);
        }
    }
}