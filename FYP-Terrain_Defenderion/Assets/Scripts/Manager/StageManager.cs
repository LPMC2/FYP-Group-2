using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public static StageManager Singleton;
    [SerializeField] private bool isActive = false;
    public void SetActive(bool state)
    {
        isActive = state;
        if (state == true)
        {
            if (StageGoal.TimeLimit > -2 && StageGoal.TimeManager != null)
            {
                m_StageGoal[m_currentStage.Value].TimeManager.ActiveTimer(StageGoal.TimeLimit);
            }
        }
    }
    [SerializeField] private NetworkVariable<int> m_currentStage = new NetworkVariable<int>();
    public void ResetStage()
    {
        m_currentStage.Value = -1;
        foreach(Goal goal in m_StageGoal)
        {
            goal.Reset();
            
        }
        WinStage = false;
        isActive = false;
    }
    public void AddStage()
    {
        isActive = false;
        if (CurrentStage != -1 && m_StageGoal[CurrentStage].MainGoalType == Goal.GoalType.Time)
        {
            StageGoal.TimeManager.m_EndTimeEvent.RemoveAllListeners();
        }
        if (CurrentStage >=0 && CurrentStage < m_StageGoal.Length)
        m_StageGoal[CurrentStage].goalCompleted = false;
        m_currentStage.Value++;
        m_StageGoal[m_currentStage.Value].goalCompleted = false;
        if (m_StageGoal[CurrentStage].TimeLimit > 0 && m_StageGoal[CurrentStage].TimeManager != null)
        {
            StageGoal.TimeManager.m_EndTimeEvent.AddListener(StageGoal.GoalComplete);
        }

    }
    public int CurrentStage
    {
        get { return m_currentStage.Value; }
    }
    [SerializeField] private Goal[] m_StageGoal;
    public Goal StageGoal { get { return m_StageGoal[CurrentStage]; } }
    public bool WinStage = false;
    private void Awake()
    {
        Singleton = this;
       
    }
    private void Update()
    {
        GoalDetection();
    }
    private void GoalDetection()
    {
        if (!isActive) return;
        if (CurrentStage < 0 || CurrentStage > m_StageGoal.Length) return;
        if (StageGoal.goalCompleted) return;
        switch (StageGoal.MainGoalType)
        {
            case Goal.GoalType.Destoryed:
            case Goal.GoalType.Inactive:
                foreach(GoalGameObject target in  m_StageGoal[CurrentStage].TargetGameObject)
                {
                    if(target.Target == null || target.Target.activeInHierarchy ==false)
                    {
                        if(!target.IsOwnSide)
                        {
                            WinStage = true;
                        }
                        StageGoal.GoalCompleteEvent.Invoke();
                        m_StageGoal[CurrentStage].goalCompleted = true;
                        break;
                    }
                }

                break;
            case Goal.GoalType.Time:

                break;
            case Goal.GoalType.custom:
                m_StageGoal[CurrentStage].CustomGoalEvent.Invoke();
                break;
        }
    }
    public void TimeOutDetection()
    {
        if (StageGoal.TimeManager.TimeRemain > 0f || StageGoal.MainGoalType == Goal.GoalType.Time) return;
        int count = 0;
        GoalGameObject goalGameObject = StageGoal.TargetGameObject[0];
        HealthBehaviour goalHealthGameObject = null;
        foreach (GoalGameObject target in m_StageGoal[CurrentStage].TargetGameObject)
        {

            HealthBehaviour healthBehaviour = target.Target.GetComponent<HealthBehaviour>();
            if (healthBehaviour != null)
            {
                if (goalHealthGameObject == null || goalHealthGameObject.GetHealth() > healthBehaviour.GetHealth())
                {
                    goalGameObject = target;
                    goalHealthGameObject = healthBehaviour;
                }
            }

            count++;
        }
        if (count == m_StageGoal[CurrentStage].TargetGameObject.Length)
        {
            if (!goalGameObject.IsOwnSide)
            {
                WinStage = true;
            }
        }
    }
}
[Serializable]
public struct Goal
{
    [SerializeField] private GoalType goalType;
    public GoalType MainGoalType { get { return goalType; } }
    [Header("Goal Type: Destroyed / Inactive")]
    [SerializeField] private GoalGameObject[] m_goalGameObject;
    public GoalGameObject[] TargetGameObject { get { return m_goalGameObject; } }
    public List<GameObject> ResultGameObject { 
        get
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach(GoalGameObject gameObject in m_goalGameObject)
            {
                if(gameObject.Target.activeInHierarchy) { gameObjects.Add(gameObject.Target); }
            }
            return gameObjects;
        }
    }

    [Header("Goal Type: Time")]
    [SerializeField] private TimeManager m_TimeManager;
    [SerializeField] private int m_TimeLimit;
    public int TimeLimit { get { return m_TimeLimit; } }
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
    public bool goalCompleted;
    public void Reset()
    {
        foreach(GoalGameObject gameObject in m_goalGameObject)
        {
            gameObject.SetActive(true);
        }
        goalCompleted = false;

    }

}
[Serializable]
public struct GoalGameObject
{
    [SerializeField] private GameObject targetObj;
    [SerializeField] private bool isOwnSide;
    [SerializeField]
    public bool IsOwnSide
    {
        get { return isOwnSide; }
        set => isOwnSide = value;
    }
    public GameObject Target { get { return targetObj; } }
    public void SetActive(bool state)
    {
        targetObj.SetActive(state);
    }
}