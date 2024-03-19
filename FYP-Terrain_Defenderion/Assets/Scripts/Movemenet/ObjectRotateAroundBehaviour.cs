using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotateAroundBehaviour : MonoBehaviour
{
    [SerializeField] private RotateType rotateType;
    [SerializeField] private Axis axis;
    [SerializeField] private FacingSide facingSide;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float targetAngle = 90f;
    [SerializeField] private float baseAngle = 0f;
    [SerializeField] private float facingAngle = 0f;
    private void Start()
    {
        if(targetTransform == null)
        {
            targetTransform = gameObject.transform;
        }
    }
    private void Update()
    {
        if (rotateType == RotateType.Continuous)
        {
            // Calculate the desired position in the rotation circle
            Vector3 desiredPosition = rotationPoint.position + Quaternion.Euler(
                axis == Axis.X? baseAngle + Time.time * rotationSpeed : 0f,
                axis == Axis.Y? baseAngle + Time.time * rotationSpeed : 0f,
                axis == Axis.Z? baseAngle + Time.time * rotationSpeed : 0f
                ) * Vector3.forward * distance;

            // Rotate the object around the rotation point
            targetTransform.position = desiredPosition;
            // Calculate the direction from targetTransform to rotationPoint
            Vector3 direction = rotationPoint.position - targetTransform.position;

            // Calculate the reversed look direction
            Vector3 lookDirection = facingSide == FacingSide.FRONT ? direction : -direction;

            // Calculate the desired rotation
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, rotationPoint.up);

            // Apply the rotation to the target transform
            targetTransform.rotation = desiredRotation;
        }
    }
    public void StartRotateAngle(Transform specificGameObject = null)
    {

        if(specificGameObject != null) { targetTransform = specificGameObject; }
        if(rotateType == RotateType.Callable)
            StartCoroutine(RotateObject());
        else
        {
            rotateType = RotateType.Continuous;
        }
    }
    public void ResetRot()
    {
        Vector3 desiredPosition = rotationPoint.position + Quaternion.Euler(
               axis == Axis.X ? baseAngle  : 0f,
               axis == Axis.Y ? baseAngle  : 0f,
               axis == Axis.Z ? baseAngle  : 0f
               ) * Vector3.forward * distance;
        targetTransform.position = desiredPosition;
        // Calculate the direction from targetTransform to rotationPoint
        Vector3 direction = rotationPoint.position - targetTransform.position;

        // Calculate the reversed look direction
        Vector3 lookDirection = facingSide == FacingSide.FRONT ? direction : -direction;

        // Calculate the desired rotation
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, rotationPoint.up);

        // Apply the rotation to the target transform
        targetTransform.rotation = desiredRotation;
    }
    [SerializeField] private float CURRENTANGLE = 0f;
    private IEnumerator RotateObject()
    {
        float currentAngle = 0f;
        while (currentAngle < targetAngle)
        {
            currentAngle += Time.deltaTime * rotationSpeed;
            // Calculate the desired position in the rotation circle
            Vector3 desiredPosition = rotationPoint.position + Quaternion.Euler(
                 axis == Axis.X ? baseAngle + currentAngle : 0f,
                 axis == Axis.Y ? baseAngle + currentAngle : 0f,
                 axis == Axis.Z ? baseAngle + currentAngle : 0f
                 ) * Vector3.forward * distance;

            // Rotate the object around the rotation point
            targetTransform.position = desiredPosition;
            // Calculate the direction from targetTransform to rotationPoint
            Vector3 direction = rotationPoint.position - targetTransform.position;

            // Calculate the reversed look direction
            Vector3 lookDirection = facingSide == FacingSide.FRONT? direction:-direction;

            // Calculate the desired rotation
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, rotationPoint.up);

            // Apply the rotation to the target transform
            targetTransform.rotation = desiredRotation;


            CURRENTANGLE = currentAngle;

            yield return null;
        }
    }
    public enum RotateType
    {
        Continuous,
        Callable
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }
    public enum FacingSide
    {
        FRONT,
        BACK
    }
}
