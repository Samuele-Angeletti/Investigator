public class RandomWalkSettingsFrancesco
{
    public (int x, int y) Start = (25, 25);
    public int Steps = 5;
    public (int xMin, int yMin, int xMax, int yMax)? Bounds = null;
    public int Seed = 0;
    public bool RandomSeed = false;
    public int WalkerCount = 1;
    public (int dx, int dy)[] CustomDirections = null;

    // Global coordinates mapped inside Biases array (POIs)
    public (int x, int y)[] Biases;

    // Global coordinates that walkers are strictly forbidden from entering
    public (int x, int y)[] Exclusions;

    // Amplification intensity scalar factor for choosing favored POI paths
    public float PoiBiasIntensity = 3.0f;
}