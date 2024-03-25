using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using InputDeviceControl.Manager;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Singleton;
    private bool isActive = true;
    private bool forceVisibleCursor = false;
    public bool VisibleCursor { get {return forceVisibleCursor; } set { forceVisibleCursor = value; } }
    public bool ActiveState { get { return isActive; } set { isActive = value; } }
    public static GameObject Obj;
    [SerializeField] private string location = "/Settings/Data";
    [SerializeField] private string dataName = "SettingsData";
    private string settingsLocation = "";
    [SerializeField]
     private SettingsDataSO settingsData;
    public SettingsDataSO SettingsData { get { return settingsData; } }
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject bg;
    [SerializeField] private InputAction openMenuButton;
    [SerializeField] private EasyTween menuAnimation;
    [Header("Reference Settings Object")]
    [SerializeField] private ReferenceObject<Slider> m_MusicSlider = new ReferenceObject<Slider>();
    [SerializeField] private ReferenceObject<Slider> m_MainSlider = new ReferenceObject<Slider>();
    [SerializeField] private TMP_Dropdown m_GraphicsTypeDropdown;
    [SerializeField] private TMP_Dropdown m_GraphicsQualityDropDown;
    [SerializeField] private ReferenceObject<Slider> m_POVSlider = new ReferenceObject<Slider>();
    [SerializeField] private ReferenceObject<Slider> m_SensitivitySlider = new ReferenceObject<Slider>();
    private CinemachineBrain m_cinemachineBrain;
    private CinemachineVirtualCamera[] m_cinemachineVirtualCameras;
    private Camera[] m_camera;
    CameraManager[] inputManager;
    FlightController[] flightControllers;
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
        if (!isActive) return;
        menuAnimation.OpenCloseObjectAnimation();
        if(!forceVisibleCursor)
            InputDeviceManager.SetDevice(menuAnimation.animationParts.GetStatus());
        bg.SetActive(menuAnimation.animationParts.GetStatus());
        SetSettingsData();
    }
    public void OnClick()
    {
        if (!isActive) return;
        menuAnimation.OpenCloseObjectAnimation();
        if (!forceVisibleCursor)
            InputDeviceManager.SetDevice(menuAnimation.animationParts.GetStatus());
        bg.SetActive(menuAnimation.animationParts.GetStatus());
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
        m_camera = GameObject.FindObjectsOfType<Camera>(true);
       
        m_cinemachineVirtualCameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>(true);
        inputManager = GameObject.FindObjectsOfType<CameraManager>(true);
        flightControllers = GameObject.FindObjectsOfType<FlightController>(true);
    }
    
    void Start()
    {
        FolderManager.CreateFolder(location);
        settingsLocation = Application.persistentDataPath + location + dataName + ".json";
        //Load Data
        if(File.Exists(settingsLocation))
        {
            LoadSettingsData();
            UpdateAll();
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
        UpdateAll();
    }
    public void SaveSettingsData()
    {
        SettingsData settingsData1 = settingsData.GetData();

        JsonSerializer.SaveData(settingsData1, settingsLocation);
    }
    // Update is called once per frame
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    void Update()
    {
        /*
        if (Time.time >= cacheExpirationTimer)
        {
            if (File.Exists(settingsLocation) && !SettingsPanel.activeInHierarchy)
            {
                UpdatePovValue(settingsData.GetData().POV.Value, false);
                UpdateSensitivityValue(settingsData.GetData().Sensitivity.Value, false);
            }
            cacheExpirationTimer = Time.time + cacheExpirationTime;
        }
        */
       
    }
    public void SetGraphicsQuality(int index)
    {
        switch (index)
        {
            case 0:
                settingsData.Settings.GraphicsQualityType.Value = GraphicsType.Normal;
                break;
            case 1:
                settingsData.Settings.GraphicsQualityType.Value = GraphicsType.Low;
                break;
        }
        SaveSettingsData();
        UpdateGraphicsQuality();
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
    public void UpdateAll()
    {
        UpdateUI();
        UpdateMainVolume(settingsData.GetData().MainVolume.Value);
        UpdateMusicVolume(settingsData.GetData().MusicVolume.Value);
        UpdatePovValue(settingsData.GetData().POV.Value);
        UpdateSensitivityValue(settingsData.GetData().Sensitivity.Value);
        UpdateGraphics();
    }
    public void UpdateUI()
    {
        m_MainSlider.TargetObject.value = settingsData.Settings.MainVolume.Value;
        m_MusicSlider.TargetObject.value = settingsData.Settings.MusicVolume.Value;
        m_GraphicsTypeDropdown.value = (int)settingsData.Settings.screenType.Value;
        m_GraphicsQualityDropDown.value = (int)settingsData.Settings.GraphicsQualityType.Value;
        m_POVSlider.TargetObject.value = settingsData.Settings.POV.Value;
        m_SensitivitySlider.TargetObject.value = settingsData.Settings.Sensitivity.Value;
    }
    public void UpdateMainVolume(float value)
    {
        settingsData.Settings.MainVolume.Value = value;
        m_MainSlider.DisplayText = Mathf.Ceil(value).ToString();
        //foreach(AudioSource audioSource in GameObject.FindObjectsOfType<AudioSource>())
        //{
        //    audioSource.volume = value;
        //}
        AudioManager.Singleton.SetSFXVolume(value/100f);
        SaveSettingsData();
    }
    public void UpdateMusicVolume(float value)
    {
        settingsData.Settings.MusicVolume.Value = value;
        m_MusicSlider.DisplayText = Mathf.Ceil(value).ToString();
        AudioManager.Singleton.SetVolume(value/100f);
        SaveSettingsData();
    }
    public void UpdatePovValue(float value, bool saveData = true)
    {
        settingsData.Settings.POV.Value = value;
        m_POVSlider.DisplayText = value.ToString();

                foreach (Camera camera in m_camera)
                {
                    camera.fieldOfView = value;
                }

                foreach (CinemachineVirtualCamera cinemachineVirtualCamera in m_cinemachineVirtualCameras)
                {
                    cinemachineVirtualCamera.m_Lens.FieldOfView = value;
                }
            
        
        if(saveData)
        SaveSettingsData();
    }
    public void TriggerPovValue(float value) { UpdatePovValue(value, true); }
    public void UpdateSensitivityValue(float value, bool saveData = true)
    {
        settingsData.Settings.Sensitivity.Value = value;
        m_SensitivitySlider.DisplayText = Mathf.Ceil(value * 100).ToString();
        SetControlSensitivity(value);
        if(saveData)
        SaveSettingsData();
    }
    public void TriggerSensitivityValue(float value) { UpdateSensitivityValue(value, true); }
    private void SetControlSensitivity(float value)
    {
        if(inputManager.Length > 0)
        {
            foreach(CameraManager inputManager1 in inputManager)
            {
                inputManager1.CameraLookSpeed = value * 2;
            }
        }
        if(flightControllers.Length > 0)
        {
            foreach(FlightController flightController in flightControllers)
            {
                flightController.MouseSensitivity = value;
            }
        }
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
    public void UpdateGraphicsQuality()
    {
        UniversalAdditionalCameraData[] universalAdditionalCameraDatas = FindObjectsOfType<UniversalAdditionalCameraData>(true);
        foreach (UniversalAdditionalCameraData universalAdditionalCameraData in universalAdditionalCameraDatas)
        {
            switch (settingsData.Settings.GraphicsQualityType.Value)
            {
                case GraphicsType.Normal:
                    if (universalAdditionalCameraData != null)
                    {
                        universalAdditionalCameraData.renderPostProcessing = true;
                    }
                    break;
                case GraphicsType.Low:
                    if (universalAdditionalCameraData != null)
                    {
                        universalAdditionalCameraData.renderPostProcessing = false;
                    }
                    break;
            }
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
