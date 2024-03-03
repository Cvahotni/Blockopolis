using UnityEngine;

public class StaticCoroutineAccess : MonoBehaviour
{
    public static StaticCoroutineAccess Instance { get; private set; }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
}
