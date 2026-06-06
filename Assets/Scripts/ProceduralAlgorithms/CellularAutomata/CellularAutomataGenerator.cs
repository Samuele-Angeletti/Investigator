using System;

public class CellularAutomataGenerator : IMapGenerator
{
    private readonly CellularAutomataSettings _settings;

    public CellularAutomataGenerator(CellularAutomataSettings settings)
    {
        _settings = settings;
    }

    public GenerationResult Generate()
    {
        int seed = _settings.RandomSeed ? new Random().Next() : _settings.Seed;
        var random = new Random(seed);

        // FASE 1 riempimento mappa casuale

        var map = new int[_settings.Width, _settings.Height];

        var initializer = new CellularAutomataInitializer(random, _settings.InitialWallChance, _settings.SolidBorder);
        initializer.Initialize(map);

        // FASE 2 simulazione

        var simulator = new CellularAutomataSimulator(_settings.Steps, _settings.BirthLimit, _settings.DeathLimit, _settings.SolidBorder);

        var result = simulator.Simulate(map);

        return new GenerationResult(_settings.Width, _settings.Height, result);
    }
}
