using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{
    

    public RectTransform panelRectTransform;
    public Vector2 newPosition;
    private void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
    }

    public void ChangePanelPosition()
    {
        panelRectTransform.anchoredPosition = newPosition;
    }

    private float rotateSpeed = 1.5f;
    private float zoomSpeed = 15f;
    private float zoomAmount = 60f;

    private GameObject gameObjectValue;
    private Vector3 rotValue;
    public bool is_needChangeRotate = false;
    // Update is called once per frame
    private void Update()
    {
        CameraController();
    }

    private void CameraController()
    {
        if (is_needChangeRotate)
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                rotValue.z += Input.GetAxis("Mouse X") * rotateSpeed;
                transform.localRotation = Quaternion.Euler(0, 0, -rotValue.z);
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

    }

    public void ChangeCameraRotate(float y)
    {
        rotValue.z += y;
            transform.localRotation = Quaternion.Euler(0,0, rotValue.z);
    }

    public void changeposition(Vector3 poA)
    {
        transform.position = poA;
    }
}
