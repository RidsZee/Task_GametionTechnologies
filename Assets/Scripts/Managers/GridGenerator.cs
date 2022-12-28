using System.Collections;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Tile Grid Generator")]

    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Transform TileInitPosition;
    
    [Range(5, 50)] 
    public int TilesGenerationPerFrame = 5; // Determines how many tiles to generate per frame
    
    GameObject TilePrefab; // The original prefab gameobject
    GameObject TilePrefabScaled; // Modified prefab to instantiate for tiles
    Vector3 TileScale;
    Vector3 NextPosition; // Holds position to place the next tile
    
    int CurrentTilesPerFrame;
    int TotalTiles;
    int CurrentWidth;
    int CurrentCell; // Pre calculated value <TileWidth + gap> for next tile placement
    int CurrentLength;
    float PositionOffset;
    bool isColor1;
    
    const string CellPrefabName = "Prefab_Tile";
    const string CellNamePrefix = "Cell_";
    const string WarningReferences = "Required fields are not assigned in the inspector!";
    const string WarningCellData = "CellData component not attached to Tile Prefab";

    Coroutine coroutineGenerateGrid;

    void Start()
    {
        StartGeneratingGrid();
    }

    void StartGeneratingGrid()
    {
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.GeneratingGrid);

        TilePrefab = Resources.Load(CellPrefabName) as GameObject;

        // Validation checks
        if(!TilePrefab || !TileInitPosition || !gridConfig || !GridManager.Instance.cellDataContainer)
        {
            UIManager.Instance.ShowWarning(WarningReferences);
            return;
        }

        if(TilePrefab && !TilePrefab.GetComponent<CellData>())
        {
            UIManager.Instance.ShowWarning(WarningCellData);
            return;
        }

        PositionOffset = gridConfig.TileSize + gridConfig.GridGap;

        // Scale the tile prefab according to the config and spawn scaled tiles for the grid
        TilePrefabScaled = Instantiate(TilePrefab);
        TileScale.x = gridConfig.TileSize;
        TileScale.y = TilePrefabScaled.transform.localScale.y;
        TileScale.z = gridConfig.TileSize;
        TilePrefabScaled.transform.localScale = TileScale;

        TotalTiles = gridConfig.GridWidth * gridConfig.GridLength;
        CurrentLength = 0;
        CurrentWidth = 0;
        CurrentCell = 0;
        isColor1 = false;

        // Store the reference of all Cell's <CellData> component
        GridManager.Instance.cellDataContainer.AllCells = new CellData[TotalTiles];

        // Set start position of the 1st tile
        NextPosition = TileInitPosition.position;

        DefineTilesPerFrame();
        coroutineGenerateGrid = StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid()
    {
        while (TotalTiles > 0)
        {
            // Generate <CurrentTilesPerFrame> number of tiles per frame
            while (CurrentTilesPerFrame > 0)
            {
                // Current row is completed, calculate values for next row.
                if (CurrentWidth == gridConfig.GridWidth)
                {
                    NextPosition.z += PositionOffset;
                    NextPosition.x = TileInitPosition.position.x;
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

                // Instantiate the scaled tile object at next <TargetPosition>
                GameObject G = Instantiate(TilePrefabScaled, NextPosition, Quaternion.identity, TileInitPosition);
                
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
                CurrentTilesPerFrame--;
                CurrentWidth++;
                TotalTiles--;
                CurrentCell++;
            }

            DefineTilesPerFrame();

            // Current frame is rendered, wait for the next frame
            yield return null;
        }

        TilePrefabScaled.SetActive(false);
        ActionsContainer.OnGridGenerationCompleted?.Invoke();
        PhotonNetworkManager.Instance.Call_ConnectToServer();
    }

    // Define required Tiles-Generation-Per-Frame
    void DefineTilesPerFrame()
    {
        CurrentTilesPerFrame = TilesGenerationPerFrame;

        if (CurrentTilesPerFrame > TotalTiles)
        {
            CurrentTilesPerFrame = TotalTiles;
        }
    }
}