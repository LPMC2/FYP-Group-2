using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineTransitionManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cinemachine;
    private void Start()
    {
        if(cinemachine ==null)
        {
            cinemachine = GetComponent<CinemachineBrain>();
        }
    }
    public void SetTransitionTime(float time)
    {
        cinemachine.m_DefaultBlend.m_Time = time;
    }
}
