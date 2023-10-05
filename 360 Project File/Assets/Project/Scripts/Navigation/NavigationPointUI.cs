using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class NavigationPointUI : Singleton<NavigationPointUI>
{
    [SerializeField]
    private NavigationButton m_ButtonPrefab;

    [SerializeField]
    private TMP_Text m_LocationDisplay;
    public string LocationDisplay
    {
        get => m_LocationDisplay.text;
        set => m_LocationDisplay.text = value;
    }

    [SerializeField]
    private float m_FadeDuration;

    private RectTransform m_CanvasRect;
    private CanvasGroup m_CanvasGroup;
    private Coroutine m_CanvasFade;
    private SphericalHelper m_Current;
    private NavigationButton[] m_Buttons;

    protected override void Awake()
    {
        base.Awake();
        m_CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_Buttons = new NavigationButton[10];
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            var button = Instantiate(m_ButtonPrefab, transform);
            button.gameObject.SetActive(false);
            m_Buttons[i] = button;
        }
    }

    private void Start()
    {
        LocationDisplay = string.Empty;
    }

    private void OnEnable()
    {
        NavigationManager.navigationStarted += OnNavigationStarted;
        NavigationManager.navigationFinished += OnNavigationFinished;
        NavigationManager.sphericalChanged += OnSphericalChanged;
    }

    private void OnDisable()
    {
        NavigationManager.navigationStarted -= OnNavigationStarted;
        NavigationManager.navigationFinished -= OnNavigationFinished;
        NavigationManager.sphericalChanged -= OnSphericalChanged;
    }

    private void Update()
    {
        if (m_Current == null)
            return;

        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        var points = m_Current.NavPoints;
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            var button = m_Buttons[i];

            bool shouldActive = true;
            if (i >= points.Length || !GeometryUtility.TestPlanesAABB(planes, points[i].Bounds))
                shouldActive = false;

            button.gameObject.SetActive(shouldActive);
            if (!button.gameObject.activeSelf)
                continue;

            // Worldspace to UI position
            var viewportPoint = Camera.main.WorldToViewportPoint(points[i].transform.position);
            button.GetComponent<RectTransform>().anchoredPosition = new(
                (viewportPoint.x * m_CanvasRect.sizeDelta.x) - (m_CanvasRect.sizeDelta.x * 0.5f),
                (viewportPoint.y * m_CanvasRect.sizeDelta.y) - (m_CanvasRect.sizeDelta.y * 0.5f));

            // Update data
            button.NavPoint = points[i];
        }
    }

    private void OnNavigationStarted()
        => Fade();

    private void OnNavigationFinished()
        => Fade(true);

    private void OnSphericalChanged(SphericalHelper _, SphericalHelper newSpherical)
        => m_Current = newSpherical;

    private void Fade(bool fadeIn = false)
    {
        if (m_CanvasFade != null)
            StopCoroutine(m_CanvasFade);

        m_CanvasFade = StartCoroutine(PerformFade(fadeIn));
    }

    private IEnumerator PerformFade(bool fadeIn)
    {
        float from = fadeIn ? 0f : 1f;
        float to = fadeIn ? 1f : 0f;

        if (fadeIn)
            m_CanvasGroup.blocksRaycasts = true;

        float time = 0f;
        while (time < m_FadeDuration)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(from, to, time / m_FadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        m_CanvasGroup.alpha = to;

        if (!fadeIn)
            m_CanvasGroup.blocksRaycasts = false;
    }
}
