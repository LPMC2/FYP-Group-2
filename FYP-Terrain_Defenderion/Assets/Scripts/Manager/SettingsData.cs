using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization;

[System.Serializable]
public class SettingsData
{
    [SerializeField] private DataType<float> m_MusicVolume = new DataType<float>(50f);
    [SerializeField] private DataType<float> m_MainVolume = new DataType<float>(50f);
    [SerializeField] private DataType<ScreenType> m_ScreenType = new DataType<ScreenType>(ScreenType.FullScreen);
    [SerializeField] private DataType<float> m_POV = new DataType<float>(60f);
    [SerializeField] private DataType<float> m_Sensitivity = new DataType<float>(.5f);
    [OptionalField(VersionAdded = 2)]
    [SerializeField] private DataType<GraphicsType> m_graphicType = new DataType<GraphicsType>(GraphicsType.Normal);
    public DataType<float> MusicVolume { get { return m_MusicVolume; }set { m_MusicVolume = value; } }
    public DataType<float> MainVolume { get { return m_MainVolume; } set { m_MainVolume = value; } }
    public DataType<ScreenType> screenType { get { return m_ScreenType; } set { m_ScreenType = value; } }
    public DataType<float> POV { get { return m_POV; } set { m_POV = value; } }
    public DataType<float> Sensitivity { get { return m_Sensitivity;} set { m_Sensitivity = value; } }
    public DataType<GraphicsType> GraphicsQualityType { get { return m_graphicType; } set { m_graphicType = value; } }
    //More to place here
    public void SetData(SettingsData settingsData)
    {
        m_MainVolume.Value = settingsData.MainVolume.Value;
        m_MusicVolume.Value = settingsData.MusicVolume.Value;
        m_ScreenType.Value = settingsData.screenType.Value;
        m_POV.Value = settingsData.POV.Value;
        m_Sensitivity.Value = settingsData.Sensitivity.Value;
        m_graphicType.Value = settingsData.m_graphicType.Value;
    }
    public SettingsData GetData()
    {
        
        return this;
    }
    [System.Serializable]
    public class DataType<T1>
    {
        [SerializeField]
        private T1 value = default;
        public T1 Value { get { return value; } set { this.value = value; } }
        public DataType(T1 defaultValue = default) 
        {
            value = defaultValue;
         
        }
       
    }
}
public enum ScreenType
{
    FullScreen,
    Window
}
public enum GraphicsType
{
    Normal,
    Low
}