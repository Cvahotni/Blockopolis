using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeArrayCleaner : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy() {
        BlockRegistry.OnDestroy();
        BlockModelRegistry.OnDestroy();
        FeatureRegistry.OnDestroy();
        NativeArrayExtension.OnDestroy();
    }
}
