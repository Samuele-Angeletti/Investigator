using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWalkPlaceEvidences : MonoBehaviour
{
    [Header("NavMesh Scanning Settings")]
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private float _cellSize = 2.0f;

    [Header("Generation Parameters")]
    [SerializeField] private int _seed = 1337;
    [SerializeField] private int _steps = 100;
    [SerializeField] private int _walkersCount = 1;
    [SerializeField] private Vector2Int _startPos = new Vector2Int(0, 0);
    [SerializeField] private Transform[] _biases;

    [Header("Evidence Instantiation")]
    [SerializeField] private EvidenceNode[] _testEvidenceNodes;

    public List<EvidenceModel> SpawnedEvidences = new List<EvidenceModel>();

    private GenerationResult _latestResult;
    private float _cachedMinX;
    private float _cachedMinZ;

    private void Start()
    {
        Initialize(_seed, _steps, _walkersCount, _startPos, _testEvidenceNodes, _biases);
    }

    public void Initialize(int seed, int steps, int walkersCount, Vector2Int startPos, EvidenceNode[] evidenceNodes, Transform[] biases)
    {
        if (_boxCollider == null) return;

        ClearSpawnedEvidences();

        Bounds bounds = _boxCollider.bounds;
        _cachedMinX = bounds.min.x;
        _cachedMinZ = bounds.min.z;

        int gridWidth = Mathf.FloorToInt(bounds.size.x / _cellSize);
        int gridHeight = Mathf.FloorToInt(bounds.size.z / _cellSize);

        List<(int x, int y)> biasesList = new();
        if (biases != null)
        {
            foreach (var b in biases)
            {
                if (b == null) continue;
                int gX = Mathf.Clamp(Mathf.FloorToInt((b.position.x - _cachedMinX) / _cellSize), 0, gridWidth - 1);
                int gY = Mathf.Clamp(Mathf.FloorToInt((b.position.z - _cachedMinZ) / _cellSize), 0, gridHeight - 1);
                biasesList.Add((gX, gY));
            }
        }

        int localStartX = Mathf.Clamp(Mathf.FloorToInt((startPos.x - _cachedMinX) / _cellSize), 0, gridWidth - 1);
        int localStartY = Mathf.Clamp(Mathf.FloorToInt((startPos.y - _cachedMinZ) / _cellSize), 0, gridHeight - 1);

        // 1. Scan NavMesh to find Exclusions (This automatically excludes BSP areas if they don't have NavMesh)
        List<(int x, int y)> exclusionList = new();
        float centerY = bounds.center.y;
        float extentsY = bounds.extents.y;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float worldX = _cachedMinX + (x * _cellSize) + (_cellSize * 0.5f);
                float worldZ = _cachedMinZ + (y * _cellSize) + (_cellSize * 0.5f);
                Vector3 worldPoint = new Vector3(worldX, centerY, worldZ);

                if (!NavMesh.SamplePosition(worldPoint, out NavMeshHit _, extentsY + 1f, NavMesh.AllAreas))
                {
                    exclusionList.Add((x, y));
                }
            }
        }

        // 2. Run Generator
        RandomWalkSettingsFrancesco randomWalkSettings = new()
        {
            Bounds = (0, 0, gridWidth - 1, gridHeight - 1),
            RandomSeed = false,
            Seed = seed,
            Steps = steps,
            Start = (localStartX, localStartY),
            WalkerCount = walkersCount,
            Biases = biasesList.ToArray(),
            Exclusions = exclusionList.ToArray(),
            PoiBiasIntensity = 8.0f
        };

        RandomWalkGeneratorFrancesco randomWalkGenerator = new(randomWalkSettings);
        _latestResult = randomWalkGenerator.Generate();

        // 3. Populate building references and scatter street nodes
        if (evidenceNodes != null && evidenceNodes.Length > 0)
        {
            PlaceEvidencesAlongPath(_latestResult, evidenceNodes);
        }
    }

    private void PlaceEvidencesAlongPath(GenerationResult result, EvidenceNode[] nodes)
    {
        float centerY = _boxCollider.bounds.center.y;

        // 1. Locate all building areas in the scene
        BSPAreaData[] structuralAreas = FindObjectsByType<BSPAreaData>(FindObjectsSortMode.None);

        // Create a fast lookup set of our assigned biases/POIs
        HashSet<Transform> allowedBiases = new HashSet<Transform>();
        if (_biases != null)
        {
            foreach (var bias in _biases)
            {
                if (bias != null) allowedBiases.Add(bias);
            }
        }

        // CRITICAL FIX: Convert nodes into a Queue so items are uniquely consumed one-by-one
        Queue<EvidenceNode> nodeQueue = new Queue<EvidenceNode>(nodes);
        int cellPadding = 2;

        Debug.Log($"[Generator] Starting placement with {nodeQueue.Count} total evidence nodes.");

        // 2. Assign references ONLY to reached buildings that are part of the biases array
        for (int i = 0; i < structuralAreas.Length; i++)
        {
            if (structuralAreas[i] == null) continue;
            if (nodeQueue.Count == 0) break; // Out of evidence entirely

            if (!allowedBiases.Contains(structuralAreas[i].transform)) continue;

            Bounds obstacleBounds;
            if (structuralAreas[i].TryGetComponent(out NavMeshObstacle obstacle))
            {
                obstacleBounds = new Bounds(structuralAreas[i].transform.position + obstacle.center, obstacle.size);
            }
            else if (structuralAreas[i].TryGetComponent(out Collider col))
            {
                obstacleBounds = col.bounds;
            }
            else
            {
                obstacleBounds = new Bounds(structuralAreas[i].transform.position, new Vector3(_cellSize, 2f, _cellSize));
            }

            int minGridX = Mathf.FloorToInt((obstacleBounds.min.x - _cachedMinX) / _cellSize) - cellPadding;
            int maxGridX = Mathf.FloorToInt((obstacleBounds.max.x - _cachedMinX) / _cellSize) + cellPadding;
            int minGridY = Mathf.FloorToInt((obstacleBounds.min.z - _cachedMinZ) / _cellSize) - cellPadding;
            int maxGridY = Mathf.FloorToInt((obstacleBounds.max.z - _cachedMinZ) / _cellSize) + cellPadding;

            bool poiWasReached = false;

            for (int checkX = minGridX; checkX <= maxGridX; checkX++)
            {
                for (int checkY = minGridY; checkY <= maxGridY; checkY++)
                {
                    if (checkX >= 0 && checkX < result.Width && checkY >= 0 && checkY < result.Height)
                    {
                        if (result.Map[checkX, checkY] == 0)
                        {
                            poiWasReached = true;
                            break;
                        }
                    }
                }
                if (poiWasReached) break;
            }

            if (poiWasReached && structuralAreas[i].pointOfInterest == null)
            {
                // Pull the next available unique node out of the queue permanently
                EvidenceNode targetNode = nodeQueue.Dequeue();
                structuralAreas[i].pointOfInterest = targetNode;

                Debug.Log($"[Generator] Reference assigned to biased building: {structuralAreas[i].name} (Node ID: {targetNode.name})");
            }
        }

        // 3. Gather all valid remaining street coordinates from the walk path
        List<Vector3> validWorldPositions = new List<Vector3>();
        for (int x = 0; x < result.Width; x++)
        {
            for (int y = 0; y < result.Height; y++)
            {
                if (result.Map[x, y] == 0)
                {
                    float wX = _cachedMinX + (x * _cellSize) + (_cellSize * 0.5f);
                    float wZ = _cachedMinZ + (y * _cellSize) + (_cellSize * 0.5f);
                    validWorldPositions.Add(new Vector3(wX, centerY, wZ));
                }
            }
        }

        // 4. Shuffle open street layout spots organically via PRNG Fisher-Yates
        System.Random prng = new System.Random(_seed);
        for (int i = validWorldPositions.Count - 1; i > 0; i--)
        {
            int k = prng.Next(i + 1);
            Vector3 value = validWorldPositions[k];
            validWorldPositions[k] = validWorldPositions[i];
            validWorldPositions[i] = value;
        }

        // 5. Instantiate whatever remaining unassigned nodes are left in the queue out onto the street paths
        int streetSpawnIndex = 0;
        while (nodeQueue.Count > 0)
        {
            EvidenceNode activeNode = nodeQueue.Dequeue();
            if (activeNode == null || activeNode.EvidenceModel == null) continue;

            if (streetSpawnIndex >= validWorldPositions.Count)
            {
                Debug.LogWarning($"[Generator] Ran out of unique street tiles! Could not place: {activeNode.name}");
                break;
            }

            Vector3 spawnPosition = validWorldPositions[streetSpawnIndex];
            streetSpawnIndex++;

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }

            EvidenceModel modelInstance = Instantiate(activeNode.EvidenceModel, spawnPosition, Quaternion.identity);
            modelInstance.Initialize(activeNode);
            SpawnedEvidences.Add(modelInstance);

            Debug.Log($"[Generator] Physically spawned remaining item on street path: {activeNode.name} at {spawnPosition}");
        }

        Debug.Log($"[Generator] Finished placement execution. Street Spawns Total: {SpawnedEvidences.Count}");
    }

    public void ClearSpawnedEvidences()
    {
        foreach (var item in SpawnedEvidences)
        {
            if (item != null) Destroy(item.gameObject);
        }
        SpawnedEvidences.Clear();
    }

    private void OnDrawGizmos()
    {
        if (_latestResult == null || _latestResult.Map == null) return;

        Vector3 cubeDimensions = new Vector3(_cellSize * 0.9f, 0.2f, _cellSize * 0.9f);
        float verticalY = _boxCollider != null ? _boxCollider.bounds.center.y : 0f;

        for (int x = 0; x < _latestResult.Width; x++)
        {
            for (int y = 0; y < _latestResult.Height; y++)
            {
                float worldX = _cachedMinX + (x * _cellSize) + (_cellSize * 0.5f);
                float worldZ = _cachedMinZ + (y * _cellSize) + (_cellSize * 0.5f);
                Vector3 worldPos = new Vector3(worldX, verticalY, worldZ);

                if (_latestResult.Map[x, y] == 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(worldPos, cubeDimensions);
                }
            }
        }
    }
}