using System.IO;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavigationPoint : MonoBehaviour
{
    [SerializeField]
    private Type m_Type;
    public Type NavigationType => m_Type;

    [SerializeField]
    private SphericalHelper m_Destination;
    public SphericalHelper Destination => m_Destination;

    [SerializeField]
    private string m_InfoLocalizeKey;
    public string InfoLocalizeKey => m_InfoLocalizeKey;

    private Collider m_Collider;
    public Bounds Bounds => m_Collider.bounds;

    [SerializeField]
    private bool m_PlayVideo;
    public bool PlayVideo => m_PlayVideo;
    [SerializeField]
    private string m_StreamingAssetPath;
    public string VideoPath => Path.Combine(Application.streamingAssetsPath, m_StreamingAssetPath);

    private void Awake()
        => m_Collider = GetComponent<Collider>();

    public enum Type
    {
        Navigate,
        Information
    }
}
