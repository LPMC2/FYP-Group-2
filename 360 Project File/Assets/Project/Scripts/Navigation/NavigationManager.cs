using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigationManager : Singleton<NavigationManager>
{
    public static event UnityAction navigationStarted;
    public static event UnityAction navigationFinished;
    public static event UnityAction<MapFloor> mapFloorAdded;
    public static event UnityAction<MapFloor, MapFloor> mapFloorChanged;
    public static event UnityAction<SphericalHelper, SphericalHelper> sphericalChanged;

    [Header("Camera")]
    [SerializeField]
    private Transform m_CameraRig;
    [SerializeField]
    private Transform m_Camera;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve m_RotationAnim;
    [SerializeField]
    private AnimationCurve m_PositionAnim;
    [SerializeField]
    private float m_RotationSpeed = 1f;
    [SerializeField]
    private float m_PositionSpeed = 1f;

    [Header("Map Floor")]
    [SerializeField]
    private Material m_PlaneMaterial;

    [Header("Navigation")]
    [SerializeField]
    private SphericalHelper m_SphericalPrefab;
    [SerializeField]
    private NavigationPoint m_NavigationPointPrefab;

    private static int s_MapFloorLayer;
    private Dictionary<MapFloor, Dictionary<MapLandmark, SphericalHelper>> m_MapDict;
    private MapFloor m_CurrentMapFloor;
    private SphericalHelper m_CurrentSpherical;
    private Coroutine m_Animation;

    protected override void Awake()
    {
        base.Awake();
        if (s_MapFloorLayer == 0)
            s_MapFloorLayer = LayerMask.NameToLayer("MapFloor");
        m_MapDict = new();
    }

    public void LoadMap(Map map)
    {
        for (int i = 0; i < map.Floors.Length; i++)
        {
            var floor = map.Floors[i];
            if (!m_MapDict.ContainsKey(floor))
            {
                mapFloorAdded?.Invoke(floor);
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
                spherical.transform.SetLocalPositionAndRotation(new Vector3(landmark.Position.x / 10f, 1f, -(landmark.Position.y / 10f)), Quaternion.Euler(landmark.Rotation));
                m_MapDict[floor].Add(landmark, spherical);
            }
        }

        for (int i = 0; i < map.Connections.Length; i++)
        {
            var connection = map.Connections[i];
            var from = GetSphericalHelper(connection.from);
            var to = GetSphericalHelper(connection.to);
            if (from == null || to == null)
            {
                Debug.LogWarning($"[NavManager] Connection {connection.from.name} to {connection.to.name} is missing spherical reference(s)");
                continue;
            }
            else if (from == to)
            {
                Debug.LogWarning($"[NavManager] Self connection at #{i} ignored!");
                continue;
            }

            var fromPoint = Instantiate(m_NavigationPointPrefab);
            fromPoint.gameObject.name = $"NavPoint (#{i}: {connection.to.name})";
            fromPoint.NavigationType = NavigationPoint.Type.Navigate;
            fromPoint.Flags = connection.flags;
            fromPoint.Destination = to;
            from.AddNavigationPoint(fromPoint);

            if (connection.flags.HasFlag(Map.MapConnectionFlags.OneWayOnly))
                continue;

            var toPoint = Instantiate(m_NavigationPointPrefab);
            toPoint.gameObject.name = $"NavPoint (#{i}: {connection.from.name})";
            toPoint.NavigationType = NavigationPoint.Type.Navigate;
            toPoint.Flags = connection.flags;
            toPoint.Destination = from;
            to.AddNavigationPoint(toPoint);
        }
    }

    public void TeleportTowards(SphericalHelper target)
    {
        if (m_Animation != null)
        {
            StopCoroutine(m_Animation);
            m_Animation = null;
        }

        m_CameraRig.position = target.transform.position;
        var targetFloor = GetMapFloor(target);
        mapFloorChanged?.Invoke(m_CurrentMapFloor, targetFloor);
        m_CurrentMapFloor = targetFloor;
        sphericalChanged?.Invoke(m_CurrentSpherical, target);
        m_CurrentSpherical = target;
    }

    public void NavigateTowards(SphericalHelper target)
    {
        if (m_Animation != null)
            StopCoroutine(m_Animation);

        m_Animation = StartCoroutine(PerformCameraMove(target));
    }

    public MapFloor GetMapFloor(SphericalHelper sphericalHelper)
    {
        foreach (var mapEntry in m_MapDict)
        {
            if (!mapEntry.Value.ContainsValue(sphericalHelper))
                continue;

            return mapEntry.Key;
        }
        return null;
    }

    public SphericalHelper GetSphericalHelper(MapLandmark mapLandmark)
    {
        foreach (var mapEntry in m_MapDict)
        {
            if (!mapEntry.Value.ContainsKey(mapLandmark))
                continue;

            return mapEntry.Value[mapLandmark];
        }
        return null;
    }

    private IEnumerator PerformCameraMove(SphericalHelper target)
    {
        navigationStarted?.Invoke();

        // Floor
        var targetFloor = GetMapFloor(target);
        mapFloorChanged?.Invoke(m_CurrentMapFloor, targetFloor);
        m_CurrentMapFloor = targetFloor;

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
            time += Time.deltaTime * m_RotationSpeed;
            yield return null;
        }
        cameraTransform.rotation = toRotation;

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
            time += Time.deltaTime * m_PositionSpeed;
            yield return null;
        }
        m_CameraRig.position = toPosition;
        // Set previous spherical back to fully visible for scene view debugging
        m_CurrentSpherical.Alpha = target.Alpha = 1f;
        m_CurrentSpherical.transform.localScale = target.transform.localScale = Vector3.one;

        sphericalChanged?.Invoke(m_CurrentSpherical, target);
        navigationFinished?.Invoke();

        m_Animation = null;
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
}
