using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    private bool m_ChangeSceneTriggered;

    public void ChangeScene(string sceneToLoad)
    {
        if (m_ChangeSceneTriggered)
            return;

        //SceneLoader.Instance.LoadScene(sceneToLoad);
        m_ChangeSceneTriggered = true;
    }

    public void StartJourney(string journeyToLoad)
    {
        if (m_ChangeSceneTriggered)
            return;

        //SceneLoader.Instance.StartJourney(journeyToLoad);
        m_ChangeSceneTriggered = true;
    }
}
