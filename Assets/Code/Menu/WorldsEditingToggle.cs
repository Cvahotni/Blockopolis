using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldsEditingToggle : MonoBehaviour
{
    public static WorldsEditingToggle Instance { get; private set; }

    private List<WorldListing> listings = new List<WorldListing>();
    private bool isEditing = false;

    [SerializeField] private TMP_Text editButtonText;
    [SerializeField] private TMP_Text editButtonTextShadow;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ToggleEditMode() {
        isEditing = !isEditing;

        PopulateWorldListings();
        UpdateWorldListings();
    }

    public void SetEditMode(bool value) {
        isEditing = value;

        PopulateWorldListings();
        UpdateWorldListings();
    }

    private void PopulateWorldListings() {
        listings.Clear();
        GameObject[] listingObjects = GameObject.FindGameObjectsWithTag(MenuProperties.worldListingTag);

        foreach (GameObject listingObject in listingObjects) {
            listings.Add(listingObject.GetComponent<WorldListing>());
        }
    }

    private void UpdateWorldListings() {
        UpdateEditButtonText();

        foreach(WorldListing listing in listings) {
            listing.IsEditing = isEditing;
        }
    }

    private void UpdateEditButtonText() {
        editButtonText.text = isEditing ? MenuProperties.editButtonTextEnabled : MenuProperties.editButtonTextDisabled;
        editButtonTextShadow.text = isEditing ? MenuProperties.editButtonTextEnabled : MenuProperties.editButtonTextDisabled;
    }
}
