using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemTextureHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] renderers;

    private void Start() {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        renderers = meshRenderers;
    }

    public void SetMaterials(Material material) {
        foreach(MeshRenderer renderer in renderers) {
            renderer.sharedMaterial = material;
        }
    }
}
