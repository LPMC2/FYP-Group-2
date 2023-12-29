using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Goal m_StageGoal;
    public Goal StageGoal { get { return m_StageGoal; } }

}
[Serializable]
public class Goal
{
    [SerializeField] private GoalType goalType;
    public GoalType MainGoalType { get { return goalType; } }
    [Header("Goal Type: Destroyed / Inactive")]
    [SerializeField] private GameObject[] GoalGameObject;
    public GameObject[] TargetGameObject { get { return GoalGameObject; } }
    [Header("Goal Type: Time")]
    [SerializeField] private TimeManager m_TimeManager;
    [Header("Goal Type: Custom")]
    [SerializeField] private UnityEvent customGoalEvent;
    public enum GoalType
    {
        Destoryed,
        Inactive,
        Time,
        custom
    }
    [Header("Goal Completion")]
    [SerializeField] private UnityEvent m_GoalCompleteEvent;
}
