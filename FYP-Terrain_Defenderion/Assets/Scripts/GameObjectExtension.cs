using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectExtension : MonoBehaviour
{
    public delegate void VoidEvent();
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
    public static void DelayEventInvoke(MonoBehaviour monoBehaviour,VoidEvent targetEvent, float delayTime)
    {
        monoBehaviour.StartCoroutine(DelayEventIEnumerator(targetEvent, delayTime));
    }
    private static IEnumerator DelayEventIEnumerator(VoidEvent targetEvent, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        targetEvent?.Invoke();
    }
    private static Coroutine delayEvent;
    public static void DelayUnityEventInvoke(UnityEvent action, float delayTime)
    {
        if (delayEvent != null)
        {
            GameManager.Singleton.StopCoroutine(delayEvent);
            delayEvent = null;
        }
        delayEvent = GameManager.Singleton.StartCoroutine(DelayUnityEventInvokeCoroutine(action, delayTime));
    }
    private static IEnumerator DelayUnityEventInvokeCoroutine(UnityEvent action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        action.Invoke();
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
    public static T[] FindChildObjectsOfType<T>(Transform parent)
    {
        // Create a list to store the found objects
        var foundObjects = new List<T>();

        // Loop through all child objects
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            // Check if the child object has the specified component
            var component = child.GetComponent<T>();
            if (component != null)
            {
                // Add the found object to the list
                foundObjects.Add(component);
            }

            // Recursively search for child objects
            var childObjects = FindChildObjectsOfType<T>(child);
            foundObjects.AddRange(childObjects);
        }

        // Return the array of found objects
        return foundObjects.ToArray();
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
