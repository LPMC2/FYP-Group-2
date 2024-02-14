using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = ("Setiings Data"), menuName = ("Create new Settings"))]
public class SettingsDataSO : ScriptableObject
{
    [SerializeField] private SettingsData settings;
    public SettingsData Settings { get { return settings; } }

    public void SetData(SettingsData settingsData)
    {
        settings.SetData(settingsData);
    }
    public SettingsData GetData()
    {
        
        return settings.GetData();
    }
}
