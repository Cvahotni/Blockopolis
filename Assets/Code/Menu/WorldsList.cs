using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldsList : MonoBehaviour
{
    public static WorldsList Instance { get; private set; }

    [SerializeField] private GameObject worldListing;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void OnEnable() {
        GenerateWorldListings();
    }

    public void GenerateWorldListings() {
        ClearWorldListings();

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

    private void ClearWorldListings() {
        GameObject[] listingObjects = GameObject.FindGameObjectsWithTag(MenuProperties.worldListingTag);

        foreach(GameObject listingObject in listingObjects) {
            Destroy(listingObject);
        }
    }
}
