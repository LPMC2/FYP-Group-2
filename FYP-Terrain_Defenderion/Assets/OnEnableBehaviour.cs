using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableBehaviour : MonoBehaviour
{
    public UnityEvent OnEnableEvent;
    [SerializeField] private float DelayTimer = 0f;
    // Start is called before the first frame update
    public void OnEnable()
    {
        StartCoroutine(DelayEnableEvent());
    }
    private IEnumerator DelayEnableEvent()
    {

        yield return new WaitForSeconds(DelayTimer);
        OnEnableEvent.Invoke();
    }
}
