using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Sound Group", menuName = "Voxel Engine/Block Sound Group")]
public class BlockSoundGroup : ScriptableObject
{
    public BlockMaterial material;

    public AudioClip[] breakSounds;
    public float breakSoundVolume = 1.0f;

    public AudioClip[] placeSounds;
    public float placeSoundVolume = 1.0f;
}
