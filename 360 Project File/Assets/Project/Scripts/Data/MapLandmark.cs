using UnityEngine;

[CreateAssetMenu]
public class MapLandmark : ScriptableObject
{
    [SerializeField]
    private Vector2 m_Position;
    public Vector2 Position => m_Position;

    [SerializeField]
    private Vector3 m_Rotation;
    public Vector3 Rotation => m_Rotation;

    [SerializeField]
    private Texture2D m_Texture;
    public Texture2D Texture => m_Texture;

    [SerializeField]
    private MapLandmarkFlags m_Flags;
    public MapLandmarkFlags Flags => m_Flags;

    [System.Flags]
    public enum MapLandmarkFlags
    {
        None = 0,
        Locked = 1 << 0
    }
}
