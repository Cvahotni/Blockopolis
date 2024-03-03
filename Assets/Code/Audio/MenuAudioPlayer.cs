using UnityEngine;

public class MenuAudioPlayer : MonoBehaviour
{
    public static MenuAudioPlayer Instance { get; private set; }
    [SerializeField] private AudioSource menuAudioSource;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        DontDestroyOnLoad(this);
    }

    public void PlayButtonClick() {
        menuAudioSource.Play();
    }
}
