using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStorage : MonoBehaviour
{
    public static PlayerStorage Instance { get; private set; }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        LoadPlayer();
    }

    private void LoadPlayer() {
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
