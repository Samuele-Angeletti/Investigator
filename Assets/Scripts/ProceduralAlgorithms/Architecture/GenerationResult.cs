using System;

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
}
