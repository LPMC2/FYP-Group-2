using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("Stage")]
    [SerializeField]
    private StageSO m_StartStage;

    [SerializeField]
    private Transform m_StageObjectParent;

    [Header("Skybox")]
    [SerializeField]
    [Range(0.5f, 2.5f)]
    private float m_AutoRotateSpeed;

    [Header("UI")]
    [SerializeField]
    private Image m_UIOverlay;

    public bool AutoRotation { get; set; }

    private Material m_SkyboxMaterial;
    private StageSO m_CurrentStage;
    private bool m_ChangingStage;

    private void Awake()
    {
        m_SkyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
    }

    private void Start()
    {
        RenderSettings.skybox = m_SkyboxMaterial;
        m_UIOverlay.color = new Color(0f, 0f, 0f, 1f);
        ChangeStage(m_StartStage);
    }

    private void Update()
    {
        if (AutoRotation)
        {
            var value = m_SkyboxMaterial.GetFloat("_Rotation");
            value = Mathf.Clamp(value + Time.deltaTime * m_AutoRotateSpeed, 0f, 360f);
            if (Mathf.Approximately(value, 360f))
                value = 0f;
            m_SkyboxMaterial.SetFloat("_Rotation", value);
        }
    }

    public void ChangeStage(StageSO stage)
    {
        if (m_CurrentStage == stage)
            return;

        if (m_ChangingStage)
        {
            Debug.LogWarning("Please wait for current stage change to complete before changing to the next.");
            return;
        }

        StartCoroutine(PerformStageChange(stage));
    }

    private bool m_ChangeSceneTriggered;
    public void ChangeScene(string sceneName)
    {
        if (m_ChangeSceneTriggered)
            return;

        SceneManager.LoadSceneAsync(sceneName);
        m_ChangeSceneTriggered = true;
    }

    private IEnumerator PerformStageChange(StageSO newStage)
    {
        m_ChangingStage = true;

        // Fade out
        var color = new Color(0f, 0f, 0f, 0f);
        var currentTexture = m_SkyboxMaterial.GetTexture("_Tex");
        if (currentTexture != null)
        {
            m_UIOverlay.color = color;
            while (!Mathf.Approximately(m_UIOverlay.color.a, 1f))
            {
                color.a = Mathf.Clamp(color.a + Time.deltaTime * 2f, 0f, 1f);
                m_UIOverlay.color = color;
                yield return null;
            }
            color.a = 1f;
            m_UIOverlay.color = color;
        }

        // Change skybox
        m_SkyboxMaterial.SetTexture("_Tex", newStage.m_SkyboxTexture);
        yield return null;

        // Fade in
        color = new Color(0f, 0f, 0f, 1f);
        m_UIOverlay.color = color;
        while (!Mathf.Approximately(m_UIOverlay.color.a, 0f))
        {
            color.a = Mathf.Clamp(color.a - Time.deltaTime * 2f, 0f, 1f);
            m_UIOverlay.color = color;
            yield return null;
        }
        color.a = 0f;
        m_UIOverlay.color = color;

        m_ChangingStage = false;
    }
}
