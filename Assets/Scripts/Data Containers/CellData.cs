/// <Sumery>
/// This class is responsible for:
/// 1. Holding cell specific data
/// 2. Processing cell specific operations affecing its configuration like color, occupacy, index in grid etc
/// </Summery>

using UnityEngine;
using System.Collections;

public class CellData : MonoBehaviour
{
    #region Variables

    public CustomDataStructures.CellIndex CellIndex;
    public bool isOccupied;

    [SerializeField] Renderer MyRenderer;

    Material DefaultMaterial;
    bool isWrongCellHighlighted;
    Coroutine Coroutine_HighlightWrongCell;

    #endregion


    #region Initialization

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

    #endregion


    #region Update Colors

    public void SetGridColor(Material _mat)
    {
        if(MyRenderer)
        {
            DefaultMaterial = _mat;

            MyRenderer.material = _mat;
        }
    }

    public void SetHighlightColor(Material _mat)
    {
        if (MyRenderer)
        {
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

    public void SetCorrectColor(Material _mat)
    {
        if (MyRenderer)
        {
            MyRenderer.material = _mat;
        }
    }

    #endregion


    #region Highlight Wrong Cell

    public void SetWrongColor(Material _mat, float _timer)
    {
        if (MyRenderer)
        {
            MyRenderer.material = _mat;

            if(isWrongCellHighlighted)
            {
                StopCoroutine(Coroutine_HighlightWrongCell);
            }

            Coroutine_HighlightWrongCell = StartCoroutine(HighlightWrongCell(_mat, _timer));
        }
    }

    IEnumerator HighlightWrongCell(Material _mat, float _timer)
    {
        isWrongCellHighlighted = true;
        float currentTimer = _timer;

        while (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;

            yield return null;
        }

        SetDefaultColor();
        isWrongCellHighlighted = false;
    }

    #endregion
}