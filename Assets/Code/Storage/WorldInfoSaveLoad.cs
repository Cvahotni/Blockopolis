using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldInfoSaveLoad
{
    public static void SaveWorldInfo(World world) {
        string path = WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.worldInfoFileName);

        using (StreamWriter writer = new StreamWriter(path, append: true)) {
            writer.WriteLine("name: " + world.Name);
            writer.WriteLine("seed: " + world.Seed);
        }
    }

    public static World LoadWorldInfo(string path) {
        string line;

        string name = "";
        int seed = -1;

        string loadPath = WorldSaveLoad.CheckWorldLoadFile(path, WorldStorageProperties.worldInfoFileName);

        using (StreamReader reader = new StreamReader(loadPath)) while((line = reader.ReadLine()) != null) {
            string[] splitString = line.Split(": ");

            if(line.Contains("name")) name = splitString[1];
            if(line.Contains("seed")) seed = int.Parse(splitString[1]);
        }

        return new World(name, seed);
    }
}
