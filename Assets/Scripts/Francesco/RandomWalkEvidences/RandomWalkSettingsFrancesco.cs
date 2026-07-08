public class RandomWalkSettingsFrancesco
{
    public (int x, int y) Start = (25, 25);
    public int Steps = 5;
    public (int xMin, int yMin, int xMax, int yMax)? Bounds = null;
    public int Seed = 0;
    public bool RandomSeed = false;
    public int WalkerCount = 1;
    public (int dx, int dy)[] CustomDirections = null;
    public (int x, int y)[] Biases;
    public (int x, int y)[] Exclusions;

    public float PoiBiasIntensity = 1000f;
    public float ExploreBias = 0.1f;
    public float RevisitPenalty = 0.001f;
    public float Inertia = 3.0f;
}