using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public string SceneName = "";
    public bool isAsync = false;
    public LoadSceneMode loadSceneMode;
    [SerializeField]
    private float progressValue = 0f;
    public void SetSceneName(string value)
    {
        SceneName = value;
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName, loadSceneMode);
    }
    public void StartLoadSceneAsync()
    {
        StartCoroutine(LoadSceneAsync(SceneName));
    }
    private IEnumerator LoadSceneAsync(string scenename)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenename, loadSceneMode);
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f); // Normalize the progress value
            progressValue = progress;

            yield return null;
        }
    }
    public void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}

