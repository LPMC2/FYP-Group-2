using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Singleton;
    [SerializeField] private string location = "/Settings/Data";
    [SerializeField] private string dataName = "SettingsData";
    private string settingsLocation = "";
    [SerializeField] private  SettingsData settingsData;
    public SettingsData SettingsData { get { return settingsData; } }
    // Start is called before the first frame update
    private void Awake()
    {
        Singleton = this;
    }
    void Start()
    {
        FolderManager.CreateFolder(location);
        settingsLocation = Application.persistentDataPath + location + dataName + ".json";
        //Load Data
        if(File.Exists(settingsLocation))
        {
            settingsData = JsonSerializer.LoadData<SettingsData>(settingsLocation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
