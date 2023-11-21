using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Events/Scene Loader")]
public class SceneLoaderEventChannelSO : ScriptableObject
{
    public LoadingProgressUpdatedAction OnLoadingProgressUpdated;

    public ChangeSceneAction OnChangeScene;
    public StartJourneyAction OnStartJourney;
}

public delegate void LoadingProgressUpdatedAction(float percentage);

public delegate void ChangeSceneAction(AssetReference sceneToLoad);
public delegate void StartJourneyAction(AssetReference journeyToLoad);
