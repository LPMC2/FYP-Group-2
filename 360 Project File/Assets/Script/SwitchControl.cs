using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class SwitchControl : MonoBehaviour
{
    private void Start()
    {
       
    }
    IEnumerator SetLocale(int id)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }

    public void LangTo0()
    {
        StartCoroutine(SetLocale(0));
    }
    public void LangTo1()
    {
        StartCoroutine(SetLocale(1));
    }
    public void LangTo2()
    {
        StartCoroutine(SetLocale(2));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
