using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Journey : CollapsiblePanel
{
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
    private AnimationCurve m_NavigateAnim = AnimationCurve.EaseInOut(0f, 0f, 0.35f, 1f);

    [Header("Event Channels")]
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannel;

    protected override RectTransform.Edge ExpandEdge => RectTransform.Edge.Top;
    protected override float ContentHeight => m_EntryHeight * 3f;

    private Dictionary<JourneyEntrySO, JourneyEntry> m_ManagedEntries;
    private int m_CurrentEntry;

    private Coroutine m_Navigate;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_JourneyEventChannel.OnEntryCompleted += OnEntryCompleted;
        m_JourneyEventChannel.OnLoadEntries += OnLoadEntries;
        m_PrevButton.onClick.AddListener(PrevItem);
        m_NextButton.onClick.AddListener(NextItem);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_JourneyEventChannel.OnEntryCompleted -= OnEntryCompleted;
        m_JourneyEventChannel.OnLoadEntries -= OnLoadEntries;
        m_PrevButton.onClick.RemoveListener(PrevItem);
        m_NextButton.onClick.RemoveListener(NextItem);
    }

    private void OnEntryCompleted(JourneyEntrySO entry)
    {
        Debug.Log($"[Journey] '{entry.name}' completed");
    }

    private void OnLoadEntries(JourneyEntrySO[] entries)
    {
        m_ManagedEntries = new();
        for (int i = 0; i < entries.Length; i++)
        {
            var instance = Instantiate(m_EntryPrefab, m_EntryRoot);
            instance.Index = i + 1;
            instance.StepKey = entries[i].StepKey;
            m_ManagedEntries.Add(entries[i], instance);
        }
        RefeshPosition();
        m_PrevButton.gameObject.SetActive(false);
    }

    public void PrevItem()
        => NavigateTo(m_CurrentEntry - 1);

    public void NextItem()
        => NavigateTo(m_CurrentEntry + 1);

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
        while (time < m_NavigateAnim.GetLastKeyTime())
        {
            m_ViewportRectTransform.anchoredPosition = Vector2.Lerp(fromPos, toPos, m_NavigateAnim.Evaluate(time));
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
}
