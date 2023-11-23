using UnityEngine;

[CreateAssetMenu]
public class Map : ScriptableObject
{
    [SerializeField]
    private MapFloor[] m_Floors;
    public MapFloor[] Floors => m_Floors;

    [SerializeField]
    private MapConnection[] m_Connections;
    public MapConnection[] Connections => m_Connections;

    [System.Serializable]
    public struct MapConnection
    {
        public MapLandmark from, to;
        public MapConnectionFlags flags;
        public JourneyEntrySO unlockingJourneyEntrySO;
    }

    [System.Flags]
    public enum MapConnectionFlags
    {
        None = 0,
        OneWayOnly = 1 << 0,
        Teleport = 1 << 1,
    }
}
