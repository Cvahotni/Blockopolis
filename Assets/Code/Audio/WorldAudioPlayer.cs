using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WorldAudioPlayer : MonoBehaviour
{
    public static WorldAudioPlayer Instance { get; private set; }

    [SerializeField] private float despawnTime = 10.0f;
    [SerializeField] private float deleteTime = 0.15f;

    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private float itemPickupSoundVolume = 0.5f;
    
    private WaitForSeconds despawnShortTime;
    private WaitForSeconds deleteShortTime;

    private Dictionary<AudioSource, float> audioSourceMap = new Dictionary<AudioSource, float>();
    private int miningSoundIndex = 0;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        despawnShortTime = new WaitForSeconds(despawnTime);
        deleteShortTime = new WaitForSeconds(deleteTime);
    }

    public void PlayBlockBreak(object sender, BlockModifyData data) {
        PlayBlockAudio(data, BlockAudioType.Break);
    }

    public void PlayBlockPlace(object sender, BlockModifyData data) {
        PlayBlockAudio(data, BlockAudioType.Place);
    }

    public void PlayBlockMining(object sender, BlockModifyData data) {
        PlayBlockAudio(data, BlockAudioType.Mining);
    }

    public void PlayItemPickup(object sender, ItemPickupData data) {
        PlayWorldAudio(data.position, itemPickupSound, itemPickupSoundVolume, false);
    }

    private void PlayBlockAudio(BlockModifyData data, BlockAudioType type) {
        Vector3 position = new Vector3(data.x, data.y, data.z);

        BlockMaterial blockMaterial = BlockRegistry.GetMaterialForBlock(data.block);
        BlockSoundGroup blockSoundGroup = BlockSoundGroupRegistry.GetBlockSound(blockMaterial);

        if(blockSoundGroup.breakSounds == null || blockSoundGroup.placeSounds == null || blockSoundGroup.miningSounds == null) return;

        switch(type) {
            case BlockAudioType.Break: {
                int breakSoundsSize = blockSoundGroup.breakSounds.Length;
                if(breakSoundsSize == 0) return;

                int breakRandom = UnityEngine.Random.Range(0, breakSoundsSize);
                PlayWorldAudio(position, blockSoundGroup.breakSounds[breakRandom], blockSoundGroup.breakSoundVolume, false);

                break;
            }

            case BlockAudioType.Place: {
                int placeSoundsSize = blockSoundGroup.placeSounds.Length;
                if(placeSoundsSize == 0) return;

                int placeRandom = UnityEngine.Random.Range(0, placeSoundsSize);
                PlayWorldAudio(position, blockSoundGroup.placeSounds[placeRandom], blockSoundGroup.placeSoundVolume, false);

                break;
            }

            case BlockAudioType.Mining: {
                int miningSoundsSize = blockSoundGroup.miningSounds.Length;

                if(miningSoundsSize == 0) return;
                if(miningSoundIndex >= miningSoundsSize) miningSoundIndex = 0;

                PlayWorldAudio(position, blockSoundGroup.miningSounds[miningSoundIndex], blockSoundGroup.miningSoundVolume, true);
                miningSoundIndex++;

                break;
            }

            default: break;
        }
    }

    public void RemoveInterruptableAudioSources(object sender, EventArgs e) {
        for(int i = audioSourceMap.Count - 1; i >= 0; i--) {
            AudioSource source = audioSourceMap.Keys.ElementAt(i);
            if(source == null) continue;
            if(source.volume == 0.0f) continue;

            source.volume = 0.0f;
            StartCoroutine(DestroyAudioSourceCoroutine(source.gameObject, true));
        }

        audioSourceMap.Clear();
    }

    private void PlayWorldAudio(Vector3 position, AudioClip sound, float volume, bool interrupt) {
        GameObject audioSourceObject = new GameObject("World Audio Source");
        audioSourceObject.transform.position = position;

        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

        audioSource.volume = volume;
        audioSource.clip = sound;

        if(interrupt) audioSourceMap.Add(audioSource, volume);
        audioSource.Play();

        StartCoroutine(DestroyAudioSourceCoroutine(audioSourceObject, false));
    }

    private IEnumerator DestroyAudioSourceCoroutine(GameObject audioObject, bool delete) {
        yield return delete ? deleteShortTime : despawnShortTime;
        Destroy(audioObject);
    }
}
