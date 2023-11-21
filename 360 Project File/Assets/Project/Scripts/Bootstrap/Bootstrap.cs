using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

public class Bootstrap : MonoBehaviour
{
    [SerializeField]
    private AssetReference m_SceneToLoad;

    [Header("Event Channels")]
    [SerializeField]
    private SceneLoaderEventChannelSO m_SceneLoaderEventChannel;

    private IEnumerator Start()
    {
        var addressables = Addressables.InitializeAsync();
        yield return addressables;

        var localization = LocalizationSettings.InitializationOperation;
        yield return localization;

        m_SceneLoaderEventChannel.OnChangeScene?.Invoke((string)m_SceneToLoad.RuntimeKey);
    }
}
