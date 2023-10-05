using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T k_Instance;
    public static T Instance => k_Instance;

    protected virtual void Awake()
    {
        if (k_Instance != null)
        {
            Debug.LogWarning($"[Singleton] A component of type '{GetType()}' already exists. This component will be destoryed.", gameObject);
            Destroy(this);
            return;
        }

        if (k_Instance == null)
            k_Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (k_Instance == this)
            k_Instance = null;
    }
}
