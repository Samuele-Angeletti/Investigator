/// <summary>
/// Responsabile di eseguire la simulazione dell'algoritmo
/// </summary>
public class CellularAutomataSimulator
{
    private readonly int _steps;
    private readonly int _birthLimit;
    private readonly int _deathLimit;
    private readonly bool _solidBorder;

    public CellularAutomataSimulator(int steps, int birthLimit, int deathLimit, bool solidBorder)
    {
        _steps = steps;
        _birthLimit = birthLimit;
        _deathLimit = deathLimit;
        _solidBorder = solidBorder;
    }

    public int[,] Simulate(int[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        int[,] current = map;

        for (int i = 0; i < _steps; i++)
        {
            current = SimulateStep(current, width, height);
        }

        return current;
    }

    /// <summary>
    /// Esegue la scansione totale della mappa e ne esegue la modifica restituendone una nuova
    /// </summary>
    private int[,] SimulateStep(int[,] current, int width, int height)
    {
        var next = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_solidBorder && MapUtilities.IsBorderCell(x, y, width, height))
                {
                    next[x, y] = 1; // imposta il muro al bordo
                    continue;
                }

                next[x, y] = ComputeNextState(current, x, y);
            }
        }

        return next;
    }

    private int ComputeNextState(int[,] current, int x, int y)
    {
        int wallNeighbors = CountWallNeighbors(current, x, y);

        if (current[x, y] == 1) // se muro
            return wallNeighbors >= _deathLimit ? 1 : 0;
        else // se pavimento
            return wallNeighbors >= _birthLimit ? 1 : 0;
    }

    private int CountWallNeighbors(int[,] current, int x, int y)
    {
        int width = current.GetLength(0);
        int height = current.GetLength(1);
        int count = 0;

        for (int destinationX = -1; destinationX <= 1; destinationX++)
        {
            for (int destinationY = -1; destinationY <= 1; destinationY++)
            {
                if (destinationX == 0 && destinationY == 0)
                    continue; // la cella stessa la ignoriamo

                int newX = x + destinationX;
                int newY = y + destinationY;

                // check se siamo fuori dai bordi
                if (newX < 0 || newX >= width || newY < 0 || newY >= height)
                {
                    count++;
                    continue;
                }

                if (current[newX, newY] == 1) // se è muro
                    count++;
            }
        }

        return count;
    }
}
