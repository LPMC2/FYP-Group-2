using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIState
{
    [SerializeField] private State m_AIState = State.Idle;
    public State CurrentAIState { get { return m_AIState;} set { m_AIState = value; } }
    [SerializeField] private State m_DefaultAIState = State.Idle;
    public State DefaultAIState { get { return m_DefaultAIState; } }
    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Defend,
        Flee
    }


}
