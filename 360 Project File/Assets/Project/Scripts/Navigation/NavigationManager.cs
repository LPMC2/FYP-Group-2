using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NavigationManager : Singleton<NavigationManager>
{
    public static event UnityAction navigationStarted;
    public static event UnityAction navigationFinished;
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

    [Header("Navigation")]
    [SerializeField]
    private NavigationPoint m_NavigationPointPrefab;

    private SphericalHelper m_Current;
    private Coroutine m_Animation;

    public void LoadMap(Map map)
    {
        var mapManager = MapManager.Instance;
        if (mapManager == null)
        {
            Debug.LogWarning("[NavManager] MapManager is required for this function to work.");
            return;
        }

        for (int i = 0; i < map.Connections.Length; i++)
        {
            var connection = map.Connections[i];
            var from = mapManager[connection.from];
            var to = mapManager[connection.to];
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
        sphericalChanged?.Invoke(m_Current, target);
        m_Current = target;
    }

    public void NavigateTowards(SphericalHelper target)
    {
        if (m_Animation != null)
            StopCoroutine(m_Animation);

        m_Animation = StartCoroutine(PerformCameraMove(target));
    }

    private IEnumerator PerformCameraMove(SphericalHelper target)
    {
        navigationStarted?.Invoke();
        var cameraTransform = m_Camera.transform;

        // Camera rotation
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

        // Movement
        Vector3 fromPosition = m_CameraRig.position;
        Vector3 toPosition = target.transform.position;
        time = 0f;
        while (time < m_PositionAnim.GetLastKeyTime())
        {
            float progress = m_PositionAnim.Evaluate(time);
            m_CameraRig.position = Vector3.Lerp(fromPosition, toPosition, progress);
            m_Current.Alpha = Mathf.Lerp(1f, 0f, progress);
            target.Alpha = Mathf.Lerp(0f, 1f, progress);
            time += Time.deltaTime;
            yield return null;
        }
        m_CameraRig.position = toPosition;
        // Set previous spherical back to fully visible for scene view debugging
        m_Current.Alpha = target.Alpha = 1f;

        sphericalChanged?.Invoke(m_Current, target);
        navigationFinished?.Invoke();

        m_Animation = null;
        m_Current = target;
    }
}
