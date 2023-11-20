using System.Collections;
using UnityEngine;

public class LoadingProgress : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;
    [SerializeField]
    private RectTransform m_Indicator, m_Fill;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_FadeAnim = AnimationCurve.EaseInOut(0f, 0f, 0.35f, 1f);

    private float m_IndicatorWidth;
    private Coroutine m_Fade;
    public bool Animating => m_Fade != null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_CanvasGroup.alpha = 0f;
        m_CanvasGroup.blocksRaycasts = false;
        m_IndicatorWidth = m_Indicator.rect.width;
    }

    private void OnEnable()
    {
        SceneLoader.loadProgressUpdated += OnLoadingProgressUpdated;
    }

    private void OnDisable()
    {
        SceneLoader.loadProgressUpdated -= OnLoadingProgressUpdated;
    }

    public void Fade(bool fadeOut = false)
    {
        if (m_Fade != null)
            return;

        m_Fade = StartCoroutine(PerformFade(fadeOut));
    }

    private IEnumerator PerformFade(bool fadeOut)
    {
        // Always block raycast regradless of direction
        m_CanvasGroup.blocksRaycasts = true;

        var fromColor = fadeOut ? 1f : 0f;
        var toColor = fadeOut ? 0f : 1f;

        var time = 0f;
        while (time < m_FadeAnim.GetLastKeyTime())
        {
            m_CanvasGroup.alpha = Mathf.Lerp(fromColor, toColor, m_FadeAnim.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        m_CanvasGroup.alpha = toColor;
        m_CanvasGroup.blocksRaycasts = !fadeOut;

        // Reset indicator on fade out
        if (fadeOut)
            m_Fill.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, 0f);

        m_Fade = null;
    }

    private void OnLoadingProgressUpdated(float percentage)
        => m_Fill.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, m_IndicatorWidth * percentage);
}
