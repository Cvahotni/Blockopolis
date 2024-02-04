using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Sound Group", menuName = "Voxel Engine/Block Sound Group")]
public class BlockSoundGroup : ScriptableObject
{
    [Header("Basic Information")]
    public BlockMaterial material;

    [Header("Sounds")]
    public AudioClip[] breakSounds;
    public AudioClip[] placeSounds;
    public AudioClip[] miningSounds;

    [Header("Volume")]
    public float breakSoundVolume = 1.0f;
    public float placeSoundVolume = 1.0f;
    public float miningSoundVolume = 1.0f;
}
