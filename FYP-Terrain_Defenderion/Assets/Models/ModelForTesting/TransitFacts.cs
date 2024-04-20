using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitFacts : MonoBehaviour
{
    [SerializeField]
    private PageContent[] m_DataSource;

    [Header("Pages")]
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
    private Button m_PrevButton;
    [SerializeField]
    private Button m_NextButton;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_NavigateAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.35f, 1f);

    private List<(CarouselPage, CarouselIndicator)> m_ManagedPages;
    private int m_CurrentPage;

    private Coroutine m_Navigate;

    private void Awake()
    {
        m_ManagedPages = new();
        for (int i = 0; i < m_DataSource.Length; i++)
        {
            var page = Instantiate(m_PagePrefab, m_PageRoot);      
            page.Alpha = 0f;
            page.BlockRaycasts = false;
            page.Image = m_DataSource[i].Image;
            page.Caption = m_DataSource[i].Caption;
            var indicator = Instantiate(m_IndicatorPrefab, m_IndicatorRoot);
            indicator.Active = false;
            m_ManagedPages.Add((page, indicator));
        }
    }

    private void Start()
    {
        var (page, indicator) = m_ManagedPages[m_CurrentPage];
        page.Alpha = 1f;
        page.BlockRaycasts = true;
        indicator.Active = true;
        m_PrevButton.interactable = false;
    }

    private void OnEnable()
    {
        m_PrevButton.onClick.AddListener(PrevItem);
        m_NextButton.onClick.AddListener(NextItem);
    }

    private void OnDisable()
    {
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
        var (oldPage, oldIndicator) = m_ManagedPages[m_CurrentPage];
        var (newPage, newIndicator) = m_ManagedPages[index];

        float time = 0f;
        while (time < m_NavigateAnimation.GetLastKeyTime())
        {
            oldPage.Alpha = Mathf.Lerp(1f, 0f, m_NavigateAnimation.Evaluate(time));
            newPage.Alpha = Mathf.Lerp(0f, 1f, m_NavigateAnimation.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        oldPage.Alpha = 0f;
        newPage.Alpha = 1f;

        oldPage.BlockRaycasts = oldIndicator.Active = false;
        newPage.BlockRaycasts = newIndicator.Active = true;

        m_PrevButton.interactable = index != 0;
        m_NextButton.interactable = index != m_ManagedPages.Count - 1;

        m_CurrentPage = index;
        m_Navigate = null;
    }

    public void PrevItem()
        => NavigateTo(m_CurrentPage - 1);

    public void NextItem()
        => NavigateTo(m_CurrentPage + 1);
    
    [System.Serializable]
    public struct PageContent
    {
        public Sprite Image;
        public string Caption;
    }
}
