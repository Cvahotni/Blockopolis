using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct World
{
    private string name;
    private int seed;
    private string path;

    public string Name { 
        get { return name; }
        set { name = value; }
    }

    public string Path {
        get { return path; }
        set { path = value; }
    }
    
    public int Seed { get { return seed; }}

    public World(string name, int seed) {
        this.name = name;
        this.seed = seed;

        this.path = "";
    }
}
