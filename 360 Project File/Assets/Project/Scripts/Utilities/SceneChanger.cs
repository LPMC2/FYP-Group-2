using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneChanger : MonoBehaviour
{
    private bool m_ChangeSceneTriggered;

    public void ChangeScene(string sceneToLoad)
    {
        if (m_ChangeSceneTriggered)
            return;

        Addressables.LoadSceneAsync(sceneToLoad);
        m_ChangeSceneTriggered = true;
    }
}
