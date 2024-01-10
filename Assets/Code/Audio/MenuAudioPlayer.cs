using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuAudioPlayer : MonoBehaviour
{
    public static MenuAudioPlayer Instance { get; private set; }
    [SerializeField] private AudioSource menuAudioSource;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void PlayButtonClick() {
        menuAudioSource.Play();
    }
}
