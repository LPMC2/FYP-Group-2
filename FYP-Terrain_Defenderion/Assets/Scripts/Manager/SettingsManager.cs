using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Singleton;
    public static GameObject Obj;
    [SerializeField] private string location = "/Settings/Data";
    [SerializeField] private string dataName = "SettingsData";
    private string settingsLocation = "";
    [SerializeField]
     private SettingsDataSO settingsData;
    public SettingsDataSO SettingsData { get { return settingsData; } }
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private InputAction openMenuButton;
    [SerializeField] private EasyTween menuAnimation;
    [Header("Reference Settings Object")]
    [SerializeField] private ReferenceObject<Slider> m_MusicSlider = new ReferenceObject<Slider>();
    [SerializeField] private ReferenceObject<Slider> m_MainSlider = new ReferenceObject<Slider>();
    [SerializeField] private TMP_Dropdown m_GraphicsTypeDropdown;
    [SerializeField] private ReferenceObject<Slider> m_POVSlider = new ReferenceObject<Slider>();
    [SerializeField] private ReferenceObject<Slider> m_SensitivitySlider = new ReferenceObject<Slider>();

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
        SetSettingsData();
    }
    public void OnClick()
    {
        menuAnimation.OpenCloseObjectAnimation();
        SetSettingsData();
    }
    private void SetSettingsData()
    {

    }
    // Start is called before the first frame update
    private void Awake()
    {
        Singleton = this;
        Obj = this.gameObject;
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
        m_MainSlider.TargetObject.value = settingsData.Settings.MainVolume.Value;
        m_MusicSlider.TargetObject.value = settingsData.Settings.MusicVolume.Value;
        m_GraphicsTypeDropdown.value = (int)settingsData.Settings.screenType.Value;

        m_POVSlider.TargetObject.value = settingsData.Settings.POV.Value;
        m_SensitivitySlider.TargetObject.value = settingsData.Settings.Sensitivity.Value;
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
                settingsData.Settings.screenType.Value =  ScreenType.FullScreen;
                break;
            case 1:
                settingsData.Settings.screenType.Value = ScreenType.Window;
                break;
        }
        SaveSettingsData();
        UpdateGraphics();
    }
    #region -----------------------------Update Data-----------------------------
    public void UpdateUI()
    {

    }
    public void UpdateMainVolume(float value)
    {
        settingsData.Settings.MainVolume.Value = value;
        m_MainSlider.DisplayText = Mathf.Ceil(value * 100).ToString();
        SaveSettingsData();
    }
    public void UpdateMusicVolume(float value)
    {
        settingsData.Settings.MusicVolume.Value = value;
        m_MusicSlider.DisplayText = Mathf.Ceil(value*100).ToString();
        SaveSettingsData();
    }
    public void UpdatePovValue(float value)
    {
        settingsData.Settings.POV.Value = value;
        m_POVSlider.DisplayText = value.ToString();
        SaveSettingsData();
    }
    public void UpdateSensitivityValue(float value)
    {
        settingsData.Settings.Sensitivity.Value = value;
        m_SensitivitySlider.DisplayText = (value * 10).ToString();
        SaveSettingsData();
    }
    public void UpdateGraphics()
    {
        switch(settingsData.Settings.screenType.Value)
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
    [System.Serializable]
    public class ReferenceObject<T>
    {
        [SerializeField] private T value;
        public T TargetObject { get { return value; } set { this.value = value; } }
        [SerializeField] private TMP_Text displayText;
        public string DisplayText { get { if (displayText != null) return displayText.text; else return ""; } set { if (displayText != null) displayText.text = value; else return; } }

    }
}
