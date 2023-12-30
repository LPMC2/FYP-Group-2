using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public static StageManager Singleton;
    [SerializeField] private NetworkVariable<int> m_currentStage = new NetworkVariable<int>();
    public void ResetStage()
    {
        m_currentStage.Value = -1;
        foreach(Goal goal in m_StageGoal)
        {
            goal.Reset();
            
        }
        WinStage = false;
    }
    public void AddStage()
    {
        if(CurrentStage >=0 && CurrentStage < m_StageGoal.Length)
        m_StageGoal[CurrentStage].goalCompleted = false;
        m_currentStage.Value++;
        if (m_StageGoal[CurrentStage].MainGoalType == Goal.GoalType.Time)
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
}