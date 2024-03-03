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

    public static void CheckWorldSaveFile(World world, string fileName) {
        string path = GetWorldFilePath(world, fileName);
        if(DoesFileExist(path)) EraseFileContents(path);
        
        else {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + world.Name);
        }
    }

    public static void CheckWorldLoadFile(string path, string fileName) {
        string loadPath = GetWorldFilePath(path, fileName);

        if(!DoesFileExist(WorldStorageProperties.savesFolderName)) {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + path);
        }

        if(!File.Exists(loadPath)) {
            using (File.Create(loadPath));
        }
    }

    public static string GetWorldFilePath(World world, string fileName) {
        return GetWorldFilePath(world.Name, fileName);
    }

    public static string GetWorldFilePath(string path, string fileName) {
        return WorldStorageProperties.savesFolderName + path + fileName;
    }
}
