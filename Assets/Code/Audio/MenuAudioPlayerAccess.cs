using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioPlayerAccess : MonoBehaviour
{
    public void PlayButtonClick() {
        MenuAudioPlayer.Instance.PlayButtonClick();
    }
}
