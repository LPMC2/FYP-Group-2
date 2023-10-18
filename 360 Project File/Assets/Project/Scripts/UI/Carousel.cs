using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private TransitFacts m_DataSource;

    [Header("Pages")]
    [SerializeField]
    private float m_PageAspectRatio;
    [SerializeField]
    private CarouselPage m_PagePrefab;
    [SerializeField]
    private Transform m_PageRoot;

    [Header("Indicator")]
    [SerializeField]
    private CarouselIndicator m_IndicatorPrefab;
    [SerializeField]
    private Transform m_IndicatorRoot;

    [Header("Controls")]
    [SerializeField]
    private RectTransform m_Viewport;
    [SerializeField]
    private GridLayoutGroup m_ContentLayoutGroup;
    [SerializeField]
    private Button m_PrevButton;
    [SerializeField]
    private Button m_NextButton;
    [SerializeField]
    private GameObject m_LastPageObject;

    [Header("Animation")]
    [SerializeField]
    private float m_NavigateTime;

    private RectTransform m_ContentRectTransform;
    
    private List<(CarouselPage, CarouselIndicator)> m_ManagedPages;
    private int m_CurrentPage;

    private Vector2 m_PageSize;
    private float m_LeftPadding, m_PageSpacing;

    private Coroutine m_Navigate;
    
    private void Awake()
    {
        m_ContentRectTransform = m_ContentLayoutGroup.GetComponent<RectTransform>();
        m_ManagedPages = new();
        for (int i = 0; i < m_DataSource.Entries.Length; i++)
        {
            var entry = m_DataSource.Entries[i];
            var page = Instantiate(m_PagePrefab, m_PageRoot);
            page.TitleKey = entry.titleKey;
            page.ContentKey = entry.bodyKey;
            var indicator = Instantiate(m_IndicatorPrefab, m_IndicatorRoot);
            indicator.Active = false;
            m_ManagedPages.Add((page, indicator));
        }
    }

    private void Start()
    {
        RefreshLayout();
        RefeshPosition();
        var (_, indicator) = m_ManagedPages[m_CurrentPage];
        indicator.Active = true;
        m_PrevButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResolutionHelper.resolutionChanged += OnResolutionChanged;
        m_PrevButton.onClick.AddListener(PrevItem);
        m_NextButton.onClick.AddListener(NextItem);
    }

    private void OnDisable()
    {
        ResolutionHelper.resolutionChanged -= OnResolutionChanged;
        m_PrevButton.onClick.RemoveListener(PrevItem);
        m_NextButton.onClick.RemoveListener(NextItem);
    }

    private void NavigateTo(int index)
    {
        if (0 > index || m_ManagedPages.Count <= index)
        {
            Debug.LogWarning($"[Carousel] Invalid page index: {index}");
            return;
        }

        if (m_CurrentPage == index || m_Navigate != null)
            return;

        m_Navigate = StartCoroutine(PerformNavigate(index));
    }

    private IEnumerator PerformNavigate(int index)
    {
        var fromPos = m_ContentRectTransform.anchoredPosition;
        var toPos = new Vector2(CalculateOffset(index), 0f);
        float time = 0f;
        while (time < m_NavigateTime)
        {
            m_ContentRectTransform.anchoredPosition = Vector2.Lerp(fromPos, toPos, time / m_NavigateTime);
            time += Time.deltaTime;
            yield return null;
        }

        m_ManagedPages[m_CurrentPage].Item2.Active = false;
        m_ManagedPages[index].Item2.Active = true;

        m_PrevButton.gameObject.SetActive(index != 0);
        m_NextButton.gameObject.SetActive(index != m_ManagedPages.Count - 1);
        m_LastPageObject.gameObject.SetActive(index == m_ManagedPages.Count - 1);

        m_CurrentPage = index;
        m_Navigate = null;
    }

    private void OnResolutionChanged()
    {
        StartCoroutine(DelayedRefresh());
    }

    private IEnumerator DelayedRefresh()
    {
        yield return new WaitForEndOfFrame();
        RefreshLayout();
        RefeshPosition();
    }

    private void RefreshLayout()
    {
        var paddedWidth = Mathf.FloorToInt(m_Viewport.rect.width - 286f);
        var paddedHeight = Mathf.FloorToInt(paddedWidth / m_PageAspectRatio);
        if (paddedHeight > m_Viewport.rect.height)
        {
            paddedHeight = Mathf.FloorToInt(m_Viewport.rect.height);
            paddedWidth = Mathf.FloorToInt(paddedHeight * m_PageAspectRatio);
        }

        m_LeftPadding = (m_Viewport.rect.width - paddedWidth) / 2;
        m_PageSize = new(paddedWidth, paddedHeight);
        m_ContentLayoutGroup.cellSize = m_PageSize;

        m_PageSpacing = Mathf.FloorToInt((m_Viewport.rect.width - paddedWidth - 77f) / 2);
        m_ContentLayoutGroup.spacing = new(m_PageSpacing, 0f);
    }

    private void RefeshPosition()
        => m_ContentRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, CalculateOffset(m_CurrentPage), 0);

    private float CalculateOffset(int index)
    {
        var result = m_LeftPadding;
        if (index > 0)
            result = -(m_PageSize.x * index) + 77f / 2f - (m_PageSpacing * (index - 1));
        return result;
    }
    
    public void PrevItem()
        => NavigateTo(m_CurrentPage - 1);

    public void NextItem()
        => NavigateTo(m_CurrentPage + 1);
}
