using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField]
    private Texture m_SkyboxTexture;

    private Material m_SkyboxMaterial;

    private void Awake()
    {
        m_SkyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
        m_SkyboxMaterial.SetTexture("_Tex", m_SkyboxTexture);
    }

    private void Start()
    {
        RenderSettings.skybox = m_SkyboxMaterial;
    }

    private void Update()
    {
        var currentTexture = m_SkyboxMaterial.GetTexture("_Tex");
        if (currentTexture != m_SkyboxTexture)
            m_SkyboxMaterial.SetTexture("_Tex", m_SkyboxTexture);
    }

    public void ChangeTexture(Texture texture)
    {
        m_SkyboxTexture = texture;
    }
}
