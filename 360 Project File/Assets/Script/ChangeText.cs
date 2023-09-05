using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
}
