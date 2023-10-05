using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField]
    private AssetReference m_SceneToLoad;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        m_SceneToLoad.LoadSceneAsync(LoadSceneMode.Additive).Completed += OnSceneLoadComplete;
    }

    private void OnSceneLoadComplete(AsyncOperationHandle<SceneInstance> scene)
    {
        SceneManager.UnloadSceneAsync(0);
    }
}
