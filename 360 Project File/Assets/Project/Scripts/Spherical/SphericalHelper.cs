using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class SphericalHelper : MonoBehaviour
{
    private static readonly int k_BaseMapId = Shader.PropertyToID("_BaseMap");
    private static readonly int k_BaseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private Material m_MaterialRef;

    [SerializeField]
    private NavigationPoint[] m_NavPoints;
    public NavigationPoint[] NavPoints => m_NavPoints;

    private MeshRenderer m_Renderer;
    private Material m_Material;

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
    }

    private void Start()
        => m_Renderer.material = m_Material;
}
