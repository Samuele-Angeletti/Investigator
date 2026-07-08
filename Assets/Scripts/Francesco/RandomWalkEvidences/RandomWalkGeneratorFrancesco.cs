using System;
using System.Collections.Generic;

public class RandomWalkGeneratorFrancesco : IMapGenerator
{
    private readonly RandomWalkSettingsFrancesco _settings;
    private readonly Random _rng;
    private readonly (int dx, int dy)[] _dirs;
    private static readonly (int dx, int dy)[] DefaultDirs = { (0, 1), (0, -1), (1, 0), (-1, 0) };

    private int[,] _distField;          
    private List<(int x, int y)> _activePOIs;
    private HashSet<(int x, int y)> _exclusions;
    private int _width, _height, _offsetX, _offsetY;

    public List<List<(int x, int y)>> WalkerPaths { get; private set; } = new();
    public int OffsetX => _offsetX;
    public int OffsetY => _offsetY;

    public RandomWalkGeneratorFrancesco(RandomWalkSettingsFrancesco settings)
    {
        _settings = settings;
        int seed = settings.RandomSeed ? new Random().Next() : settings.Seed;
        _rng = new Random(seed);
        _dirs = settings.CustomDirections ?? DefaultDirs;
    }

    public GenerationResult Generate()
    {
        (_width, _height, _offsetX, _offsetY) = ComputeGridDimensions();
        var map = new int[_width, _height];
        
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                map[x, y] = 1; // 1 = Solid wall / Unvisited

        int startX = _settings.Start.x - _offsetX;
        int startY = _settings.Start.y - _offsetY;

        _exclusions = new HashSet<(int x, int y)>();
        if (_settings.Exclusions != null)
        {
            foreach (var exc in _settings.Exclusions)
                _exclusions.Add((exc.x - _offsetX, exc.y - _offsetY));
        }

        _activePOIs = new List<(int x, int y)>();
        if (_settings.Biases != null)
        {
            foreach (var bias in _settings.Biases)
                _activePOIs.Add((bias.x - _offsetX, bias.y - _offsetY));
        }

        ComputeDistanceField(_activePOIs);
        WalkerPaths.Clear();
        
        for (int w = 0; w < _settings.WalkerCount; w++)
        {
            RunWalker(map, startX, startY);
        }

        return new GenerationResult(_width, _height, map);
    }

    private void RunWalker(int[,] map, int startX, int startY)
    {
        Stack<(int x, int y)> history = new Stack<(int x, int y)>();
        HashSet<(int x, int y)> deadEnds = new HashSet<(int x, int y)>();
        int[,] visitCounts = new int[_width, _height];

        int cx = Math.Max(0, Math.Min(_width - 1, startX));
        int cy = Math.Max(0, Math.Min(_height - 1, startY));

        MarkFloor(map, cx, cy);
        history.Push((cx, cy));
        visitCounts[cx, cy] = 1;

        int step = 0;
        while (step < _settings.Steps && history.Count > 0)
        {
            List<(int dx, int dy)> validDirs = new List<(int dx, int dy)>();
            for (int d = 0; d < _dirs.Length; d++)
            {
                var (dx, dy) = _dirs[d];
                int nx = cx + dx;
                int ny = cy + dy;
                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height &&
                    !_exclusions.Contains((nx, ny)) && !deadEnds.Contains((nx, ny)))
                {
                    validDirs.Add((dx, dy));
                }
            }

            if (validDirs.Count == 0)
            {
                deadEnds.Add((cx, cy));
                history.Pop();
                if (history.Count == 0) break;
                var prev = history.Peek();
                cx = prev.x;
                cy = prev.y;
                continue;
            }

            double[] weights = new double[validDirs.Count];
            double totalWeight = 0.0;

            bool explore = _rng.NextDouble() < _settings.ExploreBias;

            if (explore)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = 1.0;
                    totalWeight += 1.0;
                }
            }
            else
            {
                float distanceFactor = _settings.PoiBiasIntensity * 0.001f;
                distanceFactor = Math.Max(0.01f, distanceFactor);

                for (int i = 0; i < validDirs.Count; i++)
                {
                    var (dx, dy) = validDirs[i];
                    int nx = cx + dx;
                    int ny = cy + dy;
                    int dist = _distField[nx, ny];

                    double weight = (dist >= 0) ? Math.Exp(-dist * distanceFactor) : 0.0001;

                    int visits = visitCounts[nx, ny];
                    weight /= (1.0 + visits * _settings.RevisitPenalty);

                    weights[i] = weight;
                    totalWeight += weight;
                }
            }

            double choice = _rng.NextDouble() * totalWeight;
            double sum = 0.0;
            int selectedIndex = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i];
                if (choice <= sum)
                {
                    selectedIndex = i;
                    break;
                }
            }

            var (chosenDx, chosenDy) = validDirs[selectedIndex];
            cx += chosenDx;
            cy += chosenDy;

            MarkFloor(map, cx, cy);
            history.Push((cx, cy));
            visitCounts[cx, cy]++;
            step++;
        }

        var finalPath = new List<(int x, int y)>(history);
        finalPath.Reverse();
        WalkerPaths.Add(finalPath);
    }

    private void ComputeDistanceField(List<(int x, int y)> pois)
    {
        _distField = new int[_width, _height];
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                _distField[x, y] = -1;

        var queue = new Queue<(int x, int y)>();
        foreach (var poi in pois)
        {
            if (poi.x < 0 || poi.x >= _width || poi.y < 0 || poi.y >= _height)
                continue;
            _distField[poi.x, poi.y] = 0;
            queue.Enqueue(poi);
        }

        while (queue.Count > 0)
        {
            (int cx, int cy) = queue.Dequeue();
            int current = _distField[cx, cy];
            foreach (var (dx, dy) in _dirs)
            {
                int nx = cx + dx, ny = cy + dy;
                if (nx < 0 || nx >= _width || ny < 0 || ny >= _height)
                    continue;
                if (_exclusions.Contains((nx, ny)))
                    continue;
                if (_distField[nx, ny] != -1)
                    continue;
                _distField[nx, ny] = current + 1;
                queue.Enqueue((nx, ny));
            }
        }
    }

    private void MarkFloor(int[,] map, int x, int y) => map[x, y] = 0;

    private (int width, int height, int offsetX, int offsetY) ComputeGridDimensions()
    {
        if (_settings.Bounds.HasValue)
        {
            var bounds = _settings.Bounds.Value;
            return (bounds.xMax - bounds.xMin + 1, bounds.yMax - bounds.yMin + 1, bounds.xMin, bounds.yMin);
        }
        return (50, 50, 0, 0);
    }
}