using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ObjectAnimationCoroutineBehaviour<T> : MonoBehaviour, IObjectAnimationCoroutineBehaviour
{
    [SerializeField] private UnityEvent m_startAnimationEvent;
    [SerializeField] private UnityEvent m_endAnimationEvent;
    [SerializeField] private UnityEvent m_openAnimationEvent;
    [SerializeField] private UnityEvent m_closeAnimationEvent;
    [SerializeField] private float m_animationSpeed = 1f;
    [SerializeField] private AnimationCurve m_animationCurve; 
    [SerializeField] private T m_StartValue;
    [SerializeField] private T m_EndValue;
    [SerializeField] private State m_state = State.CLOSE;
    public T CurrentValue { get; private set; }
    float timer = 0f;
    public Coroutine coroutine { get; private set; }
    public virtual void StartAnimation()
    {
        SwitchState();
        timer = 0f;
            StopAllCoroutines();
            coroutine = StartCoroutine(CoroutineAnimation());
        
    }
    public void PlayAnimationState(State state)
    {
        m_state = state;
        timer = 0f;
        StopAllCoroutines();
        coroutine = StartCoroutine(CoroutineAnimation());
        switch(m_state)
        {
            case State.OPEN:
                m_openAnimationEvent.Invoke();
                break;
            case State.CLOSE:
                m_closeAnimationEvent.Invoke();
                break;
        }
    }
    private void Start()
    {
        if (m_animationCurve == null || m_animationCurve.length == 0)
        {
            m_animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
        timer = 0f;
        StopAllCoroutines();
        coroutine = StartCoroutine(CoroutineAnimation());
    }
    private void SwitchState()
    {
        //int stateNum = Enum.GetValues(typeof(State)).Length;
        //int nextNum = ((int)stateNum + 1) % stateNum;
        //m_state = (State)nextNum;
        if(m_state == State.OPEN)
        {
            GameObjectExtension.DelayUnityEventInvoke(this,m_closeAnimationEvent , m_animationSpeed);
            m_state = State.CLOSE;
        } else
        {
            m_openAnimationEvent.Invoke();
            m_state = State.OPEN;
        }
    }
    private void SwitchValues<T>(ref T value1, ref T value2)
    {
        T temp = value1;
        value1 = value2;
        value2 = temp;
    }
    private IEnumerator CoroutineAnimation()
    {
        while (true)
        {
            if (timer <= m_animationSpeed)
            {
                if(timer == 0f) { m_startAnimationEvent.Invoke(); }
                timer += Time.deltaTime;
                float t = timer / m_animationSpeed;
                float curveValue = m_animationCurve.Evaluate(t);
                if (typeof(T) == typeof(float))
                {
                    float SValue = (float)(object)(m_StartValue);
                    float EValue = (float)(object)(m_EndValue);
                    if(m_state == State.CLOSE)
                    {
                        SwitchValues(ref SValue, ref EValue);
                    }
                    CurrentValue = (T)(object)Mathf.LerpUnclamped(SValue, EValue, curveValue);
                } else if(typeof(T) == typeof(Vector3))
                {
                    Vector3 Vector3SValue = (Vector3)(object)m_StartValue;
                    Vector3 Vector3EValue = (Vector3)(object)m_EndValue;
                    if (m_state == State.CLOSE)
                    {
                        SwitchValues(ref Vector3SValue, ref Vector3EValue);
                    }
                    CurrentValue = (T)(object)Vector3.LerpUnclamped(Vector3SValue, Vector3EValue, curveValue);
                } else if(typeof(T) == typeof(Vector2))
                {
                    Vector2 Vector2SValue = (Vector2)(object)m_StartValue;
                    Vector2 Vector2EValue = (Vector2)(object)m_EndValue;
                    if (m_state == State.CLOSE)
                    {
                        SwitchValues(ref Vector2SValue, ref Vector2EValue);
                    }
                    CurrentValue = (T)(object)Vector2.LerpUnclamped(Vector2SValue, Vector2EValue, curveValue);
                } else
                {
                    Debug.LogWarning("Error: Incorrect type as T. Only accepts Float, Vector2 and Vector3");
                }
                yield return null;
            } else
            {
                timer = 0f;
                m_endAnimationEvent.Invoke();
                StopAllCoroutines();
                coroutine = null;
            }
        }
        
    }
    public enum State
    {
        OPEN,
        CLOSE
    }
}
public interface IObjectAnimationCoroutineBehaviour
{
    void StartAnimation();
}
