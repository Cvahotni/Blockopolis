using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStorage : MonoBehaviour
{
    public static PlayerStorage Instance { get; private set; }
    [SerializeField] public GameObject playerCamera;

    private static float xRotation = 0.0f;
    private static float yRotation = 0.0f;

    public static float XRotation { 
        get { return xRotation; }
        set { xRotation = value; }
    }

    public static float YRotation { 
        get { return yRotation; }
        set { yRotation = value; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void LoadPlayer(object sender, EventArgs e) {
        World world = WorldHandler.CurrentWorld;
        string path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);

        if(!WorldSaveLoad.DoesFileExist(path)) {
            Debug.Log("Failed to load player data: File doesn't exist.");
            return;
        }

        WorldPlayerSaveLoad.LoadWorldPlayer(world, gameObject);
    }

    public void SavePlayer(object sender, EventArgs e) {
        World world = WorldHandler.CurrentWorld;
        WorldPlayerSaveLoad.SaveWorldPlayer(world, gameObject);
    }
}
