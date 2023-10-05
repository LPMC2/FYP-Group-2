using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private bool m_ChangeSceneTriggered;

    public void ChangeScene(string sceneName)
    {
        if (m_ChangeSceneTriggered)
            return;

        SceneManager.LoadSceneAsync(sceneName);
        m_ChangeSceneTriggered = true;
    }
}
