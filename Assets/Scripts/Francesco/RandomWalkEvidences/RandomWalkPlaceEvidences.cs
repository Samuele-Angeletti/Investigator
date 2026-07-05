using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWalkPlaceEvidences : MonoBehaviour
{
    [Header("NavMesh Scanning")]
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private float _cellSize = 1.0f;

    [Header("Generation")]
    [SerializeField] private int _seed = 1337;
    [SerializeField] private int _steps = 2000;
    [SerializeField] private int _walkersCount = 1;
    [SerializeField] private Vector2Int _startPos = new(0, 0);
    [SerializeField] private PointOfInterest[] _biases;

    [Header("Path Shaping")]
    [SerializeField] private float _poiBiasIntensity = 10000f;
    [SerializeField] private float _exploreBias = 0.1f;
    [SerializeField] private float _revisitPenalty = 0.001f;

    [Header("Evidence")]
    [SerializeField] private EvidenceNode[] _testEvidenceNodes;

    [Header("Assignment")]
    [SerializeField] private float _poiAssignmentRadius = 5.0f; // in cells

    [Header("Path Simplification")]
    [SerializeField] private float _simplificationEpsilon = 4.0f;   // cells (Douglas‑Peucker)
    [SerializeField] private float _minPointDistance = 1.5f;        // cells (remove close points)
    [SerializeField] private float _angleThreshold = 25f;           // degrees (keep only sharp turns)

    [Header("Debug")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private bool _showGridWireframe = false;
    [SerializeField] private LineRenderer _lineRenderer;             // <-- NEW

    public List<EvidenceModel> SpawnedEvidences = new();

    [Header("Output")]
    public List<List<Vector3>> WalkerPaths = new();
    public List<Vector3> PrimaryWalkerPath => WalkerPaths.Count > 0 ? WalkerPaths[0] : new();
    public List<Vector3> SimplifiedPath = new();

    private GenerationResult _latestResult;
    private float _cachedMinX, _cachedMinZ;
    private HashSet<(int x, int y)> _cachedExclusions = new();
    private List<PointOfInterest> _cachedAllPois = new();
    private int _gridWidth, _gridHeight;

    private void Start()
    {
        Vector3 center = _boxCollider.bounds.center;
        int gx = Mathf.FloorToInt((center.x - _boxCollider.bounds.min.x) / _cellSize);
        int gy = Mathf.FloorToInt((center.z - _boxCollider.bounds.min.z) / _cellSize);
        Initialize(_seed, _steps, _walkersCount, new Vector2Int(gx, gy), _testEvidenceNodes, _biases);
    }

    // ------------------------------------------------------------------------
    // Original public Initialize (kept for backward compatibility)
    // ------------------------------------------------------------------------
    public void Initialize(int seed, int steps, int walkersCount, Vector2Int startPos,
                           EvidenceNode[] evidenceNodes, PointOfInterest[] biases)
    {
        InitializeFull(seed, steps, walkersCount, startPos,
                       evidenceNodes, biases,
                       _boxCollider, _cellSize,
                       _poiBiasIntensity, _exploreBias, _revisitPenalty,
                       _poiAssignmentRadius);
    }

    // ------------------------------------------------------------------------
    // Full initialisation – all parameters can be passed from outside
    // ------------------------------------------------------------------------
    public void InitializeFull(int seed, int steps, int walkersCount, Vector2Int startPos,
                               EvidenceNode[] evidenceNodes, PointOfInterest[] biases,
                               BoxCollider boxCollider, float cellSize,
                               float poiBiasIntensity, float exploreBias, float revisitPenalty,
                               float poiAssignmentRadius)
    {
        _boxCollider = boxCollider;
        _cellSize = cellSize;
        _seed = seed;
        _steps = steps;
        _walkersCount = walkersCount;
        _startPos = startPos;
        _biases = biases;
        _testEvidenceNodes = evidenceNodes;
        _poiBiasIntensity = poiBiasIntensity;
        _exploreBias = exploreBias;
        _revisitPenalty = revisitPenalty;
        _poiAssignmentRadius = poiAssignmentRadius;

        RunGeneration();
    }

    // ------------------------------------------------------------------------
    // Core generation logic
    // ------------------------------------------------------------------------
    private void RunGeneration()
    {
        if (_boxCollider == null) return;

        ClearSpawnedEvidences();
        _cachedExclusions.Clear();
        _cachedAllPois.Clear();

        Bounds bounds = _boxCollider.bounds;
        _cachedMinX = bounds.min.x;
        _cachedMinZ = bounds.min.z;
        _gridWidth = Mathf.FloorToInt(bounds.size.x / _cellSize);
        _gridHeight = Mathf.FloorToInt(bounds.size.z / _cellSize);

        // Clear POI references
        foreach (var poi in FindObjectsByType<PointOfInterest>(FindObjectsSortMode.None))
            if (poi != null) poi.EvidenceNode = null;

        // 1. Exclusions (NavMesh)
        float centerY = bounds.center.y;
        float halfCell = _cellSize * 0.5f;
        var exclusionList = new List<(int x, int y)>();

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                float wx = _cachedMinX + x * _cellSize + halfCell;
                float wz = _cachedMinZ + y * _cellSize + halfCell;
                Vector3 bl = new(wx - halfCell, centerY, wz - halfCell);
                Vector3 tl = new(wx - halfCell, centerY, wz + halfCell);
                Vector3 br = new(wx + halfCell, centerY, wz - halfCell);
                Vector3 tr = new(wx + halfCell, centerY, wz + halfCell);
                bool ok = NavMesh.SamplePosition(bl, out _, bounds.extents.y + 1f, NavMesh.AllAreas) &&
                          NavMesh.SamplePosition(tl, out _, bounds.extents.y + 1f, NavMesh.AllAreas) &&
                          NavMesh.SamplePosition(br, out _, bounds.extents.y + 1f, NavMesh.AllAreas) &&
                          NavMesh.SamplePosition(tr, out _, bounds.extents.y + 1f, NavMesh.AllAreas);
                if (!ok)
                {
                    exclusionList.Add((x, y));
                    _cachedExclusions.Add((x, y));
                }
            }
        }

        // 2. Collect ALL POIs (regardless of ShouldSpawn) for biasing and assignment.
        var targetPois = new List<PointOfInterest>();
        var targetCoords = new List<(int x, int y)>();

        if (_biases != null)
        {
            foreach (var poi in _biases)
            {
                if (poi == null) continue;
                int gx = Mathf.Clamp(Mathf.FloorToInt((poi.transform.position.x - _cachedMinX) / _cellSize), 0, _gridWidth - 1);
                int gy = Mathf.Clamp(Mathf.FloorToInt((poi.transform.position.z - _cachedMinZ) / _cellSize), 0, _gridHeight - 1);
                targetPois.Add(poi);
                targetCoords.Add((gx, gy));
            }
        }

        _cachedAllPois = targetPois;
        Debug.Log($"Collected {_cachedAllPois.Count} POIs total (regardless of ShouldSpawn).");

        // 3. Ensure start is not on an excluded tile
        int startX = Mathf.Clamp(_startPos.x, 0, _gridWidth - 1);
        int startY = Mathf.Clamp(_startPos.y, 0, _gridHeight - 1);
        if (_cachedExclusions.Contains((startX, startY)))
        {
            bool found = false;
            for (int radius = 1; radius < 20 && !found; radius++)
            {
                for (int dx = -radius; dx <= radius && !found; dx++)
                {
                    for (int dy = -radius; dy <= radius && !found; dy++)
                    {
                        int nx = startX + dx, ny = startY + dy;
                        if (nx >= 0 && nx < _gridWidth && ny >= 0 && ny < _gridHeight &&
                            !_cachedExclusions.Contains((nx, ny)))
                        {
                            startX = nx;
                            startY = ny;
                            found = true;
                        }
                    }
                }
            }
            if (!found)
                Debug.LogError("No walkable start cell found in the grid.");
        }

        // 4. Run generator
        var settings = new RandomWalkSettingsFrancesco
        {
            Bounds = (0, 0, _gridWidth - 1, _gridHeight - 1),
            RandomSeed = false,
            Seed = _seed,
            Steps = _steps,
            Start = (startX, startY),
            WalkerCount = _walkersCount,
            Biases = targetCoords.ToArray(),
            Exclusions = exclusionList.ToArray(),
            PoiBiasIntensity = _poiBiasIntensity,
            ExploreBias = _exploreBias,
            RevisitPenalty = _revisitPenalty
        };

        var generator = new RandomWalkGeneratorFrancesco(settings);
        _latestResult = generator.Generate();

        WalkerPaths = BuildWorldPaths(generator.WalkerPaths, centerY);

        // 5. Aggressive path simplification – using the new parameters
        if (WalkerPaths.Count > 0)
        {
            var simplified = SimplifyPath(WalkerPaths[0], _simplificationEpsilon, _minPointDistance, _angleThreshold);
            SimplifiedPath = simplified;
            Debug.Log($"Path simplified from {WalkerPaths[0].Count} to {SimplifiedPath.Count} points.");

            // Automatically update the LineRenderer if assigned
            UpdateLineRenderer();
        }

        // 6. Place evidences
        PlaceEvidencesAlongPath(_latestResult, _testEvidenceNodes, _cachedAllPois);
    }

    // ------------------------------------------------------------------------
    // AGGRESSIVE PATH SIMPLIFICATION
    // ------------------------------------------------------------------------
    private List<Vector3> SimplifyPath(List<Vector3> path, float epsilon, float minDist, float angleThresholdDeg)
    {
        if (path.Count < 3) return path;

        // 1. Remove points that are too close together
        var filtered = RemoveClosePoints(path, minDist);

        // 2. Douglas‑Peucker with a generous epsilon
        var simplified = DouglasPeucker(filtered, epsilon);

        // 3. Remove near‑collinear points (angle threshold)
        simplified = RemoveCollinearPoints(simplified, angleThresholdDeg);

        return simplified;
    }

    // ------------------------------------------------------------------------
    // Douglas‑Peucker implementation
    // ------------------------------------------------------------------------
    private List<Vector3> DouglasPeucker(List<Vector3> points, float epsilon)
    {
        if (points.Count < 3) return points;

        float maxDist = 0f;
        int index = -1;
        Vector3 first = points[0], last = points[^1];
        float lineLen = Vector3.Distance(first, last);
        if (lineLen < 0.001f) return new List<Vector3> { first, last };

        for (int i = 1; i < points.Count - 1; i++)
        {
            float dist = Mathf.Abs(Vector3.Cross(last - first, points[i] - first).magnitude) / lineLen;
            if (dist > maxDist) { maxDist = dist; index = i; }
        }

        if (maxDist > epsilon)
        {
            var left = DouglasPeucker(points.GetRange(0, index + 1), epsilon);
            var right = DouglasPeucker(points.GetRange(index, points.Count - index), epsilon);
            var result = new List<Vector3>(left);
            result.RemoveAt(result.Count - 1);
            result.AddRange(right);
            return result;
        }
        else
        {
            return new List<Vector3> { first, last };
        }
    }

    // ------------------------------------------------------------------------
    // Remove consecutive points that are too close (minDist in world units)
    // ------------------------------------------------------------------------
    private List<Vector3> RemoveClosePoints(List<Vector3> path, float minDist)
    {
        if (path.Count < 2) return path;
        var result = new List<Vector3> { path[0] };
        for (int i = 1; i < path.Count; i++)
        {
            if (Vector3.Distance(result[^1], path[i]) > minDist)
                result.Add(path[i]);
        }
        if (Vector3.Distance(result[^1], path[^1]) > minDist)
            result.Add(path[^1]);
        return result;
    }

    // ------------------------------------------------------------------------
    // Remove points that are nearly collinear (angle threshold in degrees)
    // ------------------------------------------------------------------------
    private List<Vector3> RemoveCollinearPoints(List<Vector3> path, float angleThresholdDeg)
    {
        if (path.Count < 3) return path;

        float thresholdRad = angleThresholdDeg * Mathf.Deg2Rad;
        var result = new List<Vector3> { path[0] };

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 prev = path[i - 1];
            Vector3 curr = path[i];
            Vector3 next = path[i + 1];

            Vector3 dir1 = (curr - prev).normalized;
            Vector3 dir2 = (next - curr).normalized;

            float dot = Vector3.Dot(dir1, dir2);
            float angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

            if (angle > thresholdRad)
            {
                result.Add(curr);
            }
        }

        result.Add(path[^1]);
        return result;
    }

    // ------------------------------------------------------------------------
    // Public method to get a simplified path with custom parameters (call after generation)
    // ------------------------------------------------------------------------
    public List<Vector3> GetSimplifiedPath(float epsilon = -1f, float minDist = -1f, float angleThreshold = -1f)
    {
        if (WalkerPaths.Count == 0) return new List<Vector3>();

        float e = (epsilon > 0) ? epsilon : _simplificationEpsilon;
        float md = (minDist > 0) ? minDist : _minPointDistance;
        float at = (angleThreshold > 0) ? angleThreshold : _angleThreshold;

        return SimplifyPath(WalkerPaths[0], e, md, at);
    }

    // ------------------------------------------------------------------------
    // World path conversion
    // ------------------------------------------------------------------------
    private List<List<Vector3>> BuildWorldPaths(List<List<(int x, int y)>> localPaths, float centerY)
    {
        float half = _cellSize * 0.5f;
        var world = new List<List<Vector3>>();
        foreach (var local in localPaths)
        {
            var wPath = new List<Vector3>(local.Count);
            foreach (var (x, y) in local)
            {
                float wx = _cachedMinX + x * _cellSize + half;
                float wz = _cachedMinZ + y * _cellSize + half;
                wPath.Add(new Vector3(wx, centerY, wz));
            }
            world.Add(wPath);
        }
        return world;
    }

    // ------------------------------------------------------------------------
    // Evidence placement
    // ------------------------------------------------------------------------
    private void PlaceEvidencesAlongPath(GenerationResult result, EvidenceNode[] nodes, List<PointOfInterest> allPois)
    {
        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogWarning("No EvidenceNodes provided. No evidences will be spawned.");
            return;
        }

        float centerY = _boxCollider.bounds.center.y;
        var queue = new Queue<EvidenceNode>(nodes);
        int totalNodes = queue.Count;

        // Precompute world positions of all visited cells (walkable)
        List<Vector3> visitedWorldPositions = new List<Vector3>();
        float halfCell = _cellSize * 0.5f;
        for (int x = 0; x < result.Width; x++)
        {
            for (int y = 0; y < result.Height; y++)
            {
                if (result.Map[x, y] == 0)
                {
                    float wx = _cachedMinX + x * _cellSize + halfCell;
                    float wz = _cachedMinZ + y * _cellSize + halfCell;
                    visitedWorldPositions.Add(new Vector3(wx, centerY, wz));
                }
            }
        }

        Debug.Log($"Visited walkable tiles: {visitedWorldPositions.Count}");

        if (visitedWorldPositions.Count == 0)
        {
            Debug.LogError("No walkable cells found in the generated map. Evidences will be placed at center.");
            visitedWorldPositions.Add(new Vector3(_boxCollider.bounds.center.x, centerY, _boxCollider.bounds.center.z));
        }

        float assignmentRadiusWorld = _poiAssignmentRadius * _cellSize;
        Debug.Log($"Assignment radius: {assignmentRadiusWorld} world units ({_poiAssignmentRadius} cells)");

        // 1. Assign to ALL POIs if they are within radius of any visited cell (2D distance, ignore Y).
        int assignedCount = 0;

        for (int i = 0; i < allPois.Count && queue.Count > 0; i++)
        {
            var poi = allPois[i];
            if (poi == null) continue;

            Vector3 poiPos = poi.transform.position;
            float minDist = float.MaxValue;
            bool reached = false;

            foreach (var visitedPos in visitedWorldPositions)
            {
                float dx = poiPos.x - visitedPos.x;
                float dz = poiPos.z - visitedPos.z;
                float dist = Mathf.Sqrt(dx * dx + dz * dz);
                if (dist < minDist) minDist = dist;
                if (dist <= assignmentRadiusWorld)
                {
                    reached = true;
                    break;
                }
            }

            if (!reached)
            {
                // Log for debugging (optional)
                // Debug.Log($"POI {poi.name} not reached. Closest visited cell distance: {minDist:F2} (threshold: {assignmentRadiusWorld})");
                continue;
            }

            if (poi.EvidenceNode == null)
            {
                var node = queue.Dequeue();
                poi.EvidenceNode = node;
                assignedCount++;
                Debug.Log($"Assigned {node.name} to POI {poi.name} (distance {minDist:F2} <= {assignmentRadiusWorld})");

                if (poi.ShouldSpawn && node.EvidenceModel != null)
                {
                    Vector3 spawnPos = poiPos;
                    if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                        spawnPos = hit.position;
                    var inst = Instantiate(node.EvidenceModel, spawnPos, Quaternion.identity);
                    inst.Initialize(node);
                    SpawnedEvidences.Add(inst);
                    Debug.Log($"Spawned evidence at POI {poi.name} (ShouldSpawn=true)");
                }
                else if (!poi.ShouldSpawn)
                {
                    Debug.Log($"POI {poi.name} has ShouldSpawn=false; evidence assigned but not spawned here.");
                }
            }
        }

        Debug.Log($"Assigned {assignedCount} out of {totalNodes} EvidenceNodes to POIs.");

        // 2. Spawn remaining evidences on random walkable tiles.
        if (queue.Count > 0)
        {
            var prng = new System.Random(_seed);
            for (int i = visitedWorldPositions.Count - 1; i > 0; i--)
            {
                int k = prng.Next(i + 1);
                (visitedWorldPositions[k], visitedWorldPositions[i]) = (visitedWorldPositions[i], visitedWorldPositions[k]);
            }

            int idx = 0;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node == null || node.EvidenceModel == null)
                {
                    Debug.LogWarning($"Skipping null EvidenceNode or missing EvidenceModel.");
                    continue;
                }

                Vector3 spawn = visitedWorldPositions[idx % visitedWorldPositions.Count];
                idx++;
                if (NavMesh.SamplePosition(spawn, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    spawn = hit.position;

                var inst = Instantiate(node.EvidenceModel, spawn, Quaternion.identity);
                inst.Initialize(node);
                SpawnedEvidences.Add(inst);
            }

            Debug.Log($"Spawned {SpawnedEvidences.Count} total evidences (assigned + random).");
        }
        else
        {
            Debug.Log($"All {totalNodes} EvidenceNodes assigned to POIs.");
        }
    }

    public void ClearSpawnedEvidences()
    {
        foreach (var e in SpawnedEvidences)
            if (e != null) Destroy(e.gameObject);
        SpawnedEvidences.Clear();
    }

    // ------------------------------------------------------------------------
    // LineRenderer handling
    // ------------------------------------------------------------------------

    /// <summary>
    /// Updates the assigned LineRenderer with the current SimplifiedPath.
    /// </summary>
    public void UpdateLineRenderer()
    {
        if (_lineRenderer == null) return;

        if (SimplifiedPath == null || SimplifiedPath.Count == 0)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        _lineRenderer.positionCount = SimplifiedPath.Count;
        _lineRenderer.SetPositions(SimplifiedPath.ToArray());
    }

    /// <summary>
    /// Sets a new LineRenderer and updates it with the current simplified path.
    /// </summary>
    public void SetLineRenderer(LineRenderer lineRenderer)
    {
        _lineRenderer = lineRenderer;
        UpdateLineRenderer();
    }

    // ------------------------------------------------------------------------
    // Public getters for raw path and LineRenderer drawing (with custom params)
    // ------------------------------------------------------------------------
    public List<Vector3> GetWalkerPath(int walkerIndex = 0)
    {
        if (walkerIndex >= 0 && walkerIndex < WalkerPaths.Count)
            return new List<Vector3>(WalkerPaths[walkerIndex]);
        return new List<Vector3>();
    }

    public void DrawPathOnLineRenderer(LineRenderer lineRenderer, int walkerIndex = 0, bool useSimplifiedPath = false,
                                       float customEpsilon = -1f, float customMinDist = -1f, float customAngle = -1f)
    {
        if (lineRenderer == null) return;

        List<Vector3> path;
        if (useSimplifiedPath)
        {
            path = GetSimplifiedPath(customEpsilon, customMinDist, customAngle);
        }
        else
        {
            path = GetWalkerPath(walkerIndex);
        }

        if (path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
    }

    // ------------------------------------------------------------------------
    // Gizmos
    // ------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        if (!_showGizmos || _boxCollider == null) return;

        Bounds bounds = _boxCollider.bounds;
        float centerY = bounds.center.y;
        float halfCell = _cellSize * 0.5f;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (_showGridWireframe && _gridWidth > 0 && _gridHeight > 0)
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    float wX = _cachedMinX + (x * _cellSize) + halfCell;
                    float wZ = _cachedMinZ + (y * _cellSize) + halfCell;
                    Gizmos.DrawWireCube(new Vector3(wX, centerY, wZ), new Vector3(_cellSize, 0.1f, _cellSize));
                }
            }
        }

        if (_cachedExclusions != null && _cachedExclusions.Count > 0)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            foreach (var cell in _cachedExclusions)
            {
                float wX = _cachedMinX + (cell.x * _cellSize) + halfCell;
                float wZ = _cachedMinZ + (cell.y * _cellSize) + halfCell;
                Gizmos.DrawCube(new Vector3(wX, centerY, wZ), new Vector3(_cellSize - 0.1f, 0.2f, _cellSize - 0.1f));
            }
        }

        if (_latestResult != null && _latestResult.Map != null)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
            for (int x = 0; x < _latestResult.Width; x++)
            {
                for (int y = 0; y < _latestResult.Height; y++)
                {
                    if (_latestResult.Map[x, y] == 0)
                    {
                        float wX = _cachedMinX + (x * _cellSize) + halfCell;
                        float wZ = _cachedMinZ + (y * _cellSize) + halfCell;
                        Gizmos.DrawCube(new Vector3(wX, centerY + 0.05f, wZ), new Vector3(_cellSize - 0.1f, 0.2f, _cellSize - 0.1f));
                    }
                }
            }
        }

        if (WalkerPaths != null)
        {
            Gizmos.color = Color.magenta;
            foreach (var path in WalkerPaths)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    Gizmos.DrawLine(path[i - 1] + Vector3.up * 0.15f, path[i] + Vector3.up * 0.15f);
                }
            }
        }

        if (_cachedAllPois != null)
        {
            foreach (var poi in _cachedAllPois)
            {
                if (poi == null) continue;

                Gizmos.color = (poi.EvidenceNode != null) ? Color.yellow : Color.cyan;
                float zoneSize = _cellSize * 7f;

                int gX = Mathf.FloorToInt((poi.transform.position.x - _cachedMinX) / _cellSize);
                int gY = Mathf.FloorToInt((poi.transform.position.z - _cachedMinZ) / _cellSize);
                float zoneCenterX = _cachedMinX + (gX * _cellSize) + halfCell;
                float zoneCenterZ = _cachedMinZ + (gY * _cellSize) + halfCell;

                Vector3 zoneCenter = new Vector3(zoneCenterX, centerY + 0.1f, zoneCenterZ);
                Gizmos.DrawWireCube(zoneCenter, new Vector3(zoneSize, 0.5f, zoneSize));
                Gizmos.DrawLine(zoneCenter, poi.transform.position);
            }
        }
    }
}