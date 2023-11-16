using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorPlan : CollapsiblePanel
{
    protected override float ContentHeight => 384f;

    [Header("UI")]
    [SerializeField]
    private RectTransform m_Dot;

    private Transform m_CameraTransform;
    private RectTransform m_CurrentMapFloor;
    private Dictionary<string, RectTransform> m_ImageDict;

    private void Awake()
    {
        m_ImageDict = new();
    }

    private void Start()
    {
        m_CameraTransform = Camera.main.transform;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        NavigationManager.mapFloorAdded += OnMapFloorAdded;
        NavigationManager.mapFloorChanged += OnMapFloorChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        NavigationManager.mapFloorAdded -= OnMapFloorAdded;
        NavigationManager.mapFloorChanged -= OnMapFloorChanged;
    }

    private void Update()
    {
        if (m_CurrentMapFloor == null)
            return;

        var contentRect = m_ContentRectTransform.rect;
        var x = -(m_CameraTransform.position.x * 10f) + (contentRect.width / 2);
        var y = Mathf.Abs((m_CameraTransform.position.z * 10f) - m_CurrentMapFloor.rect.height + (contentRect.height / 2));
        m_CurrentMapFloor.anchoredPosition = new Vector2(x, y);
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
        mapObject.transform.SetParent(m_ContentRectTransform, false);
        mapObject.SetActive(false);

        m_ImageDict.Add(mapFloor.name, rectTransform);
        m_Dot.SetAsLastSibling();
    }

    private void OnMapFloorChanged(MapFloor from, MapFloor to)
    {
        foreach (var entry in m_ImageDict)
            entry.Value.gameObject.SetActive(entry.Key == to.name);

        m_CurrentMapFloor = m_ImageDict[to.name];
    }
}
