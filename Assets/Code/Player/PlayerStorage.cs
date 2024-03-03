using UnityEngine;
using System;

public class PlayerStorage : MonoBehaviour
{
    public static PlayerStorage Instance { get; private set; }
    [SerializeField] public GameObject playerCamera;

    private static float xRotation = 0.0f;
    private static float yRotation = 0.0f;

    private World world;
    private string path;

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

        world = WorldHandler.CurrentWorld;
        path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);

        LoadPlayerData();
    }

    public void LoadPlayerData() {
        if(!WorldSaveLoad.DoesFileExist(path)) {
            Debug.Log("Failed to load player data: File doesn't exist.");
            return;
        }

        WorldPlayerSaveLoad.LoadWorldPlayer(world, gameObject);
    }

    public void LoadPlayerRotation(object sender, EventArgs e) {
        if(!WorldSaveLoad.DoesFileExist(path)) {
            Debug.Log("Failed to load player rotation: File doesn't exist.");
            return;
        }

        WorldPlayerSaveLoad.LoadWorldPlayerRotation(world);
    }

    public void SavePlayer(object sender, EventArgs e) {
        World world = WorldHandler.CurrentWorld;
        WorldPlayerSaveLoad.SaveWorldPlayer(world, gameObject);
    }
}
