using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Singleton;
    [SerializeField] private string location = "/Settings/Data";
    [SerializeField] private string dataName = "SettingsData";
    private string settingsLocation = "";
    [SerializeField]
     private SettingsDataSO settingsData;
    public SettingsDataSO SettingsData { get { return settingsData; } }
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private InputAction openMenuButton;
    [SerializeField] private EasyTween menuAnimation;
    private void OnEnable()
    {
        openMenuButton.performed += OnClick;
        openMenuButton.Enable();
    }
    private void OnDisable()
    {
        // Disable and clean up the input action
        openMenuButton.Disable();
        openMenuButton.performed -= OnClick;
    }
    private void OnClick(InputAction.CallbackContext context)
    {
        menuAnimation.OpenCloseObjectAnimation();
    }
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
            LoadSettingsData();
        } else
        {
            GenerateSettingsData();
        }
    }
    public void GenerateSettingsData()
    {
        if (settingsLocation == "") return;
        JsonSerializer.SaveData(new SettingsData(), settingsLocation);
        LoadSettingsData();
    }
    public void LoadSettingsData()
    {
        settingsData.SetData(JsonSerializer.LoadData<SettingsData>(settingsLocation));
    }
    public void SaveSettingsData()
    {
        SettingsData settingsData1 = settingsData.GetData();
        JsonSerializer.SaveData(settingsData1, settingsLocation);
    }
    // Update is called once per frame
    void Update()
    {
   
    }
    public void SetGraphicsData(int index)
    {
        switch (index)
        {
            case 0:
                settingsData.screenType = ScreenType.FullScreen;
                break;
            case 1:
                settingsData.screenType = ScreenType.Window;
                break;
        }
        SaveSettingsData();
        UpdateGraphics();
    }
    #region -----------------------------Update Data-----------------------------
    public void UpdateUI()
    {

    }
    public void UpdateMainVolume()
    {

    }
    public void UpdateMusicVolume()
    {

    }
    public void UpdateGraphics()
    {
        switch(settingsData.screenType)
        {
            case ScreenType.FullScreen:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
            case ScreenType.Window:
                Screen.SetResolution(1920, 1080, false);
                break;
        }
    }
    #endregion
}
