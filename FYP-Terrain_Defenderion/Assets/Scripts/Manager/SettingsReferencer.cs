using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsReferencer : MonoBehaviour
{
    [SerializeField] private bool forceVisibleCursor = false;
    public GameObject GetSettingsObj()
    {
        return SettingsManager.Obj;
    }

    private void Update()
    {
        if(SettingsManager.Singleton != null && SettingsManager.Singleton.VisibleCursor != forceVisibleCursor)
        {
            SettingsManager.Singleton.VisibleCursor = forceVisibleCursor;
        }
    }
    public void ToggleSettingsPanel()
    {
        if (SettingsManager.Singleton != null)
        {
            SettingsManager.Singleton.OnClick();

        }

    }
    public void SetIsActive(bool state)
    {
        if(SettingsManager.Singleton != null)
        {
            SettingsManager.Singleton.ActiveState = state;
        }
    }
}
