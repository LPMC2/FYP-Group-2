using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsReferencer : MonoBehaviour
{
    public GameObject GetSettingsObj()
    {
        return SettingsManager.Obj;
    }
    public void ToggleSettingsPanel()
    {
        if(SettingsManager.Singleton != null)
        SettingsManager.Singleton.OnClick();
    }
}
