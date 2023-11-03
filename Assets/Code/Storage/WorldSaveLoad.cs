using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldSaveLoad
{
    public static void EraseFileContents(string path) {
        FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

        stream.SetLength(0);
        stream.Close();
    }

    public static bool DoesFileExist(string path) {
        return File.Exists(path);
    }

    public static string CheckWorldSaveFile(World world, string fileName) {
        string path = WorldStorageProperties.savesFolderName + world.Name + fileName;
        if(DoesFileExist(path)) EraseFileContents(path);
        
        else {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + world.Name);
        }

        return path;
    }

    public static string CheckWorldLoadFile(string path, string fileName) {
        string loadPath = WorldStorageProperties.savesFolderName + path + fileName;

        if(!DoesFileExist(WorldStorageProperties.savesFolderName)) {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + path);
        }

        if(!File.Exists(loadPath)) {
            using (File.Create(loadPath));
        }

        return loadPath;
    }
}
