using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class RespawnManager
{
    public GameObject target { get; private set; }
    public float RespawnTime { get; private set; }
    public RespawnManager(GameObject targetObject, float time)
    {
        target = targetObject;
        RespawnTime = time;
    }

}
