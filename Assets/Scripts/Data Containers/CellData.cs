using UnityEngine;
using System.Collections;

public class CellData : MonoBehaviour
{
    public CustomDataStructures.CellIndex CellIndex;
    public bool isOccupied;
    
    [SerializeField] Renderer MyRenderer;

    Material DefaultMaterial;

    [HideInInspector]
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

    public void SetWrongColor(Material _mat, float _timer)
    {
        if (MyRenderer)
        {
            MyRenderer.material = _mat;

            StartCoroutine(HighlightWrongCell(_mat, _timer));
        }
    }

    public void SetCorrectColor(Material _mat)
    {
        if (MyRenderer)
        {
            MyRenderer.material = _mat;
        }
    }

    IEnumerator HighlightWrongCell(Material _mat, float _timer)
    {
        float currentTimer = _timer;

        while(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;

            yield return null;
        }

        SetDefaultColor();
    }
}