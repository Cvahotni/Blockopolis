using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLock
{
    private static float currentTime;

    public static void Lock() {
        currentTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    public static void Unlock() {
        Time.timeScale = currentTime;
    }
}
