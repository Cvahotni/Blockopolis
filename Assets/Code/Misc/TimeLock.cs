using UnityEngine;
using System;

public class TimeLock
{
    private static float currentTime;

    public static void Lock() {
        currentTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    public static void Lock(object sender, EventArgs e) {
        Lock();
    }

    public static void Unlock() {
        Time.timeScale = currentTime;
    }

    public static void Unlock(object sender, EventArgs e) {
        Unlock();
    }
}
