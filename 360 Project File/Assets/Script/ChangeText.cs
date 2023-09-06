using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ChangeText : MonoBehaviour
{
    public TMP_FontAsset asset1;
    //public TMP_FontAsset asset2;
    public TMP_Text[] Text1;
    //bool FontAssetNum = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeFont()
    {
        for (int i = 0; i < Text1.Length; i++)
        {
            Text1[i].font = asset1;
        }
    }
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
