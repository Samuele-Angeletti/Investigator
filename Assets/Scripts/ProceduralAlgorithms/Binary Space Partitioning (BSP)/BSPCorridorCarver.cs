using System;
using System.Collections.Generic;

/// <summary>
/// Si occupa esclusivamente di scavare i corridoi tra le stanze del BSP.
/// Opera sull'albero di partizioni già costruito e sulle stanze già posizionate.
/// 
/// Strategia: percorre l'albero bottom-up. Per ogni nodo interno collega
/// il centro della sua partizione sinistra (First) con quello della destra (Second)
/// tramite un corridoio a L.
/// </summary>
public class BSPCorridorCarver
{
    /// <summary>
    /// Collega ricorsivamente tutte le coppie di sottoalberi nell'albero BSP
    /// </summary>
    public void CarveCorridors(BSPPartition root, int[,] map)
    {
        if (root == null) return;

        // se è una foglia non c'è nulla da connettere
        if (root.First == null && root.Second == null)
            return;

        // ricorsione bottom-up: prima collego i sottoalberi figli,
        // poi collego i due figli tra loro
        CarveCorridors(root.First, map);
        CarveCorridors(root.Second, map);

        var centerFirst = root.First?.GetRoomCenter();
        var centerSecond = root.Second?.GetRoomCenter();

        if (centerFirst == null || centerSecond == null)
            return;

        CarveL(map, centerFirst.Value, centerSecond.Value);
    }

    /// <summary>
    /// Scava un corridoio a forma di L tra due punti.
    /// Prima si muove orizzontalmente, poi verticalmente.
    /// </summary>
    private void CarveL(int[,] map, (int x, int y) from, (int x, int y) to)
    {
        // tratto orizzontale: da from.x a to.x sulla riga from.y
        CarveHorizontal(map, from.y, from.x, to.x);

        // tratto verticale: da from.y a to.y sulla colonna to.x
        CarveVertical(map, to.x, from.y, to.y);
    }

    private void CarveHorizontal(int[,] map, int y, int x1, int x2)
    {
        int startX = Math.Min(x1, x2);
        int endX = Math.Max(x1, x2);

        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        for (int x = startX; x <= endX; x++)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                map[x, y] = 0;
        }
    }

    private void CarveVertical(int[,] map, int x, int y1, int y2)
    {
        int startY = Math.Min(y1, y2);
        int endY = Math.Max(y1, y2);

        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        for (int y = startY; y <= endY; y++)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                map[x, y] = 0;
        }
    }
}
