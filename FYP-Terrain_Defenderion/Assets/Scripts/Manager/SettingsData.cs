using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField] private float m_MusicVolume = 1f;
    [SerializeField] private float m_MainVolume = 1f;
    [SerializeField] private ScreenType m_ScreenType;

    //More to place here
    public enum ScreenType
    {
        FullScreen,
        Window
    }
}
