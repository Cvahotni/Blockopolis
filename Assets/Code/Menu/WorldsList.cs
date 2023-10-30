using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldsList : MonoBehaviour
{
    [SerializeField] private GameObject worldListing;

    private void Start() {
        CreateWorldListings();
    }

    private void CreateWorldListings() {
        string currentDirectory = Directory.GetCurrentDirectory();
        string path = currentDirectory + Path.DirectorySeparatorChar + WorldStorageProperties.savesFolderSecondaryName;
        
        if(!Directory.Exists(WorldStorageProperties.savesFolderSecondaryName)) return;
        string[] subDirectories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

        foreach(string subDirectory in subDirectories) {
            string[] splitName = subDirectory.Split(Path.DirectorySeparatorChar);
            string currentWorldName = splitName[splitName.Length - 1];

            if(!WorldHandler.DoesWorldExist(currentWorldName)) continue;
        
            GameObject worldListingObject = Instantiate(worldListing, transform.position, Quaternion.identity, this.transform);
            WorldListing currentListing = worldListingObject.GetComponent<WorldListing>();

            currentListing.SetName(currentWorldName);
        }
    }
}
