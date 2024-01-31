using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = ("Setiings Data"), menuName = ("Create new Settings"))]
public class SettingsDataSO : ScriptableObject
{
    [SerializeField] private SettingsData settings;
    [SerializeField] private float m_MusicVolume = 1f;
    [SerializeField] private float m_MainVolume = 1f;
    [SerializeField] private ScreenType m_ScreenType;
    public float MusicVolume { get { return m_MusicVolume; } set { m_MusicVolume = value; } }
    public float MainVolume { get { return m_MainVolume; } set { m_MainVolume = value; } }
    public ScreenType screenType { get { return m_ScreenType; } set { m_ScreenType = value; } }

    public void SetData(SettingsData settingsData)
    {
        m_MainVolume = settingsData.MainVolume;
        m_MusicVolume = settingsData.MusicVolume;
        m_ScreenType = settingsData.screenType;
    }
    public SettingsData GetData()
    {
        SettingsData settingsData = new SettingsData();
        settingsData.MainVolume = m_MainVolume;
        settingsData.MusicVolume = m_MusicVolume;
        settingsData.screenType = m_ScreenType;
        return settingsData;
    }
}
