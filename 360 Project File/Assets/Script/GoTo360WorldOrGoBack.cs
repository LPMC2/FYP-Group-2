using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoTo360WorldOrGoBack : MonoBehaviour
{
    private bool m_Switching;

    public void SwitchScene(string sceneName)
    {
        if (m_Switching)
            return;

        StartCoroutine(LoadScene(sceneName));
        m_Switching = true;
    }

    private IEnumerator LoadScene(string sceneName)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (asyncOperation.isDone)
            yield return null;
    }
}
