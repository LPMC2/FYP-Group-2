using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavigationManager : Singleton<NavigationManager>
{
    public static event UnityAction navigationStarted;
    public static event UnityAction navigationFinished;
    public static event UnityAction<SphericalHelper, SphericalHelper> sphericalChanged;

    [Header("Stage")]
    [SerializeField]
    private SphericalHelper m_StartPoint;

    [Header("UI")]
    [SerializeField]
    private Image m_UIOverlay;

    [Header("Animation")]
    [SerializeField]
    private Transform m_CameraRig;
    [SerializeField]
    private Transform m_Camera;

    [SerializeField]
    private float m_InitialFadeDuration;

    [SerializeField]
    private float m_RotationDuration;

    [SerializeField]
    private float m_PositionDelay;

    [SerializeField]
    private float m_PositionDuration;

    private SphericalHelper m_Current;
    private Coroutine m_Animation;

    private void Start()
    {
        var sphericals = FindObjectsOfType<SphericalHelper>();
        foreach (var spherical in sphericals)
        {
            if (spherical == m_StartPoint)
                continue;

            spherical.Alpha = 0f;
        }

        var startTransform = m_StartPoint.transform;
        m_CameraRig.position = startTransform.position;
        m_Camera.rotation = startTransform.rotation;
        sphericalChanged?.Invoke(null, m_StartPoint);
        m_Current = m_StartPoint;
        StartCoroutine(InitialFadeIn());
    }

    public void NavigateTowards(SphericalHelper target)
    {
        if (m_Animation != null)
            StopCoroutine(m_Animation);

        m_Animation = StartCoroutine(PerformCameraMove(target));
    }

    private IEnumerator InitialFadeIn()
    {
        m_UIOverlay.enabled = true;
        m_UIOverlay.raycastTarget = true;
        
        Color from = Color.black;
        Color to = new Color(0f, 0f, 0f, 0f);

        float time = 0f;
        while (time < m_InitialFadeDuration)
        {
            m_UIOverlay.color = Color.Lerp(from, to, time / m_InitialFadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        m_UIOverlay.raycastTarget = false;
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
        while (time < m_RotationDuration)
        {
            cameraTransform.rotation = Quaternion.Lerp(fromRotation, toRotation, time / m_RotationDuration);
            time += Time.deltaTime;
            yield return null;
        }
        cameraTransform.rotation = toRotation;

        // Delay
        yield return new WaitForSeconds(m_PositionDelay);

        // Movement
        Vector3 fromPosition = m_CameraRig.position;
        Vector3 toPosition = target.transform.position;
        time = 0f;
        while (time < m_PositionDuration)
        {
            float progress = time / m_PositionDuration;
            m_CameraRig.position = Vector3.Lerp(fromPosition, toPosition, progress);
            m_Current.Alpha = Mathf.Lerp(1f, 0f, progress);
            target.Alpha = Mathf.Lerp(0f, 1f, progress);
            time += Time.deltaTime;
            yield return null;
        }
        m_CameraRig.position = toPosition;
        m_Current.Alpha = 0f;
        target.Alpha = 1f;

        sphericalChanged?.Invoke(m_Current, target);
        navigationFinished?.Invoke();

        m_Animation = null;
        m_Current = target;
    }
}
