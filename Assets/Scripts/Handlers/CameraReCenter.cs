using UnityEngine;

public class CameraReCenter : MonoBehaviour
{
    [SerializeField]
    Transform MainCamera;

    [SerializeField]
    GridConfiguration gridConfig;

    [SerializeField]
    Transform CellInitPosition;

    float MaxWidth;
    float MaxLength;
    Vector3 camPosition;

    void Start()
    {
        if(MainCamera && gridConfig && CellInitPosition)
        {
            MaxWidth = gridConfig.GridWidth * gridConfig.CellSize + (gridConfig.GridGap * (gridConfig.GridWidth - 1));
            MaxLength = gridConfig.GridLength * gridConfig.CellSize + (gridConfig.GridGap * (gridConfig.GridLength - 1));

            camPosition = Vector3.zero;

            camPosition.x = (CellInitPosition.position.x + MaxWidth - gridConfig.CellSize) * 0.5f;
            camPosition.y = MainCamera.position.y;
            camPosition.z = (CellInitPosition.position.z + MaxLength - gridConfig.CellSize) * 0.5f;

            MainCamera.position = camPosition;
        }
    }
}
