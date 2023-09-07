using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Control : MonoBehaviour
{
    private float rotateSpeed = 1.5f;
    private float zoomSpeed = 15f;
    private float zoomAmount = 60f;
    private float fovMin = 30f, fovMax = 90f;

    private Camera m_Camera;
    private Vector2 rotValue;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        CameraController();
    }

    private void CameraController()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            rotValue.x += Input.GetAxis("Mouse X") * rotateSpeed;
            rotValue.y += Input.GetAxis("Mouse Y") * rotateSpeed;
            m_Camera.transform.localRotation = Quaternion.Euler(-rotValue.y, rotValue.x, 0);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Zoom camera
        zoomAmount = Mathf.Clamp(zoomAmount + Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, fovMin, fovMax);
        m_Camera.fieldOfView = zoomAmount;
    }

    public void ChangeCameraRotate(float y)
    {
        rotValue.x += y;
        m_Camera.transform.localRotation = Quaternion.Euler(-rotValue.y, rotValue.x, 0);
    }
}
