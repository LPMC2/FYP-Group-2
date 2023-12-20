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
    public static void DisableFromTime(MonoBehaviour monoBehaviour ,GameObject gameObject, float time) 
    {
        monoBehaviour.StartCoroutine(DisableTimer(gameObject, time));
    }
    private static IEnumerator DisableTimer(GameObject gameobj, float time)
    {
        yield return new WaitForSeconds(time);
        gameobj.SetActive(false);
    }

    public static void ActivateFromTime(MonoBehaviour monoBehaviour, GameObject gameObject, float time)
    {
        monoBehaviour.StartCoroutine(ActivateTimer(gameObject, time));
    }
    private static IEnumerator ActivateTimer(GameObject gameobj, float time)
    {
        yield return new WaitForSeconds(time);
        gameobj.SetActive(true);
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
        for(int i=0; i< parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

    }
    public static void DisableMeshFromTime(MonoBehaviour monoBehaviour, GameObject gameObject, float time)
    {
        monoBehaviour.StartCoroutine(DisableMeshTimer(gameObject, time));
    }
    private static IEnumerator DisableMeshTimer(GameObject gameobj, float time)
    {
        yield return new WaitForSeconds(time);
        gameobj.GetComponent<MeshRenderer>().enabled = false;
    }

}
