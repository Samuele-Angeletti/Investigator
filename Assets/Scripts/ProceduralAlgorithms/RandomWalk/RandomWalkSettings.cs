public class RandomWalkSettings
{
    public (int x, int y) Start = (25, 25);

    public int Steps = 5;

    public (int xMin, int yMin, int xMax, int yMax)? Bounds = null;

    public int Seed = 0;
    public bool RandomSeed = false;

    public int WalkerCount = 1;

    public (int dx, int dy)[] CustomDirections = null;
}
