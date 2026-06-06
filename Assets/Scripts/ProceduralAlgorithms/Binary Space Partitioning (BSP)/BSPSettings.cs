/// <summary>
/// Contenitore delle informazioni per la generazione delle stanze usando il BSP
/// </summary>
public class BSPSettings
{
    /// <summary>
    /// Larghezza della mappa in celle
    /// </summary>
    public int Width = 50;

    /// <summary>
    /// Altezza della mappa in celle (z)
    /// </summary>
    public int Height = 50;

    /// <summary>
    /// Numero di celle di partizione minimo in lunghezza
    /// </summary>
    public int MinPartitioningWidth = 5;

    /// <summary>
    /// Numero di celle di partizione minimo in altezza (z)
    /// </summary>
    public int MinPartitioningHeight = 5;

    /// <summary>
    /// Margine interno da usare per creare le stanze nelle aree suddivise
    /// </summary>
    public int RoomPadding = 1;

    public int Seed;
    public bool RandomSeed;
}
