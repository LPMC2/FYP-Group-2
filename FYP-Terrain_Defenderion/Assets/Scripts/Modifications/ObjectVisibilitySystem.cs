using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisibilitySystem : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float distance = 100f;
    [SerializeField] private float frequency = 10;
    private List<Transform> objList;

    private void OnEnable()
    {
        objList = new List<Transform>();
        ScanObjects();
    }

    private void Update()
    {
        // Perform object visibility check with a desired frequency
        if (Time.frameCount % frequency == 0)
        {
            CheckObjectVisibility();
        }
    }

    private void ScanObjects()
    {
        // Scan through the entire hierarchy with specific layer mask and store objects in the list
        objList.Clear();
        Transform[] allObjects = FindObjectsOfType<Transform>();
        foreach (Transform obj in allObjects)
        {
            if (((1 << obj.gameObject.layer) & layerMask) != 0)
            {
                objList.Add(obj);
            }
        }
    }

    private void CheckObjectVisibility()
    {
        foreach (Transform obj in objList)
        {
            // Calculate the distance between the player and the object
            float distanceToObj = Vector3.Distance(transform.position, obj.position);

            // Disable mesh renderer and set convex to true if the object is further than the distance variable
            if (distanceToObj > distance)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }

                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.convex = true;
                }
            }
            // Enable mesh renderer and set convex to false if the object is within the distance variable
            else
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                }

                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.convex = false;
                }
            }
        }
    }
}
