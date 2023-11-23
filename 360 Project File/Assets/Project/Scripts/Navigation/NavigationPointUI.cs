using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class NavigationPointUI : MonoBehaviour
{
    private static readonly Color k_BlockerActiveColor = Color.black;
    private static readonly Color k_BlockerInactiveColor = new(0f, 0f, 0f, 0f);

    [Header("UI")]
    [SerializeField]
    private Image m_Blocker;
    [SerializeField]
    private NavigationButton m_ButtonPrefab;

    [SerializeField]
    private TMP_Text m_LocationDisplay;
    public string LocationDisplay
    {
        get => m_LocationDisplay.text;
        set => m_LocationDisplay.text = value;
    }

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_FadeAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);

    [Header("Event Channels")]
    [SerializeField]
    private NavigationPointUIEventChannelSO m_NavigationPointUIEventChannel;
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;

    private RectTransform m_CanvasRect;
    private CanvasGroup m_CanvasGroup;
    private Coroutine m_CanvasFade;

    private SphericalHelper m_Current;
    private NavigationButton[] m_Buttons;

    private void Awake()
    {
        m_CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_Buttons = new NavigationButton[10];
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            var button = Instantiate(m_ButtonPrefab, transform);
            button.gameObject.SetActive(false);
            m_Buttons[i] = button;
        }
        m_Blocker.transform.SetAsLastSibling();
        m_Blocker.raycastTarget = false;
    }

    private void Start()
    {
        LocationDisplay = string.Empty;
    }

    private void OnEnable()
    {
        m_NavigationPointUIEventChannel.OnDisplayUnlockJourneyEntry += OnDisplayUnlockJourneyEntry;
        m_NavigationPointUIEventChannel.OnFadeUI += OnFadeUI;
        m_NavigationPointUIEventChannel.OnFadeOverlay += OnFadeOverlay;
        m_NavigationEventChannel.OnSphericalChanged += OnSphericalChanged;
    }

    private void OnDisable()
    {
        m_NavigationPointUIEventChannel.OnFadeUI -= OnFadeUI;
        m_NavigationPointUIEventChannel.OnFadeOverlay -= OnFadeOverlay;
        m_NavigationEventChannel.OnSphericalChanged -= OnSphericalChanged;
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
            button.Locked = points[i].UnlockingJourneyEntry != null;
        }
    }

    private void OnDisplayUnlockJourneyEntry(JourneyEntrySO entry)
    {
        Debug.Log($"Completed journey {entry.name}");
    }

    private float OnFadeUI(bool fadeIn)
    {
        if (m_CanvasFade != null)
            StopCoroutine(m_CanvasFade);

        m_CanvasFade = StartCoroutine(PerformUIFade(fadeIn));
        return m_FadeAnimation.GetLastKeyTime();
    }

    private float OnFadeOverlay(bool fadeIn)
    {
        if (m_CanvasFade != null)
            StopCoroutine(m_CanvasFade);

        m_CanvasFade = StartCoroutine(PerformOverlayFade(fadeIn));
        return m_FadeAnimation.GetLastKeyTime();
    }

    private void OnSphericalChanged(SphericalHelper from, SphericalHelper to, NavigationManager.NavigationMode mode)
        => m_Current = to;

    private IEnumerator PerformUIFade(bool fadeIn)
    {
        var alphaFrom = fadeIn ? 0f : 1f;
        var alphaTo = fadeIn ? 1f : 0f;

        float time = 0f;
        while (time < m_FadeAnimation.GetLastKeyTime())
        {
            m_CanvasGroup.alpha = Mathf.Lerp(alphaFrom, alphaTo, m_FadeAnimation.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        m_CanvasGroup.alpha = alphaTo;
    }

    private IEnumerator PerformOverlayFade(bool fadeIn)
    {
        var colorFrom = fadeIn ? k_BlockerActiveColor : k_BlockerInactiveColor;
        var colorTo = fadeIn ? k_BlockerInactiveColor : k_BlockerActiveColor;

        m_Blocker.raycastTarget = true;

        float time = 0f;
        while (time < m_FadeAnimation.GetLastKeyTime())
        {
            m_Blocker.color = Color.Lerp(colorFrom, colorTo, m_FadeAnimation.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        m_Blocker.color = colorTo;
        m_Blocker.raycastTarget = !fadeIn;
    }
}
