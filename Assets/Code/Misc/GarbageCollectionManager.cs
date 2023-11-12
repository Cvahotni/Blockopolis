using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class GarbageCollectionManager : MonoBehaviour
{
    [SerializeField] private float maxTimeBetweenGC = 60.0f;
    private float timeSinceLastGC;

    private void Start() {
        #if !UNITY_EDITOR && !UNITY_WEBGL
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        #endif
    }

    private void Update() {
        timeSinceLastGC += Time.unscaledDeltaTime;
        if(timeSinceLastGC > maxTimeBetweenGC) CollectGarbage();
    }

    private void CollectGarbage() {
        timeSinceLastGC = 0f;

        #if !UNITY_EDITOR
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        System.GC.Collect();
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        #endif
    }
}
