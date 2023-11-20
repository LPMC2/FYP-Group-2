using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float m_RotateSensitivity;
    [SerializeField, Range(0, 89f)]
    private float m_MaxVerticalAngle;
    [SerializeField]
    private float m_ZoomSensitivity;
    [SerializeField]
    private float m_FovMin, m_FovMax;

    [Header("Event Channels")]
    [SerializeField]
    private NavigationEventChannelSO m_NavigationEventChannel;

    private Camera m_Camera;
    private bool m_CameraLocked;
    private Vector2 m_CameraRotation;
    private float m_CameraZoom;

    private void Awake()
        => m_Camera = GetComponent<Camera>();

    private void Start()
        => m_CameraZoom = m_Camera.fieldOfView;

    private void OnEnable()
    {
        m_NavigationEventChannel.OnNavigationStarted += OnNavigationStarted;
        m_NavigationEventChannel.OnNavigationFinished += OnNavigationFinished;
    }

    private void OnDisable()
    {
        m_NavigationEventChannel.OnNavigationStarted -= OnNavigationStarted;
        m_NavigationEventChannel.OnNavigationFinished -= OnNavigationFinished;
    }

    private void Update()
    {
        if (m_CameraLocked)
            return;

        // Rotation
        var active = Input.GetMouseButton(1);
        if (active)
        {
            m_CameraRotation += m_RotateSensitivity * Time.deltaTime * new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            m_CameraRotation.x = ClampAngle(m_CameraRotation.x, -m_MaxVerticalAngle, m_MaxVerticalAngle);
            if (m_CameraRotation.y < 0f)
                m_CameraRotation.y += 360f;
            else if (m_CameraRotation.y >= 360f)
                m_CameraRotation.y -= 360f;
            m_Camera.transform.localEulerAngles = m_CameraRotation;
        }

        // Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            m_CameraZoom = Mathf.Clamp(m_CameraZoom + -Input.GetAxis("Mouse ScrollWheel") * m_ZoomSensitivity, m_FovMin, m_FovMax);
            m_Camera.fieldOfView = m_CameraZoom;
        }
    }

    private void OnNavigationStarted()
        => m_CameraLocked = true;

    private void OnNavigationFinished()
    {
        var newAngle = transform.localEulerAngles;
        newAngle.x = ClampAngle(newAngle.x, -m_MaxVerticalAngle, m_MaxVerticalAngle);
        m_CameraRotation = newAngle;
        m_CameraLocked = false;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270)
        {
            if (angle > 180)
                angle -= 360;
            if (max > 180)
                max -= 360;
            if (min > 180)
                min -= 360;
        }

        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0)
            angle += 360;

        return angle;
    }
}
