using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldsEditingToggle : MonoBehaviour
{
    public static WorldsEditingToggle Instance { get; private set; }

    private List<WorldButtonListing> listings = new List<WorldButtonListing>();
    private bool isEditing = false;

    [SerializeField] private TMP_Text editButtonText;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ToggleEditMode() {
        isEditing = !isEditing;

        PopulateWorldListings();
        UpdateWorldListings();
    }

    public void DisableEditMode() {
        SetEditMode(false);
    }

    private void SetEditMode(bool value) {
        isEditing = value;

        PopulateWorldListings();
        UpdateWorldListings();
    }

    private void PopulateWorldListings() {
        listings.Clear();
        GameObject[] listingObjects = GameObject.FindGameObjectsWithTag(MenuProperties.worldListingTag);

        foreach (GameObject listingObject in listingObjects) {
            listings.Add(listingObject.GetComponent<WorldButtonListing>());
        }
    }

    private void UpdateWorldListings() {
        UpdateEditButtonText();

        foreach(WorldButtonListing listing in listings) {
            listing.IsEditing = isEditing;
        }
    }

    private void UpdateEditButtonText() {
        editButtonText.text = isEditing ? MenuProperties.editButtonTextEnabled : MenuProperties.editButtonTextDisabled;
    }
}
