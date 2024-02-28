using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class ButtonOnClickDetector : MonoBehaviour
{
    [SerializeField] private SelectMethod selectMethod = SelectMethod.ACTIVE_SCENE;
    [SerializeField] private GameObject parentObj;
    [SerializeField] private List<string> m_audioOnClickList = new List<string>();
    [SerializeField] private TargetType types = TargetType.Button;
    Object[] buttons;
    Object[] sliders;
    Object[] toggles;
    Object[] dropDowns;
    private bool isInteracting = false;
    // Start is called before the first frame update
    void Start()
    {

        if (selectMethod == SelectMethod.ACTIVE_SCENE)
        {
            if((types & TargetType.Button) != 0)
                buttons = ScenesManager.FindObjectsOfTypeAll(typeof(Button));
            if((types & TargetType.Slider) != 0)
                sliders = ScenesManager.FindObjectsOfTypeAll(typeof(Slider));
            if((types & TargetType.Toggle) != 0)
                toggles = ScenesManager.FindObjectsOfTypeAll(typeof(Toggle));
            if ((types & TargetType.DropDown) != 0)
                dropDowns = ScenesManager.FindObjectsOfTypeAll(typeof(TMP_Dropdown));
        }
        else if (selectMethod == SelectMethod.GAMEOBJECTS)
        {
            if ((types & TargetType.Button) != 0)
                buttons = parentObj.transform.GetComponentsInChildren<Button>(true);
            if ((types & TargetType.Slider) != 0)
                sliders = parentObj.transform.GetComponentsInChildren<Slider>(true);
            if ((types & TargetType.Toggle) != 0)
                toggles = parentObj.transform.GetComponentsInChildren<Toggle>(true);
            if ((types & TargetType.DropDown) != 0)
                dropDowns = parentObj.transform.GetComponentsInChildren<TMP_Dropdown>(true);

        }
        if(buttons != null)
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(SetOnClickAudio);
        }
        if (sliders != null)
            foreach (Slider slider in sliders)
        {
            slider.onValueChanged.AddListener(SetOnClickAudio);
        }
        if (toggles != null)
            foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(SetOnClickAudio);
        }
        if (dropDowns != null)
            foreach (TMP_Dropdown dropdown in dropDowns)
        {
            dropdown.onValueChanged.AddListener(SetOnClickAudio);
        }
    }
    private void SetOnClickAudio()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            AudioManager.Singleton.RandomPlayOneShotSound(m_audioOnClickList);
            //Debug.Log("Button Clicked!");
        }
    }
    private void SetOnClickAudio<T>(T useless)
    {
        SetOnClickAudio();
    }
    public enum SelectMethod
    {
        ACTIVE_SCENE,
        GAMEOBJECTS
    }
    [System.Flags]
    public enum TargetType
    {
        None,
        Button,
        Slider,
        Toggle,
        DropDown
    }


}
