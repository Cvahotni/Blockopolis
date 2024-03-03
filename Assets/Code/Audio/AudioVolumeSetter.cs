using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeSetter : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeName = "Master";

    private void Start() {
        DontDestroyOnLoad(this);
    }

    private void FixedUpdate() {   
        audioMixer.SetFloat(masterVolumeName, Mathf.Log10(GameSettings.Volume / 100.0f) * 30.0f);
    }
}
