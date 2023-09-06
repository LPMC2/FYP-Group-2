using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoTo360WorldOrGoBack : MonoBehaviour
{
    [SerializeField] 
    private GameObject go;


    private bool ispress= false;

    public void gonext()
    {
        if(!ispress)
        {
            SceneManager.LoadScene("360World");
            ispress = true;
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
        ispress = true;

    }
}
