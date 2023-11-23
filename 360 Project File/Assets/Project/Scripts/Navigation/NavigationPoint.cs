using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavigationPoint : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannelSO;
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;
    [SerializeField]
    private NavigationPointUIEventChannelSO m_NavigationPointUIEventChannel;

    public SphericalHelper Destination { get; set; }
    public Map.MapConnectionFlags Flags { get; set; }
    public JourneyEntrySO UnlockingJourneyEntry { get; set; }

    private Collider m_Collider;
    public Bounds Bounds => m_Collider.bounds;

    private void Awake()
        => m_Collider = GetComponent<Collider>();

    private void OnEnable()
        => m_JourneyEventChannelSO.OnEntryCompleted += OnEntryCompleted;

    private void OnDisable()
        => m_JourneyEventChannelSO.OnEntryCompleted -= OnEntryCompleted;

    private void OnEntryCompleted(JourneyEntrySO entry)
    {
        if (entry != UnlockingJourneyEntry)
            return;

        UnlockingJourneyEntry = null;
    }

    public void Navigate()
    {
        if (UnlockingJourneyEntry != null)
        {
            m_NavigationPointUIEventChannel.OnDisplayUnlockJourneyEntry?.Invoke(UnlockingJourneyEntry);
            return;
        }    

        var mode = NavigationManager.NavigationMode.Move;
        if (Flags.HasFlag(Map.MapConnectionFlags.Teleport))
            mode = NavigationManager.NavigationMode.Teleport;
        m_NavigationEventChannel.OnNavigate(Destination, mode);
    }
}
