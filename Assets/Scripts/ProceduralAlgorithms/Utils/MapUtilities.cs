public static class MapUtilities
{
    /// <summary>
    /// Restituisce se la cella si trova sul bordo della mappa
    /// </summary>
    public static bool IsBorderCell(int x, int y, int width, int height) => x == 0 || x == width - 1 || y == 0 || y == height - 1;
}