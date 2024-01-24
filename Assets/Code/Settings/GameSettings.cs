using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSettings
{
    private static int viewDistance = 16;
    private static int chunksPerSecond = 500;
    private static int featuresPerSecond = 500;
    private static int chunkPoolSize = 16384;
    private static int maxFramerate = 250;
    private static int chunkBuildsPerFrame = 2;
    private static bool enableVSync = true;
    private static bool fullscreen = false;

    private static readonly int minViewDistance = 2;
    private static readonly int maxViewDistance = 64;
    private static readonly int minChunksPerSecond = 50;
    private static readonly int maxChunksPerSecond = 1000;
    private static readonly int minChunkBuildsPerFrame = 1;
    private static readonly int maxChunkBuildsPerFrame = 4;
    private static readonly int minFeaturesPerSecond = 50;
    private static readonly int maxFeaturesPerSecond = 1500;
    private static readonly int minFramerate = 10;
    private static readonly int maxFramerateLimit = 250;

    private static readonly int chunksPerSecondMultiplier = 8;

    public static int ViewDistance {
        get { return viewDistance; }
        set { viewDistance = value; }
    }

    public static int ChunksPerSecond {
        get { return chunksPerSecond; }
        set { chunksPerSecond = value; }
    }

    public static int ChunkBuildsPerFrame {
        get { return chunkBuildsPerFrame; }
        set { chunkBuildsPerFrame = value; }
    }

    public static int FeaturesPerSecond {
        get { return featuresPerSecond; }
        set { featuresPerSecond = value; }
    }

    public static int ChunkPoolSize {
        get { return chunkPoolSize; }
        set { chunkPoolSize = value; }
    }

    public static int MaxFramerate {
        get { return maxFramerate; }
        set { maxFramerate = value; }
    }

    public static bool EnableVSync {
        get { return enableVSync; }
        set { enableVSync = value; }
    }

    public static bool Fullscreen {
        get { return fullscreen; }
        set { fullscreen = value; }
    }

    public static int ChunksPerSecondMultiplier {
        get { return chunksPerSecondMultiplier; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Start() {
        GameSettingsStorage.Load();
        ApplyChangesToUnity();
    }

    public static void SetViewDistance(int newViewDistance) {
        viewDistance = Mathf.Clamp(newViewDistance, minViewDistance, maxViewDistance);
        //chunkPoolSize = 64 * (viewDistance * 2);
    }

    public static void SetChunksPerSecond(int newChunksPerSecond) {
        chunksPerSecond = Mathf.Clamp(newChunksPerSecond, minChunksPerSecond, maxChunksPerSecond);
    }

    public static void SetFeaturesPerSecond(int newFeaturesPerSecond) {
        featuresPerSecond = Mathf.Clamp(newFeaturesPerSecond, minFeaturesPerSecond, maxFeaturesPerSecond);
    }

    public static void SetChunkBuildsPerFrame(int newChunkBuildsPerFrame) {
        chunkBuildsPerFrame = Mathf.Clamp(newChunkBuildsPerFrame, minChunkBuildsPerFrame, maxChunkBuildsPerFrame);
    }

    public static void SetMaxFramerate(int newMaxFramerate) {
        maxFramerate = Mathf.Clamp(newMaxFramerate, minFramerate, maxFramerateLimit);
    }

    public static void SetEnableVSync(bool newEnableVSync) {
        enableVSync = newEnableVSync;
        ApplyChangesToUnity();
    }

    public static void SetFullscreen(bool newFullScreen) {
        fullscreen = newFullScreen;
        ApplyChangesToUnity();
    }

    public static void ApplyChangesToUnity() {
        Application.targetFrameRate = maxFramerate;
        Screen.fullScreen = fullscreen;
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
    }
}
