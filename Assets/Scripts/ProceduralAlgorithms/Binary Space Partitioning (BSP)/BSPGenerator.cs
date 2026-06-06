using System;

public class BSPGenerator : IMapGenerator
{
    private readonly BSPSettings _settings;

    public BSPGenerator(BSPSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Esegue l'intera pipeline di generazione del BSP
    /// 
    /// La mappa è una griglia di interi:
    ///     1 = muro
    ///     0 = pavimento
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public GenerationResult Generate()
    {
        int seed = _settings.RandomSeed ? new Random().Next() : _settings.Seed;
        var random = new Random(seed);

        var treeBuilder = new BSPTreeBuilder(random, _settings.MinPartitioningWidth, _settings.MinPartitioningHeight);
        var root = treeBuilder.Build(_settings.Width, _settings.Height);

        var map = new int[_settings.Width, _settings.Height];
        for (int x = 0; x < _settings.Width; x++)
            for (int y = 0; y < _settings.Height; y++)
                map[x, y] = 1; // tutto muro

        var leaves = root.GetLeaves();
        var roomPlacer = new BSPRoomPlacer(random, _settings.RoomPadding);
        roomPlacer.PlaceRooms(leaves, map);

        var corridorCarver = new BSPCorridorCarver();
        corridorCarver.CarveCorridors(root, map);

        return new GenerationResult(_settings.Width, _settings.Height, map);
    }
}
