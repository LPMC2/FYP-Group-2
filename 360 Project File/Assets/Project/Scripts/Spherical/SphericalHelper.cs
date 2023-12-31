using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class SphericalHelper : MonoBehaviour
{
    private static readonly int k_BaseMapId = Shader.PropertyToID("_BaseMap");
    private static readonly int k_BaseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private Material m_MaterialRef;

    private LocalizedString m_DisplayNameKey;

    private List<NavigationPoint> m_NavPoints;
    public NavigationPoint[] NavPoints => m_NavPoints.ToArray();
    public MapLandmark.MapLandmarkAction[] Actions { get; set; }

    private MeshRenderer m_Renderer;
    private Material m_Material;

    public LocalizedString DisplayNameKey
    {
        get => m_DisplayNameKey;
        set => m_DisplayNameKey = value;
    }
    
    public Texture Texture
    {
        get => m_Material.GetTexture(k_BaseMapId);
        set => m_Material.SetTexture(k_BaseMapId, value);
    }

    public float Alpha
    {
        get => m_Material.GetColor(k_BaseColorId).a;
        set
        {
            var newColor = m_Material.GetColor(k_BaseColorId);
            newColor.a = value;
            m_Material.SetColor(k_BaseColorId, newColor);
        }
    }

    private void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Material = Instantiate(m_MaterialRef);
        m_NavPoints = new();
    }

    private void Start()
        => m_Renderer.material = m_Material;

    public void AddNavigationPoint(NavigationPoint navigationPoint)
    {
        var direction = navigationPoint.Destination.transform.position - transform.position;
        navigationPoint.transform.parent = transform;
        navigationPoint.transform.localPosition = transform.InverseTransformDirection(direction.normalized);
        m_NavPoints.Add(navigationPoint);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (var navPoint in m_NavPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, navPoint.Destination.transform.position);
        }
    }
#endif
}
