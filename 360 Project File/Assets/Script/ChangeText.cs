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
                textComponent.fontMaterial = asset1.material;
                Debug.Log(textComponent.text);
            }
        }
    }
    public void InitialFont()
    {
        GameObject managerObj = GameObject.FindGameObjectWithTag("Manager");
        QuizUIManager quizUIManager = managerObj.GetComponent<QuizUIManager>();
        GameObject engUI = GameObject.FindGameObjectWithTag("EngUI");
        GameObject hkUI = GameObject.FindGameObjectWithTag("HKUI");
        GameObject cnUI = GameObject.FindGameObjectWithTag("CNUI");
        if (quizUIManager != null)
        {
            switch(quizUIManager.lang)
            {
                case Language.en:
                    id = 0;
                    engUI.GetComponent<ChangeText>().SetFont();
                    break;
                case Language.zh_HK:
                    id = 1;
                    hkUI.GetComponent<ChangeText>().SetFont();
                    break;
                case Language.zh_CN:
                    id = 2;
                    cnUI.GetComponent<ChangeText>().SetFont();
                    break;
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
