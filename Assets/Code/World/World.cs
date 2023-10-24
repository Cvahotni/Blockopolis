using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct World
{
    private string name;
    private int seed;

    public string Name { get { return name; }}
    public int Seed { get { return seed; }}

    public World(string name, int seed) {
        this.name = name;
        this.seed = seed;
    }
}
