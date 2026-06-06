using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CellularAutomataInitializer
{
    private readonly Random _random;
    private readonly int _wallChance;
    private readonly bool _solidBorder;

    public CellularAutomataInitializer(Random random, int wallChance, bool solidBorder)
    {
        _random = random;
        _wallChance = wallChance;
        _solidBorder = solidBorder;
    }

    /// <summary>
    /// Inizializza la mappa in maniera casuale e ponderata dalla wallChance
    /// </summary>
    /// <param name="map"></param>
    public void Initialize(int[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_solidBorder && MapUtilities.IsBorderCell(x, y, width, height))
                {
                    map[x, y] = 1; // imposta il muro al bordo
                    continue;
                }

                map[x, y] = _random.Next(100) < _wallChance ? 1 : 0;
            }
        }
    }
}