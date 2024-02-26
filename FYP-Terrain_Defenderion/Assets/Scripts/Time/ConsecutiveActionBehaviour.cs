using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ConsecutiveActionBehaviour : MonoBehaviour, IConsecutiveAction
{
    [SerializeField] private List<Action> actions = new List<Action>();
    public List<Action> Actions { get { return actions; } set { actions = value; } }
    public Coroutine Coroutine { get; private set; }
    public virtual void StartActions()
    {
        if(Coroutine != null)
        {
            StopCoroutine(Coroutine);
        }
        Coroutine = StartCoroutine(ActionEnumerator());
    }
    private IEnumerator ActionEnumerator()
    {

            foreach(Action action in actions)
            {
                action.Events.Invoke();
                yield return new WaitForSeconds(action.TimeToNextAction);
            }

        
    }

    [System.Serializable]
    public class Action
    {
        [SerializeField] private UnityEvent m_events = new UnityEvent();
        [SerializeField] private float m_timeToNextAction = 1f;
        public UnityEvent Events { get { return m_events; } set { m_events = value; } }
        public float TimeToNextAction { get { return m_timeToNextAction; } set { m_timeToNextAction = value; } }
    }
}
public interface IConsecutiveAction
{
    void StartActions();
}
