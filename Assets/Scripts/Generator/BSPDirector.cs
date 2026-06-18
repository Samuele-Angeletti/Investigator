using UnityEngine;

public class BSPDirector : MonoBehaviour
{
    [Header("Simple Walker Settings")]
    [SerializeField] int gridDimension;
    [SerializeField] int attempts;

    [SerializeField] MeshProceduralGenerator meshGenerator;
    bool[,] grid;

    GenerationResult generationResult;

    [Header("BSP Settings")]
    [SerializeField] int bspWidth = 50;
    [SerializeField] int bspHeight = 50;
    [SerializeField] int bspMinPartitionWidth = 5;
    [SerializeField] int bspMinPartitionHeight = 5;
    [SerializeField] int bspRoomPadding = 1;
    [SerializeField] int bspSeed = 1;
    [SerializeField] bool bspRandomSeed = false;

    [ContextMenu("Debug_BSP")]
    public void GenerateBSPDebug()
    {
        generationResult = GenerateBSP();
        meshGenerator.Generate(generationResult);
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
}
