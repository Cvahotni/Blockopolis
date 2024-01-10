using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldAudioPlayer : MonoBehaviour
{
    public static WorldAudioPlayer Instance { get; private set; }

    [SerializeField] private float despawnTime = 10.0f;

    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private float itemPickupSoundVolume = 0.5f;
    
    private WaitForSeconds despawnShortTime;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        despawnShortTime = new WaitForSeconds(despawnTime);
    }

    public void PlayBlockBreak(object sender, BlockModifyData data) {
        PlayBlockAudio(data, BlockAudioType.Break);
    }

    public void PlayBlockPlace(object sender, BlockModifyData data) {
        PlayBlockAudio(data, BlockAudioType.Place);
    }

    public void PlayItemPickup(object sender, ItemPickupData data) {
        PlayWorldAudio(data.position, itemPickupSound, itemPickupSoundVolume);
    }

    private void PlayBlockAudio(BlockModifyData data, BlockAudioType type) {
        Vector3 position = new Vector3(data.x, data.y, data.z);

        BlockMaterial blockMaterial = BlockRegistry.GetMaterialForBlock(data.block);
        BlockSoundGroup blockSoundGroup = BlockSoundGroupRegistry.GetBlockSound(blockMaterial);

        if(blockSoundGroup.breakSounds == null || blockSoundGroup.placeSounds == null) return;

        switch(type) {
            case BlockAudioType.Break: {
                int breakSoundsSize = blockSoundGroup.breakSounds.Length;
                if(breakSoundsSize == 0) return;

                int breakRandom = UnityEngine.Random.Range(0, breakSoundsSize);
                PlayWorldAudio(position, blockSoundGroup.breakSounds[breakRandom], blockSoundGroup.breakSoundVolume);

                break;
            }

            case BlockAudioType.Place: {
                int placeSoundsSize = blockSoundGroup.placeSounds.Length;
                if(placeSoundsSize == 0) return;

                int placeRandom = UnityEngine.Random.Range(0, placeSoundsSize);
                PlayWorldAudio(position, blockSoundGroup.placeSounds[placeRandom], blockSoundGroup.placeSoundVolume);

                break;
            }

            default: break;
        }
    }

    private void PlayWorldAudio(Vector3 position, AudioClip sound, float volume) {
        GameObject audioSourceObject = new GameObject("World Audio Source");
        audioSourceObject.transform.position = position;

        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

        audioSource.volume = volume;
        audioSource.clip = sound;

        audioSource.Play();
        StartCoroutine(DestroyAudioSourceCoroutine(audioSourceObject));
    }

    private IEnumerator DestroyAudioSourceCoroutine(GameObject audioObject) {
        yield return despawnShortTime;
        Destroy(audioObject);
    }
}
