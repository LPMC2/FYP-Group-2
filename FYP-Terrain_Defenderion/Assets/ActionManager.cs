using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ActionManager : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    public void InvokeAction()
    {
        action.Invoke();
    }
}
