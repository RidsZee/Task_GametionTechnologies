using UnityEngine;

public class CellData : MonoBehaviour
{
    public int CellIndex;
    public int SelectionIndex;
    public bool isOccupied;
    public bool isSelected;

    [SerializeField] Renderer MyRenderer;

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
}