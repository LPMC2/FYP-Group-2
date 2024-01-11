using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextManager : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string[] displayText;
    [SerializeField] private DisplayBehaviour displayBehaviour;
    private void Awake()
    {
        displayBehaviour = gameObject.GetComponent<DisplayBehaviour>();
    }
    public void DisplayText(int index)
    {
        if(index<displayText.Length)
        displayBehaviour.StartFadeInText(displayText[index]);
    }
}
