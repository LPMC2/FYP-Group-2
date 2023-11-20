using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
    public static event UnityAction<float> loadProgressUpdated;

    private Coroutine m_Load;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(AssetReference sceneToLoad, bool skipFadeIn = false)
    {
        if (m_Load != null)
            return;

        m_Load = StartCoroutine(PerformLoad((string)sceneToLoad.RuntimeKey, skipFadeIn));
    }

    public void LoadScene(string sceneToLoad, bool skipFadeIn = false)
    {
        if (m_Load != null)
            return;

        m_Load = StartCoroutine(PerformLoad(sceneToLoad, skipFadeIn));
    }

    private IEnumerator PerformLoad(string sceneToLoad, bool skipFadeIn)
    {
        LoadingProgress loadingProgress = null;
        if (loadingProgress != null && !skipFadeIn)
        {
            loadingProgress.Fade();
            yield return new WaitUntil(() => !loadingProgress.Animating);
        }

        loadProgressUpdated?.Invoke(0f);
        var load = Addressables.LoadSceneAsync(sceneToLoad);
        while (!load.IsDone)
        {
            loadProgressUpdated?.Invoke(load.PercentComplete);
            yield return null;
        }
        loadProgressUpdated?.Invoke(1f);

        if (loadingProgress != null)
        {
            loadingProgress.Fade(true);
            yield return new WaitUntil(() => !loadingProgress.Animating);
        }

        m_Load = null;
    }

    public void StartJourney(string journeyToLoad)
    {
    }
}
