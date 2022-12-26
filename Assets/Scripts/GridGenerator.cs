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
    int CurrentTile;
    float PositionOffset;
    const string TilePrefabName = "Prefab_Tile";

    void Start()
    {
        TilePrefab = Resources.Load(TilePrefabName) as GameObject;

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

        gridConfig.TilesList = new CellData[TotalTiles];

        // Set start position of the 1st tile
        NextPosition = TileInitPosition.position;

        AssignTileCache();
        StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid()
    {
        while (TotalTiles > 0)
        {
            // Generate <CurrentTilesPerFrame> number of tiles per frame
            while (CurrentTilesPerFrame > 0)
            {
                // Current row is completed, calculate values for next row.
                if(CurrentWidth == 0)
                {
                    NextPosition.z += PositionOffset;
                    NextPosition.x = TileInitPosition.position.x;

                    CurrentWidth = gridConfig.GridWidth;
                }

                // Instantiate the scaled tile object and place it to the next <TargetPosition>.
                GameObject G = Instantiate(TilePrefabScaled);
                G.transform.position = NextPosition;
                
                // Initialize CellData from CellData
                CellData _cellData = G.GetComponent<CellData>();
                gridConfig.TilesList[CurrentTile] = _cellData;
                _cellData.SetIndex(CurrentTile);
                _cellData.SetSelectionIndex(0);
                _cellData.DeSelectCell();

                NextPosition.x += PositionOffset;
                CurrentTilesPerFrame--;
                CurrentWidth--;
                TotalTiles--;
                CurrentTile++;
            }

            AssignTileCache();

            print(CurrentTile);

            // Current frame is rendered, wait for the next frame
            yield return null;
        }

        TilePrefabScaled.SetActive(false);
    }

    // Define required Tiles-Generation-Per-Frame
    void AssignTileCache()
    {
        CurrentTilesPerFrame = TilesGenerationPerFrame;

        if (CurrentTilesPerFrame > TotalTiles)
        {
            CurrentTilesPerFrame = TotalTiles;
        }
    }
}