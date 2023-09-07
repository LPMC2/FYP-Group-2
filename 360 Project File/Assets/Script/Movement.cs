using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private float rotateSpeed = 1.5f;
    private float zoomSpeed = 15f;
    private float zoomAmount = 60f;
    private float fovMin = 30f, fovMax = 90f;
    private Vector2 rotValue;
    
    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
            mainCamera.transform.localRotation = Quaternion.Euler(-rotValue.y, rotValue.x, 0);
        } else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        // Zoom camera
        zoomAmount = Mathf.Clamp(zoomAmount + Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, fovMin, fovMax);
        mainCamera.fieldOfView = zoomAmount;
    }
    public void ChangeCameraRotate(float y)
    {
        rotValue.x += y;
        mainCamera.transform.localRotation = Quaternion.Euler(-rotValue.y, rotValue.x, 0);
    }
}