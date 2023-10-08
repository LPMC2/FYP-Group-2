using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class InputFieldManager : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    void OnEnable()
    {
        //Register InputField Event
        inputField.onValueChanged.AddListener(inputValueChanged);
    }


    static string CleanInput(string strIn)
    {
        // Replace invalid characters with empty strings.
        return Regex.Replace(strIn,
              @"[^a-zA-Z0-9-_]", "");
    }

    //Called when Input changes
    void inputValueChanged(string attemptedVal)
    {
        inputField.text = CleanInput(attemptedVal);
    }
    
    void OnDisable()
    {
        //Un-Register InputField Events
        inputField.onValueChanged.RemoveAllListeners();
    }
}
