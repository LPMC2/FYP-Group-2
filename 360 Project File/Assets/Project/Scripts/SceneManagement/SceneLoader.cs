using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneLoader : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private SceneLoaderEventChannelSO m_SceneLoaderEventChannel;

    private Coroutine m_Load;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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

    public void LoadScene(AssetReference sceneToLoad)
    {
        if (m_Load != null)
            return;

        m_Load = StartCoroutine(PerformLoad((string)sceneToLoad.RuntimeKey));
    }

    public void LoadScene(string sceneToLoad)
    {
        if (m_Load != null)
            return;

        m_Load = StartCoroutine(PerformLoad(sceneToLoad));
    }

    private IEnumerator PerformLoad(string sceneToLoad)
    {
        LoadingProgress loadingProgress = null;
        if (loadingProgress != null)
        {
            loadingProgress.Fade();
            yield return new WaitUntil(() => !loadingProgress.Animating);
        }

        m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(0f);
        var load = Addressables.LoadSceneAsync(sceneToLoad);
        while (!load.IsDone)
        {
            m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(load.PercentComplete);
            yield return null;
        }
        m_SceneLoaderEventChannel.OnLoadingProgressUpdated?.Invoke(1f);

        if (loadingProgress != null)
        {
            loadingProgress.Fade(true);
            yield return new WaitUntil(() => !loadingProgress.Animating);
        }

        m_Load = null;
    }

    public void StartJourney(string journeyToLoad)
    {
        Debug.Log($"StartJournet: {journeyToLoad}");
    }
}
