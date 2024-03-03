using UnityEngine;

public class NativeArrayCleaner : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() {
        BlockRegistry.OnDestroy();
        BlockModelRegistry.OnDestroy();
        FeatureRegistry.OnDestroy();
        NativeArrayExtension.OnDestroy();
    }
}
