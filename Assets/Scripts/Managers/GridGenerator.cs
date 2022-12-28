using System.Collections;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Cell Generator")]

    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Transform CellInitPosition;
    
    [Range(5, 50)] 
    public int CellsGenerationPerFrame = 5; // Determines how many Cells to generate per frame
    
    GameObject CellPrefab; // The original prefab gameobject
    GameObject CellPrefabScaled; // Modified prefab to instantiate for Cells
    Vector3 CellScale;
    Vector3 NextPosition; // Holds position to place the next Cell
    
    int CurrentCellsPerFrame;
    int TotalCells;
    int CurrentWidth;
    int CurrentCell; // Pre calculated value <CellWidth + gap> for next Cell placement
    int CurrentLength;
    float PositionOffset;
    bool isColor1;
    
    const string CellPrefabName = "Prefab_Cell";
    const string CellNamePrefix = "Cell_";
    const string WarningReferences = "Required fields are not assigned in the inspector!";
    const string WarningCellData = "CellData component not attached to Cell Prefab";

    Coroutine coroutineGenerateGrid;

    void Start()
    {
        StartGeneratingGrid();
    }

    void StartGeneratingGrid()
    {
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.GeneratingGrid);

        CellPrefab = Resources.Load(CellPrefabName) as GameObject;

        // Validation checks
        if(!CellPrefab || !CellInitPosition || !gridConfig || !GridManager.Instance.cellDataContainer)
        {
            UIManager.Instance.ShowWarning(WarningReferences);
            return;
        }

        if(CellPrefab && !CellPrefab.GetComponent<CellData>())
        {
            UIManager.Instance.ShowWarning(WarningCellData);
            return;
        }

        PositionOffset = gridConfig.CellSize + gridConfig.GridGap;

        // Scale the Cell prefab according to the config and spawn scaled Cells for the grid
        CellPrefabScaled = Instantiate(CellPrefab);
        CellScale.x = gridConfig.CellSize;
        CellScale.y = CellPrefabScaled.transform.localScale.y;
        CellScale.z = gridConfig.CellSize;
        CellPrefabScaled.transform.localScale = CellScale;

        TotalCells = gridConfig.GridWidth * gridConfig.GridLength;
        CurrentLength = 0;
        CurrentWidth = 0;
        CurrentCell = 0;
        isColor1 = false;

        // Store the reference of all Cell's <CellData> component
        GridManager.Instance.cellDataContainer.AllCells = new CellData[TotalCells];

        // Set start position of the 1st Cell
        NextPosition = CellInitPosition.position;

        DefineCellsPerFrame();
        coroutineGenerateGrid = StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid()
    {
        while (TotalCells > 0)
        {
            // Generate <CurrentCellsPerFrame> number of Cells per frame
            while (CurrentCellsPerFrame > 0)
            {
                // Current row is completed, calculate values for next row.
                if (CurrentWidth == gridConfig.GridWidth)
                {
                    NextPosition.z += PositionOffset;
                    NextPosition.x = CellInitPosition.position.x;
                    CurrentLength++;

                    CurrentWidth = 0;

                    // Flip the color for starting of new row if Grid Width is set to even number
                    if (gridConfig.AlternateCellColor)
                    {
                        if(gridConfig.GridWidth % 2 == 0)
                        {
                            isColor1 = !isColor1;
                        }
                    }
                }

                // Instantiate the scaled Cell object at next <TargetPosition>
                GameObject G = Instantiate(CellPrefabScaled, NextPosition, Quaternion.identity, CellInitPosition);
                
                // Initialize CellData values
                CellData _cellData = G.GetComponent<CellData>();
                GridManager.Instance.cellDataContainer.AllCells[CurrentCell] = _cellData;
                _cellData.SetIndex(CurrentWidth, CurrentLength);
                G.name = string.Concat(CellNamePrefix, CurrentCell.ToString());

                // Switch to alternate color if its enabled in Grid Config
                if(gridConfig.AlternateCellColor)
                {
                    if (isColor1)
                        _cellData.SetGridColor(gridConfig.Mat1);
                    else
                        _cellData.SetGridColor(gridConfig.Mat2);

                    isColor1 = !isColor1;
                }

                NextPosition.x += PositionOffset;
                CurrentCellsPerFrame--;
                CurrentWidth++;
                TotalCells--;
                CurrentCell++;
            }

            DefineCellsPerFrame();

            // Current frame is rendered, wait for the next frame
            yield return null;
        }

        CellPrefabScaled.SetActive(false);
        ActionsContainer.OnGridGenerationCompleted?.Invoke();
        PhotonNetworkManager.Instance.Call_ConnectToServer();
    }

    // Define required Cells-Generation-Per-Frame
    void DefineCellsPerFrame()
    {
        CurrentCellsPerFrame = CellsGenerationPerFrame;

        if (CurrentCellsPerFrame > TotalCells)
        {
            CurrentCellsPerFrame = TotalCells;
        }
    }
}