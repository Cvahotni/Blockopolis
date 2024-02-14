using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWaterEffect : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Volume volume;

    [SerializeField] private VolumeProfile normalProfile;
    [SerializeField] private VolumeProfile underwaterProfile;

    [SerializeField] private float underwaterOffset = 0.88f;
    [SerializeField] private byte waterID;

    private PlayerEventSystem playerEventSystem;
    private bool underwater = false;

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
    }

    private void FixedUpdate() {
        CheckUnderwater();
    }

    private void CheckUnderwater() {
        bool bodyUnderWater = WorldAccess.GetBlockAt(playerBody.position).id == waterID;
        bool cameraUnderWater = WorldAccess.GetBlockAt(playerCamera.position).id == waterID;

        if(cameraUnderWater) volume.sharedProfile = underwaterProfile;
        if(bodyUnderWater) playerEventSystem.InvokePlayerEnterWater();

        if(!cameraUnderWater) volume.sharedProfile = normalProfile;
        if(!bodyUnderWater) playerEventSystem.InvokePlayerExitWater();
    }
}
