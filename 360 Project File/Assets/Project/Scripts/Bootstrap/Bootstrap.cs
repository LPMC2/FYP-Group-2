using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

public class Bootstrap : MonoBehaviour
{
    [SerializeField]
    private AssetReference m_SceneToLoad;

    private IEnumerator Start()
    {
        var addressables = Addressables.InitializeAsync();
        yield return addressables;

        var localization = LocalizationSettings.InitializationOperation;
        yield return localization;

        //SceneLoader.Instance.LoadScene(m_SceneToLoad);
    }
}
