using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isRotateable = true;
    #region Setter
    public void SetIsRotateable(bool value)
    {
        isRotateable = value;
    }
    #endregion
    InputManager inputManager;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask collisionLayers;
    private float defaultPosition;
     private Vector3 cameraFollorVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    [SerializeField] private float cameraCollisionOffset = 0.2f;
    [SerializeField] private float minCollisionOffset = 0.2f;
    [SerializeField] private float cameraCollisionRadius = 2f;
    [SerializeField] private float cameraFollowSpeed = 0.2f;
    [SerializeField] private float cameraLookSpeed = 2;
    [SerializeField] private float cameraPivotSpeed = 2;

    [SerializeField] private float minPivotAngle = -35f;
    [SerializeField] private float maxPivotAngle = 35f;

    [SerializeField] private float lookAngle;
    [SerializeField] private float pivotAngle;
    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }
    public void HandleAllCameraMovement()
    {
        FollowTarget();
        if (isRotateable)
        {
            RotateCamera();
        }
        HandleCameraCollisions();
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollorVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }
    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.getCameraInputX() * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.getCameraInputY() * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
