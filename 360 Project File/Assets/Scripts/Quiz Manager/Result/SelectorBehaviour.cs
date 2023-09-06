using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite correctImg;
    [SerializeField] private Sprite incorrectImg;
    // Start is called before the first frame update
    [SerializeField] private GameObject BackgroundObj;
    [SerializeField] private GameObject imgObj;
    [SerializeField] private GameObject textObj;
    public void setImg(bool correct)
    {
        if(correct == true)
        {
            imgObj.GetComponent<Image>().sprite = correctImg;
           

        } else
        {
            imgObj.GetComponent<Image>().sprite = incorrectImg;
        }
    }
    public void setQNumber(int index)
    {
        textObj.GetComponent<TMP_Text>().text = index.ToString();
    }
}
