using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FloorPlan : CollapsiblePanel
{
    private static readonly Color k_InactiveMapColor = new(1f, 1f, 1f, 0f);
    private static readonly Color k_ActiveMapColor = Color.white;

    protected override float ContentHeight => 384f;

    [Header("UI")]
    [SerializeField]
    private RectTransform m_Dot;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_FadeAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.35f, 1f);

    [Header("Event Channels")]
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;

    private Transform m_CameraTransform;
    private List<RectTransform> m_CurrentRectTransforms;
    private Dictionary<string, (RectTransform, RawImage)> m_ImageDict;

    private void Awake()
    {
        m_CurrentRectTransforms = new();
        m_ImageDict = new();
    }

    private void Start()
    {
        m_CameraTransform = Camera.main.transform;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_NavigationEventChannel.OnMapFloorAdded += OnMapFloorAdded;
        m_NavigationEventChannel.OnMapFloorChanged += OnMapFloorChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_NavigationEventChannel.OnMapFloorAdded -= OnMapFloorAdded;
        m_NavigationEventChannel.OnMapFloorChanged -= OnMapFloorChanged;
    }

    private void Update()
    {
        if (m_CurrentRectTransforms.Count <= 0)
            return;

        foreach (var rectTransform in m_CurrentRectTransforms)
        {
            var contentRect = m_ContentRectTransform.rect;
            var x = -(m_CameraTransform.position.x * 10f) + (contentRect.width / 2);
            var y = Mathf.Abs((m_CameraTransform.position.z * 10f) - rectTransform.rect.height + (contentRect.height / 2));
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    private void OnMapFloorAdded(MapFloor mapFloor)
    {
        if (m_ImageDict.ContainsKey(mapFloor.name))
            return;

        var mapObject = new GameObject(mapFloor.name, typeof(RectTransform));
        var rectTransform = mapObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.sizeDelta = new Vector2(mapFloor.MapTexture.width, mapFloor.MapTexture.height);

        var image = mapObject.AddComponent<RawImage>();
        image.texture = mapFloor.MapTexture;
        image.color = k_InactiveMapColor;
        mapObject.transform.SetParent(m_ContentRectTransform, false);

        m_ImageDict.Add(mapFloor.name, (rectTransform, image));
        m_Dot.SetAsLastSibling();
    }

    private void OnMapFloorChanged(MapFloor from, MapFloor to, NavigationManager.NavigationMode navigationMode)
    {
        if (navigationMode == NavigationManager.NavigationMode.Teleport)
        {
            if (from != null)
            {
                var (fromRect, fromImage) = GetMapFloor(from);
                fromImage.color = k_InactiveMapColor;
                m_CurrentRectTransforms.Remove(fromRect);
            }

            var (toRect, toImage) = GetMapFloor(to);
            toImage.color = k_ActiveMapColor;
            m_CurrentRectTransforms.Add(toRect);
        }
        else
            StartCoroutine(PerformFloorFade(from, to));
    }

    private IEnumerator PerformFloorFade(MapFloor from, MapFloor to)
    {
        var (fromRect, fromImage) = GetMapFloor(from);
        var (toRect, toImage) = GetMapFloor(to);

        m_CurrentRectTransforms.Add(toRect);

        var time = 0f;
        while (time < m_FadeAnimation.GetLastKeyTime())
        {
            fromImage.color = Color.Lerp(k_ActiveMapColor, k_InactiveMapColor, m_FadeAnimation.Evaluate(time));
            toImage.color = Color.Lerp(k_InactiveMapColor, k_ActiveMapColor, m_FadeAnimation.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        fromImage.color = k_InactiveMapColor;
        toImage.color = k_ActiveMapColor;

        m_CurrentRectTransforms.Remove(fromRect);
    }

    private (RectTransform, RawImage) GetMapFloor(MapFloor floor)
        => m_ImageDict.First(x => x.Key == floor.name).Value;
}
