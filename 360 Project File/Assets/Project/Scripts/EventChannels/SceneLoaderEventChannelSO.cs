using UnityEngine;

[CreateAssetMenu(menuName = "Events/Scene Loader")]
public class SceneLoaderEventChannelSO : ScriptableObject
{
    public LoadingProgressUpdatedAction OnLoadingProgressUpdated;

    public ChangeSceneAction OnChangeScene;
    public StartJourneyAction OnStartJourney;
}

public delegate void LoadingProgressUpdatedAction(float percentage);

public delegate void ChangeSceneAction(string sceneToLoad);
public delegate void StartJourneyAction(string journeyToLoad);
