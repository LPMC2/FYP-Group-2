using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavigationPoint : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;

    public SphericalHelper Destination { get; set; }
    public Map.MapConnectionFlags Flags { get; set; }

    private Collider m_Collider;
    public Bounds Bounds => m_Collider.bounds;

    private void Awake()
        => m_Collider = GetComponent<Collider>();

    public void Navigate()
    {
        var mode = NavigationManager.NavigationMode.Move;
        if (Flags.HasFlag(Map.MapConnectionFlags.Teleport))
            mode = NavigationManager.NavigationMode.Teleport;
        m_NavigationEventChannel.OnNavigate(Destination, mode);
    }
}
