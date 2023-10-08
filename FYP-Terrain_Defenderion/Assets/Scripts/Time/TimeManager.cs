using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static void StopTime()
    {
        Time.timeScale = 0;
    }
    public static void SetTime(float value)
    {
        if(value > 1 || value < 0)
        {
           Debug.LogError("Input time value is outside the allowed range.");
            return;
        }
        Time.timeScale = value;
    }
    public static void ResumeTime()
    {
        Time.timeScale = 1;
    }
}
