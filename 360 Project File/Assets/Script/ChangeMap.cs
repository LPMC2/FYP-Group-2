using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{


    public RectTransform panelRectTransform;
    public Vector2 newPosition;
    public Camera maincamera;
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
    private Quaternion cameraRotation;
    private void ViewAreaController()
    {
        if (is_needChangeRotate)
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            cameraRotation = maincamera.transform.rotation;
            //rotValue = cameraRotation.eulerAngles;
            //transform.localRotation = cameraRotation;
            transform.rotation = Quaternion.Euler(0f, 0f, -cameraRotation.eulerAngles.y - 137);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void ChangeViewAreaRotate(float z)
    {
        rotValue.z += z;
        transform.localRotation = Quaternion.Euler(0, 0, rotValue.z);
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
