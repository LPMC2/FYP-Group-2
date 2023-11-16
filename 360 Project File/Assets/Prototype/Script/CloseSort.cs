using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSort : MonoBehaviour
{
    bool ispress = false;
    [SerializeField]
    private GameObject gb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void onClick()
    {
        if (ispress)
        {
            ispress = !ispress;
            gb.SetActive(ispress);
        }
        else
        {
            ispress = !ispress;
            gb.SetActive(ispress);
        }
    }

    // Update is called once per frame
    void Update()
    {
         
    }
}
