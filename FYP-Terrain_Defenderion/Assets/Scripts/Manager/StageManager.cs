using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField] private NetworkVariable<int> m_currentStage = new NetworkVariable<int>();
    public int CurrentStage
    {
        get { return m_currentStage.Value; }
    }
        [SerializeField] private Goal[] m_StageGoal;
    public Goal StageGoal { get { return m_StageGoal[CurrentStage]; } }
    private void Start()
    {
        if(m_StageGoal[CurrentStage].MainGoalType == Goal.GoalType.Time)
        {
            StageGoal.TimeManager.m_EndTimeEvent.AddListener(StageGoal.GoalComplete);
        }
    }
    private void Update()
    {
        switch(m_StageGoal[CurrentStage].MainGoalType)
        {
            case Goal.GoalType.Destoryed:
            case Goal.GoalType.Inactive:

                break;
            case Goal.GoalType.Time:

                break;
            case Goal.GoalType.custom:
                m_StageGoal[CurrentStage].CustomGoalEvent.Invoke();
                break;
        }
    }
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
    public TimeManager TimeManager { get { return m_TimeManager; } }
    [Header("Goal Type: Custom")]
    [SerializeField] private UnityEvent customGoalEvent;
    public UnityEvent CustomGoalEvent { get { return customGoalEvent; } }
    public enum GoalType
    {
        Destoryed,
        Inactive,
        Time,
        custom
    }
    [Header("Goal Completion")]
    [SerializeField] private UnityEvent m_GoalCompleteEvent;
    public UnityEvent GoalCompleteEvent { get { return m_GoalCompleteEvent; } }
    public void GoalComplete()
    {
        m_GoalCompleteEvent.Invoke();
    }
}
