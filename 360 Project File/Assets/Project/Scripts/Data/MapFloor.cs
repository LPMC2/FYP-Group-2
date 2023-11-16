using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapFloor : ScriptableObject
{
    [SerializeField]
    private Texture2D m_MapTexture;
    public Texture2D MapTexture => m_MapTexture;

    [SerializeField]
    private List<MapLandmark> m_Landmarks;
    public List<MapLandmark> Landmarks => m_Landmarks;

    [SerializeField]
    private LandmarkConnection[] m_Connections;
    public LandmarkConnection[] Connections => m_Connections;

    public bool Contains(MapLandmark mapLandmark)
        => m_Landmarks.Contains(mapLandmark);

    [System.Serializable]
    public struct LandmarkConnection
    {
        public MapLandmark from, to;
        public LandmarkConnectionFlags flags;
    }

    [System.Flags]
    public enum LandmarkConnectionFlags
    {
        None = 0,
        OneWayOnly = 1 << 0,
        Teleport = 1 << 1,
    }
}
