using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneChanger : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private SceneLoaderEventChannelSO m_SceneLoaderEventChannel;

    private bool m_ChangeSceneTriggered;

    public void ChangeScene(string sceneToLoad)
    {
        if (m_ChangeSceneTriggered)
            return;

        m_SceneLoaderEventChannel.OnChangeScene?.Invoke(new AssetReference(sceneToLoad));
        m_ChangeSceneTriggered = true;
    }

    public void StartJourney(string journeyToLoad)
    {
        if (m_ChangeSceneTriggered)
            return;

        m_SceneLoaderEventChannel.OnStartJourney?.Invoke(new AssetReference(journeyToLoad));
        m_ChangeSceneTriggered = true;
    }
}
