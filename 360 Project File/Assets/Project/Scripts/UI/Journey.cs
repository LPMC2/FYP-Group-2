using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Journey : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private JourneyEntries m_DataSource;

    [Header("Toggle")]
    [SerializeField]
    private Button m_ToggleButton;
    [SerializeField]
    private RectTransform m_ContentRectTransform;

    [Header("Entry")]
    [SerializeField]
    private float m_EntryHeight;
    [SerializeField]
    private JourneyEntry m_EntryPrefab;
    [SerializeField]
    private Transform m_EntryRoot;

    [Header("Controls")]
    [SerializeField]
    private RectTransform m_ViewportRectTransform;
    [SerializeField]
    private Button m_PrevButton;
    [SerializeField]
    private Button m_NextButton;

    [Header("Animation")]
    [SerializeField]
    private float m_VisibilityTime;
    [SerializeField]
    private float m_NavigateTime;

    private bool m_Visible;

    private List<JourneyEntry> m_ManagedEntries;
    private int m_CurrentEntry;

    private Coroutine m_Visibility, m_Navigate;

    private void Awake()
    {
        m_ManagedEntries = new();
        for (int i = 0; i < m_DataSource.Entries.Length; i++)
        {
            var instance = Instantiate(m_EntryPrefab, m_EntryRoot);
            instance.Index = i + 1;
            instance.StepKey = m_DataSource.Entries[i];
            m_ManagedEntries.Add(instance);
        }
    }

    private void Start()
    {
        RefeshPosition();
        m_PrevButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_ToggleButton.onClick.AddListener(ToggleVisibility);
        m_PrevButton.onClick.AddListener(PrevItem);
        m_NextButton.onClick.AddListener(NextItem);
    }

    private void OnDisable()
    {
        m_ToggleButton.onClick.RemoveListener(ToggleVisibility);
        m_PrevButton.onClick.RemoveListener(PrevItem);
        m_NextButton.onClick.RemoveListener(NextItem);
    }

    private void SetVisibility(bool newState)
    {
        if (m_Visible == newState || m_Visibility != null)
            return;

        m_Visibility = StartCoroutine(PerformVisibility(newState));
    }

    private IEnumerator PerformVisibility(bool newState)
    {
        var from = newState ? 0f : m_EntryHeight * 3f;
        var to = newState ? m_EntryHeight * 3f : 0f;
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

    private void NavigateTo(int index)
    {
        if (0 > index || m_ManagedEntries.Count <= index)
        {
            Debug.LogWarning($"[Carousel] Invalid page index: {index}");
            return;
        }

        if (m_CurrentEntry == index || m_Navigate != null)
            return;

        m_Navigate = StartCoroutine(PerformNavigate(index));
    }

    private IEnumerator PerformNavigate(int index)
    {
        var fromPos = m_ViewportRectTransform.anchoredPosition;
        var toPos = new Vector2(m_ViewportRectTransform.anchoredPosition.x, CalculateOffset(index));
        float time = 0f;
        while (time < m_NavigateTime)
        {
            m_ViewportRectTransform.anchoredPosition = Vector2.Lerp(fromPos, toPos, time / m_NavigateTime);
            time += Time.deltaTime;
            yield return null;
        }
        m_ViewportRectTransform.anchoredPosition = toPos;

        m_PrevButton.gameObject.SetActive(index != 0);
        m_NextButton.gameObject.SetActive(index != m_ManagedEntries.Count - 1);

        m_CurrentEntry = index;
        m_Navigate = null;
    }

    private void RefeshPosition()
        => m_ViewportRectTransform.anchoredPosition = new(m_ViewportRectTransform.anchoredPosition.x, CalculateOffset(m_CurrentEntry));

    private float CalculateOffset(int index)
    {
        var result = -m_EntryHeight;
        if (index > 0)
            result = m_EntryHeight * (index - 1);
        return result;
    }

    public void ToggleVisibility()
        => SetVisibility(!m_Visible);

    public void PrevItem()
        => NavigateTo(m_CurrentEntry - 1);

    public void NextItem()
        => NavigateTo(m_CurrentEntry + 1);
}
