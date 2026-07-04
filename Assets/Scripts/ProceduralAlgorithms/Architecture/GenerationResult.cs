using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationResult
{
    public int Width { get; }
    public int Height { get; }
    public int[,] Map { get; }

    public GenerationResult(int width, int height, int[,] map)
    {
        Width = width;
        Height = height;
        Map = map;
    }

    // Helper di Lettura

    /// <summary>
    /// Esempio d'uso: result.ForEachCell(x, y, value) => Debug.Log($"{x},{y} = {value}")
    /// </summary>
    /// <param name="action"></param>
    public void ForEachCell(Action<int, int, int> action)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                action(x, y, Map[x, y]);
            }
        }
    }
    public bool IsWall(int xPosMap, int yPosMap)
    {
        return Map[xPosMap, yPosMap] != 0; //Map[xPosMap, yPosMap] != null && 
    }
    public Vector2Int GetFirstCornerBottomLeft()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Map[x, y] == 0)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }
    public Vector2Int GetRandomRoomPos()
    {
        List<Vector2Int> availablePoints = new List<Vector2Int>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Map[x, y] == 0)
                    availablePoints.Add(new Vector2Int(x, y));
            }
        }
        if (availablePoints.Count == 0)
            return Vector2Int.zero;
        int randomPoint = UnityEngine.Random.Range(0, availablePoints.Count);
        return availablePoints[randomPoint];
    }
}
