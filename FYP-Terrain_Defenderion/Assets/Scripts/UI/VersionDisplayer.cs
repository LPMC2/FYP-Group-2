using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class VersionDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text display;
    // Start is called before the first frame update
    void Start()
    {
        if(display ==null)
        {
            display = GetComponent<TMP_Text>();
        }
        if(display != null)
        {
            display.text = "V"+Application.version.ToString();
        }
    }

 
}
