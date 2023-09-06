using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class SwitchControl : MonoBehaviour
{
    [SerializeField] private int id;
    IEnumerator SetLocale()
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }

    public void CallSetLocale()
    {
        StartCoroutine(SetLocale());
    }
}
