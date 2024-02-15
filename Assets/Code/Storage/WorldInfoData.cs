using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WorldInfoData
{
    [SerializeField] private string name;
    [SerializeField] private int seed;

    public string Name { get { return name; }}
    public int Seed { get { return seed; }}

    public WorldInfoData(World world) {
        name = world.Name;
        seed = world.Seed;
    }
}
