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
}
