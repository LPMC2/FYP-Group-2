using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private string location = "/Settings/Data";
    private string settingsLocation = "";
    // Start is called before the first frame update
    void Start()
    {
        FolderManager.CreateFolder(location);
        settingsLocation = Application.persistentDataPath + location;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
