using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField]
    private AssetReference m_ManagerScene, m_SceneToLoad;

    [Header("Event Channels")]
    [SerializeField]
    private AssetReference m_SceneLoaderEventChannel;

    private IEnumerator Start()
    {
        var addressables = Addressables.InitializeAsync();
        yield return addressables;

        var localization = LocalizationSettings.InitializationOperation;
        yield return localization;

        var managerScene = m_ManagerScene.LoadSceneAsync(LoadSceneMode.Additive);
        yield return managerScene;

        var eventChannelHandle = m_SceneLoaderEventChannel.LoadAssetAsync<SceneLoaderEventChannelSO>();
        yield return eventChannelHandle;

        eventChannelHandle.Result.OnChangeScene?.Invoke(m_SceneToLoad);
        SceneManager.UnloadSceneAsync(0);
    }
}
