using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField] private float m_MusicVolume = 1f;
    [SerializeField] private float m_MainVolume = 1f;
    [SerializeField] private ScreenType m_ScreenType = ScreenType.FullScreen;
    public float MusicVolume { get { return m_MusicVolume; }set { m_MusicVolume = value; } }
    public float MainVolume { get { return m_MainVolume; } set { m_MainVolume = value; } }
    public ScreenType screenType { get { return m_ScreenType; } set { m_ScreenType = value; } }
    //More to place here

}
public enum ScreenType
{
    FullScreen,
    Window
}