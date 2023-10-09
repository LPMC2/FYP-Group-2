using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{
    

    public RectTransform panelRectTransform;
    public Vector2 newPosition;
    private void Start()
    {
        //panelRectTransform = GetComponent<RectTransform>();
        if (is_needChangeRotate)
        {
            ChangeViewAreaRotate(137);
        }
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
        ViewAreaController();
    }

    private void ViewAreaController()
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

    public void ChangeViewAreaRotate(float z)
    {
        rotValue.z += z;
            transform.localRotation = Quaternion.Euler(0,0, rotValue.z);
    }
    public void ViewAreaRotate(float z)
    {
        rotValue.z = z;
        transform.localRotation = Quaternion.Euler(0, 0, rotValue.z);
    }
    public void changeposition(Vector2 poA)
    {
        transform.position = poA;
    }
}
