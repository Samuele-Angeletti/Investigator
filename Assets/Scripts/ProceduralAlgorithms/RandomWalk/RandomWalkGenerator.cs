using System;
using System.Collections.Generic;

public class RandomWalkGenerator : IMapGenerator
{
    private readonly RandomWalkSettings _randomWalkSettings;
    private readonly Random _range;
    private readonly (int dx, int dy)[] _directions;

    // Direzioni cardinali di default
    private readonly static (int dx, int dy)[] DefaultDirections = { (0, 1), (0, -1), (1, 0), (-1, 0) }; // destra, sinistra, su, giù

    public RandomWalkGenerator(RandomWalkSettings settings)
    {
        _randomWalkSettings = settings;
        int seed = settings.RandomSeed ? new Random().Next() : settings.Seed;
        _range = new Random(seed);
        _directions = settings.CustomDirections ?? DefaultDirections;
    }

    /// <summary>
    /// Ritorna 0 dove i walker sono passati
    /// </summary>
    /// <returns></returns>
    public GenerationResult Generate()
    {
        // FASE 1 computazione della griglia
        var (width, height, offsetX, offsetY) = ComputeGridDimensions();

        // FASE 2 inizializzazione mappa
        var map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 1;
            }
        }

        // FASE 3 punto di partenza
        int startX = _randomWalkSettings.Start.x - offsetX;
        int startY = _randomWalkSettings.Start.y - offsetY;

        // FASE 4 avvio walkers
        for (int w = 0; w < _randomWalkSettings.WalkerCount; w++)
        {
            RunWalker(map, startX, startY, width, height, offsetX, offsetY);
        }

        return new GenerationResult(width, height, map);
    }

    private void RunWalker(int[,] map, int startX, int startY, int width, int height, int offsetX, int offsetY)
    {
        // 0 = floor
        // 1 = wall

        int currentX = startX;
        int currentY = startY;

        var floorCells = new List<(int x, int y)>();
        MarkFloor(map, currentX, currentY, floorCells);

        for (int step = 0; step < _randomWalkSettings.Steps; step++)
        {
            // raccogliamo le direzioni valide
            var validDirections = GetValidDirections(currentX, currentY, width, height, offsetX, offsetY);

            if (validDirections.Count == 0)
            {
                // fallback di sicureza
                var fallback = floorCells[_range.Next(floorCells.Count)];
                currentX = fallback.x;
                currentY = fallback.y;
                continue;
            }

            // ottengo una direzione valida casuale
            var (dx, dy) = validDirections[_range.Next(validDirections.Count)];
            currentX += dx;
            currentY += dy;

            if (map[currentX, currentY] == 1)
                MarkFloor(map, currentX, currentY, floorCells);

            /*
             |  |  |  |
             |  |  |  |
             |w2|w1|  |
             |  |  |  |
             */
        }
    }

    private List<(int x, int y)> GetValidDirections(int currentX, int currentY, int width, int height, int offsetX, int offsetY)
    {
        var valid = new List<(int x, int y)>();

        foreach (var (dx, dy) in _directions)
        {
            int nextX = currentX + dx;
            int nextY = currentY + dy;

            // Check della dimensione
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height) continue; // non posso andare in questa direzione

            // Check del bounds
            if (_randomWalkSettings.Bounds.HasValue)
            {
                var value = _randomWalkSettings.Bounds.Value;
                int boundedX = nextX + offsetX;
                int boundedY = nextY + offsetY;
                // intervallo chiuso
                if (boundedX < value.xMin || boundedX > value.xMax || boundedY < value.yMin || boundedY > value.yMax) continue; // non psso andare in questa direzione
            }

            // inseriamo la direzione valida da prendere, ma aggiorniamo la posizione solo dopo
            valid.Add((dx, dy));
        }

        return valid;
    }

    private void MarkFloor(int[,] map, int currentX, int currentY, List<(int x, int y)> floorCells)
    {
        map[currentX, currentY] = 0;
        floorCells.Add((currentX, currentY));
    }

    /// <summary>
    /// Calcola le dimensioni della griglia in base al bounds
    /// </summary>
    /// <returns></returns>
    private (int width, int height, int offsetX, int offsetY) ComputeGridDimensions()
    {
        if (_randomWalkSettings.Bounds.HasValue)
        {
            var value = _randomWalkSettings.Bounds.Value;
            return (value.xMax - value.xMin + 1, value.yMax - value.yMin + 1, value.xMin, value.yMin);
        }

        // Poiché si stima che il walker si allontana di circa radice quadrata di steps dall'origine
        // Faremo un calcolo euristico
        int stima = (int)(Math.Sqrt(_randomWalkSettings.Steps) * 3) + 10;
        int offsetX = _randomWalkSettings.Start.x - stima / 2;
        int offsetY = _randomWalkSettings.Start.y - stima / 2;
        return (stima, stima, offsetX, offsetY);
    }
}
