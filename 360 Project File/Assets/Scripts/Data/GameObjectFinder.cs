using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectFinder 
{
    public static GameObject GetGameObjectWithTagFromChilds(GameObject target, string tag)
    {
        if(target.CompareTag(tag))
        {
            return target;
        }
        foreach(Transform child in target.transform)
        {
            GameObject foundObject = GetGameObjectWithTagFromChilds(child.gameObject, tag);
            if(foundObject != null)
            {
                return foundObject;
            }
        }
        return null;
    }
   
}
