public struct World
{
    private string name;
    private int seed;

    public string Name { 
        get { return name; }
        set { name = value; }
    }
    
    public int Seed { get { return seed; }}

    public World(string name, int seed) {
        this.name = name;
        this.seed = seed;
    }
}
