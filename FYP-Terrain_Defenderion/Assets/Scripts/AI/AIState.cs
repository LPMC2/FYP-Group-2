using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIState
{
    [SerializeField] private State m_AIState = State.Idle;
    public State AiState { get { return m_AIState;} set { m_AIState = value; } }
    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Defend,
        Flee
    }


}
