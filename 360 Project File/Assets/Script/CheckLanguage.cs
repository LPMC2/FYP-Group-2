using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CheckLanguage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //LocalizationSettings.SelectedLocale;
        Debug.Log(LocalizationSettings.SelectedLocale);
    }
    public string getLanguage()
    {
        // currentSelectedLocale = LocalizationSettings.SelectedLocale;
        //string a = (string) currentSelectedLocale;
        //if(a == "English(United States)(en - US)")
        //{

        //}
        
        return "";
    }
}
