using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCompoundCollider : MonoBehaviour
{
    private BoxCollider compoundCollider;

    private void Awake()
    {
        compoundCollider = gameObject.AddComponent<BoxCollider>();
        compoundCollider.center = GetCombinedCenter();
        compoundCollider.size = GetCombinedSize();
        CheckAndRemoveOverlaps();
    }

    private Vector3 GetCombinedCenter()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        Vector3 combinedCenter = Vector3.zero;
        foreach (Renderer renderer in renderers)
        {
            combinedCenter += renderer.bounds.center;
        }

        combinedCenter /= renderers.Length;

        return combinedCenter;
    }

    private Vector3 GetCombinedSize()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds.size;
    }

    private void CheckAndRemoveOverlaps()
    {
        Collider[] colliders = FindObjectsOfType<Collider>();

        foreach (Collider otherCollider in colliders)
        {
            if (otherCollider != compoundCollider && otherCollider is BoxCollider)
            {
                BoxCollider otherBoxCollider = (BoxCollider)otherCollider;

                if (compoundCollider.bounds.Intersects(otherBoxCollider.bounds))
                {
                    RemoveOverlappingFaces(otherBoxCollider);
                }
            }
        }
    }

    private void RemoveOverlappingFaces(BoxCollider otherBoxCollider)
    {
        Vector3 overlapMin = Vector3.Max(compoundCollider.bounds.min, otherBoxCollider.bounds.min);
        Vector3 overlapMax = Vector3.Min(compoundCollider.bounds.max, otherBoxCollider.bounds.max);

        Vector3 overlapSize = overlapMax - overlapMin;

        if (overlapSize.x > overlapSize.y && overlapSize.x > overlapSize.z)
        {
            if (compoundCollider.bounds.max.x > otherBoxCollider.bounds.max.x)
            {
                compoundCollider.size = new Vector3(overlapMax.x - compoundCollider.bounds.min.x, compoundCollider.size.y, compoundCollider.size.z);
            }
            else
            {
                compoundCollider.size = new Vector3(compoundCollider.bounds.max.x - overlapMin.x, compoundCollider.size.y, compoundCollider.size.z);
                compoundCollider.center += new Vector3(overlapSize.x / 2f, 0f, 0f);
            }
        }
        else if (overlapSize.y > overlapSize.x && overlapSize.y > overlapSize.z)
        {
            if (compoundCollider.bounds.max.y > otherBoxCollider.bounds.max.y)
            {
                compoundCollider.size = new Vector3(compoundCollider.size.x, overlapMax.y - compoundCollider.bounds.min.y, compoundCollider.size.z);
            }
            else
            {
                compoundCollider.size = new Vector3(compoundCollider.size.x, compoundCollider.bounds.max.y - overlapMin.y, compoundCollider.size.z);
                compoundCollider.center += new Vector3(0f, overlapSize.y / 2f, 0f);
            }
        }
        else if (overlapSize.z > overlapSize.x && overlapSize.z > overlapSize.y)
        {
            if (compoundCollider.bounds.max.z > otherBoxCollider.bounds.max.z)
            {
                compoundCollider.size = new Vector3(compoundCollider.size.x, compoundCollider.size.y, overlapMax.z - compoundCollider.bounds.min.z);
            }
            else
            {
                compoundCollider.size = new Vector3(compoundCollider.size.x, compoundCollider.size.y, compoundCollider.bounds.max.z - overlapMin.z);
                compoundCollider.center += new Vector3(0f, 0f, overlapSize.z / 2f);
            }
        }
    }
}