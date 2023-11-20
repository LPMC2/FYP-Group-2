using UnityEngine;

public class NavigationDebug : MonoBehaviour
{
    [SerializeField]
    private JourneySO m_Journey;

    [Header("Event Channels")]
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannel;

    private void Start()
    {
        m_NavigationEventChannel.OnLoadMap(m_Journey.Map, m_Journey.StartPoint);
        m_JourneyEventChannel.OnLoadEntries(m_Journey.Entries);
        Destroy(this);
    }
}
