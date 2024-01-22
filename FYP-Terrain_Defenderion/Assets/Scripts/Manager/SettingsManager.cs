using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
