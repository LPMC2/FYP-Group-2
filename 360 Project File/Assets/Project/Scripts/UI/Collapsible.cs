using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class CollapsiblePanel : MonoBehaviour
{
    [Header("Collapsible")]
    [SerializeField]
    private Button m_ToggleButton;
    [SerializeField]
    private RectTransform m_ContentRectTransform;
    [SerializeField]
    private AnimationCurve m_AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 0.35f, 1f);

    protected abstract float ContentHeight { get; }

    private bool m_Visible;
    private Coroutine m_Animation;

    protected virtual void OnEnable()
        => m_ToggleButton.onClick.AddListener(ToggleVisibility);

    protected virtual void OnDisable()
        => m_ToggleButton.onClick.RemoveListener(ToggleVisibility);

    public void ToggleVisibility()
        => SetVisibility(!m_Visible);

    private void SetVisibility(bool newState)
    {
        if (m_Visible == newState || m_Animation != null)
            return;

        m_Animation = StartCoroutine(PerformVisibility(newState));
    }

    private IEnumerator PerformVisibility(bool newState)
    {
        var from = newState ? 0f : ContentHeight;
        var to = newState ? ContentHeight : 0f;
        float time = 0f;
        while (time < m_AnimationCurve.GetLastKeyTime())
        {
            var value = Mathf.Lerp(from, to, m_AnimationCurve.Evaluate(time));
            m_ContentRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, value);
            time += Time.deltaTime;
            yield return null;
        }
        m_ContentRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, to);

        m_Visible = newState;
        m_Animation = null;
    }
}
