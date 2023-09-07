using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float m_RotateSensitivity;
    [SerializeField]
    private float m_ZoomSensitivity;
    [SerializeField]
    private float m_FovMin;
    [SerializeField]
    private float m_FovMax;

    private Camera m_Camera;
    private Vector2 m_CameraRotation;
    private float m_CameraZoom;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        m_CameraZoom = m_Camera.fieldOfView;
    }

    private void Update()
    {
        // Rotation
        var active = Input.GetMouseButton(1);
        if (active)
        {
            m_CameraRotation.x += Input.GetAxis("Mouse X") * m_RotateSensitivity;
            m_CameraRotation.y += Input.GetAxis("Mouse Y") * m_RotateSensitivity;
            m_Camera.transform.localRotation = Quaternion.Euler(-m_CameraRotation.y, m_CameraRotation.x, 0);
        }
        Cursor.visible = !active;
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;

        // Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            m_CameraZoom = Mathf.Clamp(m_CameraZoom + -Input.GetAxis("Mouse ScrollWheel") * m_ZoomSensitivity, m_FovMin, m_FovMax);
            m_Camera.fieldOfView = m_CameraZoom;
        }
    }
}
