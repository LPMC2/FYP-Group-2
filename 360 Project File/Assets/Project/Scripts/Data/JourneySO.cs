using UnityEngine;

[CreateAssetMenu]
public class JourneySO : ScriptableObject
{
    [SerializeField]
    private Map m_Map;
    public Map Map => m_Map;

    [SerializeField]
    private MapLandmark m_StartPoint;
    public MapLandmark StartPoint => m_StartPoint;
    [SerializeField]
    private Vector3 m_StartRotation;
    public Vector3 StartRotation => m_StartRotation;

    [SerializeField]
    private JourneyEntrySO[] m_Entries;
    public JourneyEntrySO[] Entries => m_Entries;
}
