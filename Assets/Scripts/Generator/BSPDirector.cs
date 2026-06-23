using UnityEngine;

public class BSPDirector : MonoBehaviour
{
    [Header("Simple Walker Settings")]
    [SerializeField] int gridDimension;
    [SerializeField] int attempts;

    [SerializeField] MeshProceduralGenerator groundMeshGenerator;
    [SerializeField] MeshProceduralGenerator wallMeshGenerator;
    [SerializeField] MeshProceduralGenerator ceilingMeshGenerator;
    bool[,] grid;

    GenerationResult generationResult;

    [Header("BSP Settings")]
    [SerializeField] int bspWidth = 50;
    [SerializeField] int bspHeight = 50;
    [SerializeField] int bspMinPartitionWidth = 5;
    [SerializeField] int bspMinPartitionHeight = 5;
    [SerializeField] int bspRoomPadding = 1;
    [SerializeField] int bspRoomHeight = 5;
    [SerializeField] int bspSeed = 1;
    [SerializeField] bool bspRandomSeed = false;

    [HideInInspector]
    public Vector2Int firstPoint;

    [ContextMenu("Debug_BSP")]
    public void GenerateBSPDebug()
    {
        generationResult = GenerateBSP();
        groundMeshGenerator.Generate(generationResult);
    }
    public GenerationResult GenerateBSP()
    {
        return new BSPGenerator(new BSPSettings
        {
            Width = bspWidth,
            Height = bspHeight,
            MinPartitioningHeight = bspMinPartitionHeight,
            RoomPadding = bspRoomPadding,
            Seed = bspSeed,
            MinPartitioningWidth = bspMinPartitionWidth,
            RandomSeed = bspRandomSeed
        }).Generate();
    }
    public GenerationResult GenerateBSP(BSPSettings _bspSettings)
    {
        return new BSPGenerator(_bspSettings).Generate();
    }
    public void GenerateBSPWithAreaDataa(BSPSettings _bspSettings)
    {
        groundMeshGenerator.gameObject.SetActive(true);
        wallMeshGenerator.gameObject.SetActive(true);
        ceilingMeshGenerator.gameObject.SetActive(true);

        bspWidth = _bspSettings.Width;
        bspHeight = _bspSettings.Height;
        bspMinPartitionWidth = _bspSettings.MinPartitioningWidth;
        bspMinPartitionHeight = _bspSettings.MinPartitioningHeight;
        bspRoomPadding = _bspSettings.RoomPadding;
        bspSeed = _bspSettings.Seed;
        bspRandomSeed = _bspSettings.RandomSeed;

        generationResult = GenerateBSP();
        groundMeshGenerator.Generate(generationResult);
        wallMeshGenerator.Generate(generationResult, false, true, bspRoomHeight);
        ceilingMeshGenerator.transform.position = transform.position + Vector3.up * bspRoomHeight;
        ceilingMeshGenerator.Generate(generationResult, true);

       firstPoint = generationResult.GetFirstCornerBottomLeft();
    }
    public void HideGeneratedArea()
    {
        groundMeshGenerator.gameObject.SetActive(false);
        wallMeshGenerator.gameObject.SetActive(false);
        ceilingMeshGenerator.gameObject.SetActive(false);
    }
}
