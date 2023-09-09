using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
public class ChangeText : MonoBehaviour
{
    public TMP_FontAsset asset1;

    public void SetFont()
    {
        TMP_Text[] textComponents = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text textComponent in textComponents)
        {
            if (!textComponent.transform.CompareTag("Unchangeable"))
            {
                textComponent.font = asset1;
            }
        }
    }

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
    public void ChangeLanguage()
    {
        CallSetLocale();
        SetFont();
    }
}
