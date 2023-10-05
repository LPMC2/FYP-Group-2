using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class SphericalHelper : MonoBehaviour
{
    private static int k_BaseMapId;
    private static int k_BaseColorId;

    [SerializeField]
    private Material m_MaterialRef;

    [SerializeField]
    private Texture m_Texture;

    [SerializeField]
    private NavigationPoint[] m_NavPoints;
    public NavigationPoint[] NavPoints => m_NavPoints;

    private MeshRenderer m_Renderer;
    private Material m_Material;

    public float Alpha
    {
        get => m_Material.GetColor(k_BaseColorId).a;
        set
        {
            if (k_BaseColorId == 0)
                k_BaseColorId = Shader.PropertyToID("_BaseColor");

            var newColor = m_Material.GetColor(k_BaseColorId);
            newColor.a = value;
            m_Material.SetColor(k_BaseColorId, newColor);
        }
    }

    private void Awake()
    {
        if (k_BaseMapId == 0)
            k_BaseMapId = Shader.PropertyToID("_BaseMap");

        m_Renderer = GetComponent<MeshRenderer>();
        m_Material = Instantiate(m_MaterialRef);
        if (m_Texture != null)
            m_Material.SetTexture(k_BaseMapId, m_Texture);
    }

    private void Start()
        => m_Renderer.material = m_Material;

#if UNITY_EDITOR
    private void Update()
    {
        if (k_BaseMapId == 0)
            k_BaseMapId = Shader.PropertyToID("_BaseMap");

        if (m_Material.GetTexture(k_BaseMapId) != m_Texture)
            m_Material.SetTexture(k_BaseMapId, m_Texture);
    }
#endif
}
