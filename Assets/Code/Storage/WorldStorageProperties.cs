using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldStorageProperties
{
    public static readonly string metadataExtension = ".metadata";
    public static readonly string savesFolderName = "saves" + Path.DirectorySeparatorChar;
    public static readonly string savesFolderSecondaryName = "saves";
    public static readonly string regionFolderName = "region";
    public static readonly string worldInfoFileName = Path.DirectorySeparatorChar + "info.txt";
    public static readonly string inventoryFileName = Path.DirectorySeparatorChar + "inventory.txt";
}
