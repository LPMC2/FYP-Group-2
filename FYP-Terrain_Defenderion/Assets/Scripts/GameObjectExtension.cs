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
    
}
