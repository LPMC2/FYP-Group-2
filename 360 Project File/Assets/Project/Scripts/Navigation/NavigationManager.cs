using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Transform m_CameraRig;
    [SerializeField]
    private Transform m_Camera;
    [SerializeField]
    private Transform m_Floor;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_RotationAnim;
    [SerializeField]
    private AnimationCurve m_PositionAnim;

    [Header("Map Floor")]
    [SerializeField]
    private Material m_PlaneMaterial;

    [Header("Navigation")]
    [SerializeField]
    private SphericalHelper m_SphericalPrefab;
    [SerializeField]
    private NavigationPoint m_NavigationPointPrefab;

    [Header("Event Channels")]
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;
    [SerializeField]
    private NavigationPointUIEventChannelSO m_NavigationPointUIEventChannelSO;
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannel;
    [SerializeField]
    private CameraEventChannelSO m_CameraEventChannel;

    private static int s_MapFloorLayer;
    private Dictionary<MapFloor, Dictionary<MapLandmark, SphericalHelper>> m_MapDict;
    private MapFloor m_CurrentMapFloor;
    private SphericalHelper m_CurrentSpherical;

    private Coroutine m_Navigation;

    private void Awake()
    {
        if (s_MapFloorLayer == 0)
            s_MapFloorLayer = LayerMask.NameToLayer("MapFloor");
    }

    private void OnEnable()
    {
        m_NavigationEventChannel.OnLoadMap += OnLoadMap;
        m_NavigationEventChannel.OnNavigate += OnNavigate;
    }

    private void OnDisable()
    {
        m_NavigationEventChannel.OnLoadMap -= OnLoadMap;
        m_NavigationEventChannel.OnNavigate -= OnNavigate;
    }

    private void Update()
    {
        if (m_Floor != null)
            m_Floor.localEulerAngles = new Vector3(0f, m_Camera.localEulerAngles.y, 0f);
    }

    private void OnLoadMap(Map map, MapLandmark startPoint, Vector3 startRotation)
    {
        m_MapDict = new();
        for (int i = 0; i < map.Floors.Length; i++)
        {
            var floor = map.Floors[i];
            if (!m_MapDict.ContainsKey(floor))
            {
                m_NavigationEventChannel.OnMapFloorAdded?.Invoke(floor);
                m_MapDict[floor] = new();
            }

            var width = floor.MapTexture.width / 10f;
            var height = floor.MapTexture.height / 10f;

            var plane = new GameObject(floor.name);
            plane.transform.SetParent(gameObject.transform, false);
            plane.transform.localPosition = new Vector3(0f, i * 2.5f, 0f);
            plane.layer = s_MapFloorLayer;

            var meshFilter = plane.AddComponent<MeshFilter>();
            meshFilter.mesh = CreatePlane(width, height);

            var material = Instantiate(m_PlaneMaterial);
            material.mainTexture = floor.MapTexture;

            var renderer = plane.AddComponent<MeshRenderer>();
            renderer.material = material;

            var root = new GameObject("Sphericals");
            root.transform.SetParent(plane.transform, false);
            root.transform.localPosition = new Vector3(0, 0, height);
            foreach (var landmark in floor.Landmarks)
            {
                var spherical = Instantiate(m_SphericalPrefab, root.transform);
                spherical.gameObject.name = landmark.name;
                spherical.Texture = landmark.Texture;
                spherical.DisplayNameKey = landmark.DisplayNameKey;
                spherical.Actions = landmark.Actions;
                spherical.transform.SetLocalPositionAndRotation(new Vector3(landmark.Position.x / 10f, 1f, -(landmark.Position.y / 10f)), Quaternion.Euler(landmark.Rotation));
#if !UNITY_EDITOR
                spherical.Alpha = 0f;
#endif
                m_MapDict[floor].Add(landmark, spherical);
            }
        }

        for (int i = 0; i < map.Connections.Length; i++)
        {
            var connection = map.Connections[i];
            if (connection.from == connection.to)
            {
                Debug.LogWarning($"[NavManager] Self connection at #{i} ignored!");
                continue;
            }

            var from = GetSphericalHelper(connection.from);
            var to = GetSphericalHelper(connection.to);
            if (from == null || to == null)
            {
                Debug.LogWarning($"[NavManager] Connection {connection.from.name} to {connection.to.name} is missing spherical reference(s)");
                continue;
            }

            var fromPoint = Instantiate(m_NavigationPointPrefab);
            fromPoint.gameObject.name = $"NavPoint (#{i}: {connection.to.name})";
            fromPoint.Destination = to;
            fromPoint.Flags = connection.flags;
            fromPoint.UnlockingJourneyEntry = connection.unlockingJourneyEntrySO;
            from.AddNavigationPoint(fromPoint);

            if (connection.flags.HasFlag(Map.MapConnectionFlags.OneWayOnly))
                continue;

            var toPoint = Instantiate(m_NavigationPointPrefab);
            toPoint.gameObject.name = $"NavPoint (#{i}: {connection.from.name})";
            toPoint.Destination = from;
            toPoint.Flags = connection.flags;
            to.AddNavigationPoint(toPoint);
        }

        var startSpherical = GetSphericalHelper(startPoint);
        startSpherical.Alpha = 1f;
        m_CurrentSpherical = startSpherical;
        m_NavigationEventChannel.OnSphericalChanged(null, startSpherical, NavigationMode.Teleport);

        var startFloor = GetMapFloor(startSpherical);
        m_CurrentMapFloor = startFloor;
        m_NavigationEventChannel.OnMapFloorChanged?.Invoke(null, startFloor, NavigationMode.Teleport);

        m_CameraRig.position = m_CurrentSpherical.transform.position;
        m_CameraEventChannel.OnRotationSnap?.Invoke(startRotation);
    }

    private void OnNavigate(SphericalHelper destination, NavigationMode mode)
    {
        if (m_Navigation != null)
            StopCoroutine(m_Navigation);

        switch (mode)
        {
            case NavigationMode.Move:
                m_Navigation = StartCoroutine(PerformMove(destination));
                break;
            case NavigationMode.Teleport:
                m_Navigation = StartCoroutine(PerformTeleport(destination));
                break;
        }
    }

    private MapFloor GetMapFloor(SphericalHelper sphericalHelper)
    {
        foreach (var (floor, landmarkDict) in m_MapDict)
        {
            if (!landmarkDict.ContainsValue(sphericalHelper))
                continue;

            return floor;
        }
        return null;
    }

    private MapLandmark GetMapLandmark(SphericalHelper sphericalHelper)
    {
        foreach (var (_, landmarkDict) in m_MapDict)
        {
            if (!landmarkDict.ContainsValue(sphericalHelper))
                continue;

            return landmarkDict.First(x => x.Value == sphericalHelper).Key;
        }
        return null;
    }

    private SphericalHelper GetSphericalHelper(MapLandmark mapLandmark)
    {
        foreach (var landmarkDict in m_MapDict.Values)
        {
            if (!landmarkDict.ContainsKey(mapLandmark))
                continue;

            return landmarkDict[mapLandmark];
        }
        return null;
    }

    private IEnumerator PerformMove(SphericalHelper target)
    {
        m_NavigationEventChannel.OnNavigationStarted?.Invoke(NavigationMode.Move);
        m_NavigationPointUIEventChannelSO.OnFadeUI?.Invoke(false);

        // Camera rotation
        var cameraTransform = m_Camera.transform;
        Vector3 direction = target.transform.position - cameraTransform.position;
        Quaternion fromRotation = cameraTransform.rotation;
        var angles = Quaternion.LookRotation(direction, cameraTransform.up).eulerAngles;
        Quaternion toRotation = Quaternion.Euler(angles.x, angles.y, 0f);

        float time = 0f;
        while (time < m_RotationAnim.GetLastKeyTime())
        {
            cameraTransform.rotation = Quaternion.Lerp(fromRotation, toRotation, m_RotationAnim.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        cameraTransform.rotation = toRotation;

        // Floor
        var targetFloor = GetMapFloor(target);
        if (m_CurrentMapFloor != targetFloor)
        {
            m_NavigationEventChannel.OnMapFloorChanged?.Invoke(m_CurrentMapFloor, targetFloor, NavigationMode.Move);
            m_CurrentMapFloor = targetFloor;
        }

        // Movement + Scaling
        var dist = Vector3.Distance(m_CurrentSpherical.transform.position, target.transform.position) + 1f;
        Vector3 fromScale = new Vector3(dist, dist, dist);
        Vector3 toScale = Vector3.one;
        Vector3 fromPosition = m_CameraRig.position;
        Vector3 toPosition = target.transform.position;
        time = 0f;
        while (time < m_PositionAnim.GetLastKeyTime())
        {
            float progress = m_PositionAnim.Evaluate(time);
            m_CameraRig.position = Vector3.Lerp(fromPosition, toPosition, progress);
            m_CurrentSpherical.Alpha = Mathf.Lerp(1f, 0f, progress);
            m_CurrentSpherical.transform.localScale = Vector3.Lerp(toScale, fromScale, time / m_PositionAnim.GetLastKeyTime());
            target.Alpha = Mathf.Lerp(0f, 1f, progress);
            target.transform.localScale = Vector3.Lerp(fromScale, toScale, time / m_PositionAnim.GetLastKeyTime());
            time += Time.deltaTime;
            yield return null;
        }
        m_CameraRig.position = toPosition;
#if UNITY_EDITOR
        m_CurrentSpherical.Alpha = 1f;
#else
        m_CurrentSpherical.Alpha = 0f;
#endif
        target.Alpha = 1f;
        m_CurrentSpherical.transform.localScale = target.transform.localScale = Vector3.one;

        m_NavigationEventChannel.OnSphericalChanged?.Invoke(m_CurrentSpherical, target, NavigationMode.Move);
        m_NavigationEventChannel.OnNavigationFinished?.Invoke(NavigationMode.Move);
        m_NavigationPointUIEventChannelSO.OnFadeUI?.Invoke(true);
        m_JourneyEventChannel.OnDestinationReached?.Invoke(GetMapLandmark(target));

        m_Navigation = null;
        m_CurrentSpherical = target;
    }

    private IEnumerator PerformTeleport(SphericalHelper target)
    {
        m_NavigationEventChannel.OnNavigationStarted?.Invoke(NavigationMode.Teleport);

        var time = m_NavigationPointUIEventChannelSO.OnFadeOverlay?.Invoke(false);
        if (time.HasValue)
            yield return new WaitForSeconds(time.Value);

#if UNITY_EDITOR
        m_CurrentSpherical.Alpha = 1f;
#else
        m_CurrentSpherical.Alpha = 0f;
#endif
        target.Alpha = 1f;
        m_CameraRig.position = target.transform.position;

        var targetFloor = GetMapFloor(target);
        if (m_CurrentMapFloor != targetFloor)
        {
            m_NavigationEventChannel.OnMapFloorChanged?.Invoke(m_CurrentMapFloor, targetFloor, NavigationMode.Teleport);
            m_CurrentMapFloor = targetFloor;
        }

        m_NavigationEventChannel.OnSphericalChanged?.Invoke(m_CurrentSpherical, target, NavigationMode.Teleport);
        m_NavigationEventChannel.OnNavigationFinished?.Invoke(NavigationMode.Teleport);
        m_NavigationPointUIEventChannelSO.OnFadeOverlay?.Invoke(true);
        m_JourneyEventChannel.OnDestinationReached?.Invoke(GetMapLandmark(target));

        m_Navigation = null;
        m_CurrentSpherical = target;
    }

    private static Mesh CreatePlane(float width, float height)
    {
        return new Mesh()
        {
            vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, 0, height),
                new Vector3(width, 0, height)
            },
            triangles = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            },
            normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            },
            uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            }
        };
    }

    public enum NavigationMode
    {
        Move,
        Teleport
    }
}
