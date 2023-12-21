using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public LoadSceneMode loadSceneMode;
    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename, loadSceneMode);
    }
}

