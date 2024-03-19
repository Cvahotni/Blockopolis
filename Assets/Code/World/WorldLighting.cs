using UnityEngine;

public class WorldLighting : MonoBehaviour
{
    [Range(1.0f, 0.0f)]
    [SerializeField] 
    private float globalLightLevel = 1.0f;

    private void FixedUpdate() {
        Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);
    } 
}
