using System.Collections;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Tile Grid Generator")]

    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Transform TileInitPosition;
    
    // Determines how many tiles to generate per frame
    [Range(5, 50)] 
    public int TilesGenerationPerFrame = 5;

    // The original prefab gameobject
    GameObject TilePrefab;
    // Modified prefab to instantiate for tiles
    GameObject TilePrefabScaled;
    Vector3 TileScale;
    // Holds position to place the next tile
    Vector3 NextPosition;
    int CurrentTilesPerFrame;
    int TotalTiles;
    int CurrentWidth;
    // Pre calculated value <TileWidth + gap> for next tile placement
    int CurrentCell;
    float PositionOffset;
    const string CellPrefabName = "Prefab_Tile";
    const string CellNamePrefix = "Cell_";
    bool isColor1;
    Coroutine coroutineGenerateGrid;

    void Start()
    {
        StartGeneratingGrid();
    }

    void StartGeneratingGrid()
    {
        TilePrefab = Resources.Load(CellPrefabName) as GameObject;

        // Validation checks
        if(!TilePrefab || !TileInitPosition || !gridConfig)
        {
            Debug.LogWarning("Required fields are not assigned in the inspector!");
            return;
        }

        if(TilePrefab && !TilePrefab.GetComponent<CellData>())
        {
            Debug.LogWarning("CellData component not attached to Tile Prefab");
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
        CurrentWidth = gridConfig.GridWidth;
        CurrentCell = 0;
        isColor1 = false;

        gridConfig.TilesList = new CellData[TotalTiles];

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
                if (CurrentWidth == 0)
                {
                    NextPosition.z += PositionOffset;
                    NextPosition.x = TileInitPosition.position.x;

                    CurrentWidth = gridConfig.GridWidth;

                    // Flip the color for starting of new row if Grid Width is set to even number
                    if (gridConfig.AlternateCellColor)
                    {
                        if(gridConfig.GridWidth %2 == 0)
                        {
                            isColor1 = !isColor1;
                        }
                    }
                }

                // Instantiate the scaled tile object at next <TargetPosition>
                GameObject G = Instantiate(TilePrefabScaled, NextPosition, Quaternion.identity, TileInitPosition);
                
                // Initialize CellData from CellData
                CellData _cellData = G.GetComponent<CellData>();
                gridConfig.TilesList[CurrentCell] = _cellData;
                _cellData.SetIndex(CurrentCell);
                _cellData.SetSelectionIndex(0);
                _cellData.DeSelectCell();
                G.name = string.Concat(CellNamePrefix, CurrentCell.ToString());

                // Switch to alternate color if its enabled in Grid Config
                if(gridConfig.AlternateCellColor)
                {
                    if (isColor1)
                        _cellData.SetColor(gridConfig.Color2);
                    else
                        _cellData.SetColor(gridConfig.Color1);

                    isColor1 = !isColor1;
                }

                NextPosition.x += PositionOffset;
                CurrentTilesPerFrame--;
                CurrentWidth--;
                TotalTiles--;
                CurrentCell++;
            }

            DefineTilesPerFrame();

            // Current frame is rendered, wait for the next frame
            yield return null;
        }

        TilePrefabScaled.SetActive(false);
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