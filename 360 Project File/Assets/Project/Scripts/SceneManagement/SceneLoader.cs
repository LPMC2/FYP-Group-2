using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private AssetReference m_ViewerScene;

    [Header("Event Channels")]
    [SerializeField]
    private SceneLoaderEventChannelSO m_SceneLoaderEventChannel;
    [SerializeField]
    private LoadingProgressEventChannelSO m_LoadingProgressEventChannel;
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannel;

    private bool m_IsLoading;
    private AssetReference m_SceneToLoad;
    private AssetReference m_JourneyToLoad;
    private AsyncOperationHandle<SceneInstance> m_CurrentSceneHandle;
    private AsyncOperationHandle<JourneySO> m_CurrentJourneyHandle;
    private SceneInstance m_CurrentSceneInstance;

    private void Awake()
    {
        m_CurrentSceneInstance = new();
    }

    private void OnEnable()
    {
        m_SceneLoaderEventChannel.OnChangeScene += LoadScene;
        m_SceneLoaderEventChannel.OnStartJourney += StartJourney;
    }

    private void OnDisable()
    {
        m_SceneLoaderEventChannel.OnChangeScene -= LoadScene;
        m_SceneLoaderEventChannel.OnStartJourney -= StartJourney;
    }

    private void LoadScene(AssetReference sceneToLoad)
    {
        if (m_IsLoading)
            return;

        m_SceneToLoad = sceneToLoad;
        m_IsLoading = true;

        StartCoroutine(UnloadPreviousScene());
    }

    private void StartJourney(AssetReference journeyToLoad)
    {
        m_JourneyToLoad = journeyToLoad;
        LoadScene(m_ViewerScene);
    }

    private IEnumerator UnloadPreviousScene()
    {
        m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0f);
        var waitTime = m_LoadingProgressEventChannel.OnFade?.Invoke(false);
        if (waitTime.HasValue)
            yield return new WaitForSeconds(waitTime.Value);

        if (m_CurrentSceneInstance.Scene != null && m_CurrentSceneInstance.Scene.isLoaded)
            yield return Addressables.UnloadSceneAsync(m_CurrentSceneHandle);

        yield return LoadNewScene();
    }

    private IEnumerator LoadNewScene()
    {
        m_CurrentSceneHandle = Addressables.LoadSceneAsync(m_SceneToLoad, LoadSceneMode.Additive);
        yield return m_CurrentSceneHandle;
        m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0.5f);

        yield return OnNewSceneLoaded();
    }

    private IEnumerator OnNewSceneLoaded()
    {
        m_CurrentSceneInstance = m_CurrentSceneHandle.Result;
        SceneManager.SetActiveScene(m_CurrentSceneInstance.Scene);

        if (m_SceneToLoad == m_ViewerScene && m_JourneyToLoad != null)
        {
            m_CurrentJourneyHandle = m_JourneyToLoad.LoadAssetAsync<JourneySO>();
            yield return m_CurrentJourneyHandle;
            m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0.6f);

            var journey = m_CurrentJourneyHandle.Result;
            m_NavigationEventChannel.OnLoadMap(journey.Map, journey.StartPoint, journey.StartRotation);
            yield return new WaitForSeconds(0.15f);
            m_JourneyEventChannel.OnLoadEntries(journey.Entries);
            yield return new WaitForSeconds(0.15f);
            m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0.8f);
        }
        else if (m_CurrentJourneyHandle.IsValid())
        {
            Addressables.Release(m_CurrentJourneyHandle);
            m_JourneyToLoad = null;
            m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0.8f);
            yield return new WaitForSeconds(0.3f);
        }

        m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(1f);
        yield return new WaitForSeconds(0.5f);

        m_IsLoading = false;
        m_LoadingProgressEventChannel.OnFade.Invoke(true);
    }
}
