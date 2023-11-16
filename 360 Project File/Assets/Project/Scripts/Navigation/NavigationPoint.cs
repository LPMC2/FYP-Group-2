using System.IO;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavigationPoint : MonoBehaviour
{
    public Type NavigationType { get; set; }
    public Map.MapConnectionFlags Flags { get; set; }

    // Navigate
    public SphericalHelper Destination { get; set; }
    
    // Information
    public string InfoLocalizeKey { get; set; }

    // Video
    public bool PlayVideo { get; set; }
    public string VideoPath { get; set; }
    public string FullVideoPath => Path.Combine(Application.streamingAssetsPath, VideoPath);

    // Detection
    private Collider m_Collider;
    public Bounds Bounds => m_Collider.bounds;

    private void Awake()
        => m_Collider = GetComponent<Collider>();

    public enum Type
    {
        Navigate,
        Information
    }
}
