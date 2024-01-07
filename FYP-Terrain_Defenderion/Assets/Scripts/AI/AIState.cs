using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIState
{
    [SerializeField] private State m_AIState = State.Idle;
    public State GetAIState()
    {
        return m_AIState;
    }
    public State CurrentAIState { get { return m_AIState;} set { m_AIState = value; } }
    [SerializeField] private State m_MainAIState = State.Idle;
    public State MainAIState { get { return m_MainAIState; } }
    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Defend,
        Flee
    }


}
