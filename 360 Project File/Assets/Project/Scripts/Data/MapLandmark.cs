using UnityEngine;
using UnityEngine.Localization;

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
    private LocalizedString m_DisplayNameKey;
    public LocalizedString DisplayNameKey => m_DisplayNameKey;

    [SerializeField]
    private MapLandmarkAction[] m_Actions;
    public MapLandmarkAction[] Actions => m_Actions;

    [System.Serializable]
    public struct MapLandmarkAction
    {
        public LocalizedString actionStringKey;
        public JourneyEntrySO journeyEntry;
    }
}
