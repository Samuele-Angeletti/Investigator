using System;
using System.Collections.Generic;

/// <summary>
/// Si occupa di scavare le stanze nelle foglie ottenute dal BSPTreeBuilder
/// </summary>
public class BSPRoomPlacer
{
    private readonly Random _random;
    private readonly int _padding;

    public BSPRoomPlacer(Random random, int padding)
    {
        _random = random;
        _padding = padding;
    }

    public void PlaceRooms(List<BSPPartition> leaves, int[,] map)
    {
        foreach (var leaf in leaves)
        {
            PlaceRoomInLeaf(leaf, map);
        }
    }

    private void PlaceRoomInLeaf(BSPPartition leaf, int[,] map)
    {
        int availableWidth = leaf.Width - _padding * 2;
        int availableHeight = leaf.Height - _padding * 2;

        if (availableWidth < _padding * 2 || availableHeight < _padding * 2)
            return; // spazio troppo piccolo per scavare una stanza

        // dimensione casuale
        int roomWidth = _random.Next(_padding * 2, availableWidth + 1);
        int roomHeight = _random.Next(_padding * 2, availableHeight + 1);

        // posizione di inizio all'interno dello spazio
        int randomX = leaf.X + _padding + _random.Next(0, availableWidth - roomWidth + 1);
        int randomY = leaf.Y + _padding + _random.Next(0, availableHeight - roomHeight + 1);

        leaf.AssignRoom(randomX, randomY, roomWidth, roomHeight);

        CarveRoom(map, randomX, randomY, roomWidth, roomHeight);
    }

    private void CarveRoom(int[,] map, int randomX, int randomY, int roomWidth, int roomHeight)
    {
        for (int carveX = randomX; carveX < randomX + roomWidth; carveX++)
        {
            for (int carveY = randomY; carveY < randomY + roomHeight; carveY++)
            {
                map[carveX, carveY] = 0; // pavimento
            }
        }
    }
}