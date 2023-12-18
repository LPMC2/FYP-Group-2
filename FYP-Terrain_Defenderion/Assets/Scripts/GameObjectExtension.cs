using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectExtension : MonoBehaviour
{
    public void SetParentFromGameObjectsWithTag(GameObject parent, string tag)
    {
        foreach(Transform child in parent.transform)
        {
           if (child.CompareTag(tag))
            {
                child.SetParent(parent.transform);
            }
        }
    }
    public static GameObject GetGameObjectWithTagFromChilds(GameObject target, string tag)
    {
        if (target.CompareTag(tag))
        {
            return target;
        }
        foreach (Transform child in target.transform)
        {
            GameObject foundObject = GetGameObjectWithTagFromChilds(child.gameObject, tag);
            if (foundObject != null)
            {
                return foundObject;
            }
        }
        return null;
    }
    public static void RemoveAllObjectsFromParent(Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
