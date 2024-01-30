using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockSoundGroupRegistry
{
    private static List<BlockSoundGroup> blockSoundGroups = new List<BlockSoundGroup>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        LoadBlockSoundGroupsFromFolder();
    }

    private static void LoadBlockSoundGroupsFromFolder() {
        UnityEngine.Object[] objects = Resources.LoadAll("Block Sound Groups");
        foreach(UnityEngine.Object currentObject in objects) blockSoundGroups.Add((BlockSoundGroup) currentObject);
    }

    public static BlockSoundGroup GetBlockSound(BlockMaterial material) {
        foreach(BlockSoundGroup group in blockSoundGroups) {
            if(group.material == material) return group;
        }

        throw new NullReferenceException("Failed to find block sound group for material: " + material);
    } 
}