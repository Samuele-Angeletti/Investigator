using System;
using System.Collections.Generic;

public class RandomWalkGeneratorFrancesco : IMapGenerator
{
    private readonly RandomWalkSettingsFrancesco _randomWalkSettings;
    private readonly Random _range;
    private readonly (int dx, int dy)[] _directions;

    private readonly static (int dx, int dy)[] DefaultDirections = { (0, 1), (0, -1), (1, 0), (-1, 0) };

    public RandomWalkGeneratorFrancesco(RandomWalkSettingsFrancesco settings)
    {
        _randomWalkSettings = settings;
        int seed = settings.RandomSeed ? new Random().Next() : settings.Seed;
        _range = new Random(seed);
        _directions = settings.CustomDirections ?? DefaultDirections;
    }

    public GenerationResult Generate()
    {
        var (width, height, offsetX, offsetY) = ComputeGridDimensions();
        var map = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        int startX = _randomWalkSettings.Start.x - offsetX;
        int startY = _randomWalkSettings.Start.y - offsetY;

        // Since coordinates are processed as localized 0-based metrics in Initialize, deep offsets vanish
        var localPOIs = new List<(int x, int y)>(_randomWalkSettings.Biases ?? new (int, int)[0]);
        var localExclusions = new HashSet<(int x, int y)>(_randomWalkSettings.Exclusions ?? new (int, int)[0]);

        for (int w = 0; w < _randomWalkSettings.WalkerCount; w++)
        {
            RunWalker(map, startX, startY, width, height, offsetX, offsetY, localPOIs, localExclusions);
        }

        return new GenerationResult(width, height, map);
    }

    private void RunWalker(int[,] map, int startX, int startY, int width, int height, int offsetX, int offsetY, List<(int x, int y)> pois, HashSet<(int x, int y)> exclusions)
    {
        int currentX = startX;
        int currentY = startY;

        var floorCells = new List<(int x, int y)>();
        var pathHistory = new Stack<(int x, int y)>();

        if (!exclusions.Contains((currentX, currentY)))
        {
            MarkFloor(map, currentX, currentY, floorCells);
            pathHistory.Push((currentX, currentY));
        }

        CheckAndRemovePoi(currentX, currentY, pois);
        var weights = new List<double>();

        for (int step = 0; step < _randomWalkSettings.Steps; step++)
        {
            var validDirections = GetValidDirections(currentX, currentY, width, height, offsetX, offsetY, exclusions);

            if (validDirections.Count == 0)
            {
                if (pathHistory.Count > 1)
                {
                    pathHistory.Pop();
                    var previous = pathHistory.Peek();
                    currentX = previous.x;
                    currentY = previous.y;
                }
                else if (floorCells.Count > 0)
                {
                    var fallback = floorCells[_range.Next(floorCells.Count)];
                    currentX = fallback.x;
                    currentY = fallback.y;
                }
                else break;

                continue;
            }

            weights.Clear();
            double totalWeight = 0;
            (int x, int y)? targetPOI = FindClosestPOI(currentX, currentY, pois);

            foreach (var (dx, dy) in validDirections)
            {
                double weight = 1.0;
                if (targetPOI.HasValue)
                {
                    int nextX = currentX + dx;
                    int nextY = currentY + dy;

                    double currentDist = GetDistanceSq(currentX, currentY, targetPOI.Value.x, targetPOI.Value.y);
                    double nextDist = GetDistanceSq(nextX, nextY, targetPOI.Value.x, targetPOI.Value.y);

                    if (nextDist < currentDist)
                    {
                        weight *= _randomWalkSettings.PoiBiasIntensity;
                    }
                }
                weights.Add(weight);
                totalWeight += weight;
            }

            double roll = _range.NextDouble() * totalWeight;
            double counter = 0;
            int selectedIndex = 0;

            for (int i = 0; i < weights.Count; i++)
            {
                counter += weights[i];
                if (roll <= counter)
                {
                    selectedIndex = i;
                    break;
                }
            }

            var (chosenDx, chosenDy) = validDirections[selectedIndex];
            currentX += chosenDx;
            currentY += chosenDy;

            if (map[currentX, currentY] == 1)
                MarkFloor(map, currentX, currentY, floorCells);

            pathHistory.Push((currentX, currentY));
            CheckAndRemovePoi(currentX, currentY, pois);
        }
    }

    private void CheckAndRemovePoi(int tx, int ty, List<(int x, int y)> pois)
    {
        for (int i = pois.Count - 1; i >= 0; i--)
        {
            if (pois[i].x == tx && pois[i].y == ty) pois.RemoveAt(i);
        }
    }

    private (int x, int y)? FindClosestPOI(int cx, int cy, List<(int x, int y)> pois)
    {
        if (pois == null || pois.Count == 0) return null;
        (int x, int y)? closest = null;
        double minDist = double.MaxValue;
        foreach (var poi in pois)
        {
            double dist = GetDistanceSq(cx, cy, poi.x, poi.y);
            if (dist < minDist) { minDist = dist; closest = poi; }
        }
        return closest;
    }

    private double GetDistanceSq(int x1, int y1, int x2, int y2) => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

    private List<(int x, int y)> GetValidDirections(int cx, int cy, int w, int h, int ox, int oy, HashSet<(int x, int y)> ex)
    {
        var valid = new List<(int x, int y)>();
        foreach (var (dx, dy) in _directions)
        {
            int nx = cx + dx; int ny = cy + dy;
            if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;
            if (ex.Contains((nx, ny))) continue;

            if (_randomWalkSettings.Bounds.HasValue)
            {
                var val = _randomWalkSettings.Bounds.Value;
                if (nx + ox < val.xMin || nx + ox > val.xMax || ny + oy < val.yMin || ny + oy > val.yMax) continue;
            }
            valid.Add((dx, dy));
        }
        return valid;
    }

    private void MarkFloor(int[,] map, int cx, int cy, List<(int x, int y)> fc)
    {
        map[cx, cy] = 0;
        fc.Add((cx, cy));
    }

    private (int width, int height, int offsetX, int offsetY) ComputeGridDimensions()
    {
        if (_randomWalkSettings.Bounds.HasValue)
        {
            var v = _randomWalkSettings.Bounds.Value;
            return (v.xMax - v.xMin + 1, v.yMax - v.yMin + 1, v.xMin, v.yMin);
        }
        int stima = (int)(Math.Sqrt(_randomWalkSettings.Steps) * 3) + 10;
        return (stima, stima, _randomWalkSettings.Start.x - stima / 2, _randomWalkSettings.Start.y - stima / 2);
    }
}