using UnityEngine;

public class CADirector : MonoBehaviour
{
    GenerationResult generationResult;

    [Header("Cellular Automata Settings")]
    [SerializeField] int caWidth = 50;
    [SerializeField] int caHeight = 50;
    [SerializeField] int caInitialWallChance = 45;
    [SerializeField] int caSteps = 5;
    [SerializeField] int caBirthLimit = 4;
    [SerializeField] int caDeathLimit = 3;
    [SerializeField] bool caSolidBorder = true;

    [ContextMenu("Debug_BSP")]
    public void GenerateBSPDebug(Vector2Int _area)
    {
        generationResult = GenerateCellularAutomata(_area);
    }
    private GenerationResult GenerateCellularAutomata()
    {
        return new CellularAutomataGenerator(new CellularAutomataSettings()
        {
            Width = caWidth,
            Height = caHeight,
            InitialWallChance = caInitialWallChance,
            Steps = caSteps,
            BirthLimit = caBirthLimit,
            DeathLimit = caDeathLimit,
            Seed = 0,
            SolidBorder = caSolidBorder,
            RandomSeed = true
        }).Generate();
    }

    private GenerationResult GenerateCellularAutomata(Vector2Int _area)
    {
        return new CellularAutomataGenerator(new CellularAutomataSettings()
        {
            Width = _area.x,
            Height = _area.y,
            InitialWallChance = caInitialWallChance,
            Steps = caSteps,
            BirthLimit = caBirthLimit,
            DeathLimit = caDeathLimit,
            Seed = 0,
            SolidBorder = caSolidBorder,
            RandomSeed = true
        }).Generate();
    }
}
