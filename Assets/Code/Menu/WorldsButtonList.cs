using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldsButtonList : MonoBehaviour
{
    public static WorldsButtonList Instance { get; private set; }

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
        if(WorldList.Count() == 0) WorldList.Populate();

        for(int i = 0; i < WorldList.Count(); i++) {
            string currentWorldName = WorldList.At(i);

            GameObject worldListingObject = Instantiate(worldListing, transform.position, Quaternion.identity, this.transform);
            WorldButtonListing currentListing = worldListingObject.GetComponent<WorldButtonListing>();

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
