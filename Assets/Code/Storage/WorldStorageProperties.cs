using System.IO;

public class WorldStorageProperties
{
    public static readonly string savesFolderName = "saves" + Path.DirectorySeparatorChar;
    public static readonly string savesFolderSecondaryName = "saves";
    public static readonly string regionFolderName = "region";
    public static readonly string worldInfoFileName = Path.DirectorySeparatorChar + "info.json";
    public static readonly string inventoryFileName = Path.DirectorySeparatorChar + "inventory.json";
    public static readonly string playerFileName = Path.DirectorySeparatorChar + "player.json";
    public static readonly int worldNameLimit = 32;
    public static readonly int worldSeedLimit = 32;
}
