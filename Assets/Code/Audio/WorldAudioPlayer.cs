using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldAudioPlayer : MonoBehaviour
{
    public static WorldAudioPlayer Instance { get; private set; }

    [SerializeField] private float despawnTime = 10.0f;
    [SerializeField] private float muteTime = 0.25f;

    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private float itemPickupSoundVolume = 0.5f;
    
    private WaitForSeconds despawnShortTime;
    private WaitForSeconds muteShortTime;

    private Dictionary<AudioSource, float> audioObjects = new Dictionary<AudioSource, float>();
    private List<GameObject> interuptableObjects = new List<GameObject>();

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        despawnShortTime = new WaitForSeconds(despawnTime);
        muteShortTime = new WaitForSeconds(muteTime);
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
        PlayWorldAudio(data.position, itemPickupSound, itemPickupSoundVolume, false, false);
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
                PlayWorldAudio(position, blockSoundGroup.breakSounds[breakRandom], blockSoundGroup.breakSoundVolume, false, false);

                break;
            }

            case BlockAudioType.Place: {
                int placeSoundsSize = blockSoundGroup.placeSounds.Length;
                if(placeSoundsSize == 0) return;

                int placeRandom = UnityEngine.Random.Range(0, placeSoundsSize);
                PlayWorldAudio(position, blockSoundGroup.placeSounds[placeRandom], blockSoundGroup.placeSoundVolume, false, false);

                break;
            }

            case BlockAudioType.Mining: {
                int miningSoundsSize = blockSoundGroup.miningSounds.Length;
                if(miningSoundsSize == 0) return;

                for(int i = 0; i < miningSoundsSize; i++) {
                    PlayWorldAudio(position, blockSoundGroup.miningSounds[i], blockSoundGroup.miningSoundVolume, true, true);
                }

                break;
            }

            default: break;
        }
    }

    private void PlayWorldAudio(Vector3 position, AudioClip sound, float volume, bool interuptable, bool loop) {
        GameObject audioSourceObject = new GameObject("World Audio Source");
        audioSourceObject.transform.position = position;

        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

        audioSource.volume = volume;
        audioSource.clip = sound;
        audioSource.loop = loop;

        if(interuptable) {
            interuptableObjects.Add(audioSourceObject);
        }

        audioObjects.Add(audioSource, volume);

        audioSource.Play();
        StartCoroutine(DestroyAudioSourceCoroutine(audioSourceObject));
    }

    public void DestroyExistingMiningSounds(object sender, EventArgs e) {
        DestroyExistingMiningSounds();
    }

    public void DestroyExistingMiningSounds(object sender, BlockModifyData data) {
        DestroyExistingMiningSounds();
    }

    public void MuteAllWorldAudio(object sender, EventArgs e) {
        SetAudioMute(true);
    }

    public void UnmuteAllWorldAudio(object sender, EventArgs e) {
        SetAudioMute(false);
        audioObjects.Clear();
    }

    private void SetAudioMute(bool mute) {
        foreach(var pair in audioObjects) {
            AudioSource audioSource = pair.Key;
            float volume = pair.Value;

            if(audioSource == null) continue;

            if(mute) audioSource.volume = 0.0f;
            else audioSource.volume = volume;
        }
    }

    private void DestroyExistingMiningSounds() {
        foreach(GameObject interruptableGameObject in interuptableObjects) {
            if(interruptableGameObject == null) continue;

            AudioSource audioSource = interruptableGameObject.GetComponent<AudioSource>();
            StartCoroutine(MuteAudioSourceCoroutine(interruptableGameObject, audioSource));
        }
    }

    private IEnumerator MuteAudioSourceCoroutine(GameObject sourceObject, AudioSource source) {
        for(int i = 0; i < 20; i++) {
            yield return muteShortTime;

            if(source == null) break;
            source.volume -= 0.05f;
        }

        interuptableObjects.Remove(sourceObject);
        Destroy(sourceObject);
    }

    private IEnumerator DestroyAudioSourceCoroutine(GameObject audioObject) {
        yield return despawnShortTime;
        Destroy(audioObject);
    }
}
