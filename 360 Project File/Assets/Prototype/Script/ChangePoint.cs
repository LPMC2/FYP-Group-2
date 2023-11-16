using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangePoint : MonoBehaviour
{
    //public GameObject Test;
    public GameObject[] CanvesObjects;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject[] CanvesObject = FindObjectOfType<GameObject>();

        //Debug.Log(Test.GetType());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hidePoint()
    {
        foreach (GameObject CanvesObject in CanvesObjects)
        {
            CanvesObject.SetActive(false);
        }
    }


}
