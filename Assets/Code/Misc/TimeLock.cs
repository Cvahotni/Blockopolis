using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeLock
{
    private static float currentTime;

    public static void Lock(object sender, EventArgs e) {
        currentTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    public static void Unlock(object sender, EventArgs e) {
        Time.timeScale = currentTime;
    }
}
