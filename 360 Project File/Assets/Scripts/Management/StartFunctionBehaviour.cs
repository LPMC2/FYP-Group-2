using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StartFunctionBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent startFunction;
    // Start is called before the first frame update
    void Start()
    {
        startFunction.Invoke(); 
    }

}
