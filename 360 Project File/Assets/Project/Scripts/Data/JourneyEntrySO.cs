using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class JourneyEntrySO : ScriptableObject
{
    [SerializeField]
    private LocalizedString m_StepKey;
    public LocalizedString StepKey => m_StepKey;

    [SerializeField]
    private JourneyType m_JourneyType;
    public JourneyType Type => m_JourneyType;

    [SerializeField]
    private MapLandmark m_Destination;
    public MapLandmark Destination => m_Destination;

    public bool Done { get; set; }

    public enum JourneyType
    {
        None = 0,
        ReachDestination = 1,
        ReceiveItem = 2
    }
}
