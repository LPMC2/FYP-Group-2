using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorPlan : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField]
    private Button m_ToggleButton;
    [SerializeField]
    private RectTransform m_ContentRectTransform;
    [SerializeField]
    private float m_ContentHeight;

    [Header("Animation")]
    [SerializeField]
    private float m_VisibilityTime;

    private bool m_Visible;
    private Coroutine m_Visibility;

    private void OnEnable()
        => m_ToggleButton.onClick.AddListener(ToggleVisibility);

    private void OnDisable()
        => m_ToggleButton.onClick.RemoveListener(ToggleVisibility);

    private void SetVisibility(bool newState)
    {
        if (m_Visible == newState || m_Visibility != null)
            return;

        m_Visibility = StartCoroutine(PerformVisibility(newState));
    }

    private IEnumerator PerformVisibility(bool newState)
    {
        var from = newState ? 0f : m_ContentHeight;
        var to = newState ? m_ContentHeight : 0f;
        float time = 0f;
        while (time < m_VisibilityTime)
        {
            var value = Mathf.Lerp(from, to, time / m_VisibilityTime);
            m_ContentRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, value);
            time += Time.deltaTime;
            yield return null;
        }
        m_ContentRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, to);

        m_Visible = newState;
        m_Visibility = null;
    }

    public void ToggleVisibility()
        => SetVisibility(!m_Visible);
}
