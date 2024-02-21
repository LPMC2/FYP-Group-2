using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ObjectAnimationCoroutineBehaviour<T> : MonoBehaviour, IObjectAnimationCoroutineBehaviour
{

    [SerializeField] private UnityEvent m_openAnimationEvent;
    [SerializeField] private UnityEvent m_closeAnimationEvent;
    [SerializeField] private float m_animationSpeed = 1f;
    [SerializeField] private T m_StartValue;
    [SerializeField] private T m_EndValue;
    [SerializeField] private State m_state = State.CLOSE;
    public T currentValue { get; private set; }
    float timer = 0f;
    public Coroutine coroutine { get; private set; }
    public virtual void StartAnimation()
    {
        SwitchState();
        timer = 0f;
            StopAllCoroutines();
            coroutine = StartCoroutine(CoroutineAnimation());
        
    }
    private void Start()
    {
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
            GameObjectExtension.DelayUnityEventInvoke(m_closeAnimationEvent, m_animationSpeed);
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

                timer += Time.deltaTime;
                float t = timer / m_animationSpeed;

                if (typeof(T) == typeof(float))
                {
                    float SValue = (float)(object)(m_StartValue);
                    float EValue = (float)(object)(m_EndValue);
                    if(m_state == State.CLOSE)
                    {
                        SwitchValues(ref SValue, ref EValue);
                    }
                    currentValue = (T)(object)Mathf.Lerp(SValue, EValue, t);
                } else if(typeof(T) == typeof(Vector3))
                {
                    Vector3 Vector3SValue = (Vector3)(object)m_StartValue;
                    Vector3 Vector3EValue = (Vector3)(object)m_EndValue;
                    if (m_state == State.CLOSE)
                    {
                        SwitchValues(ref Vector3SValue, ref Vector3EValue);
                    }
                    currentValue = (T)(object)Vector3.Lerp(Vector3SValue, Vector3EValue, t);
                } else if(typeof(T) == typeof(Vector2))
                {
                    Vector2 Vector2SValue = (Vector2)(object)m_StartValue;
                    Vector2 Vector2EValue = (Vector2)(object)m_EndValue;
                    if (m_state == State.CLOSE)
                    {
                        SwitchValues(ref Vector2SValue, ref Vector2EValue);
                    }
                    currentValue = (T)(object)Vector2.Lerp(Vector2SValue, Vector2EValue, t);
                } else
                {
                    Debug.LogWarning("Error: Incorrect type as T. Only accepts Float, Vector2 and Vector3");
                }
                yield return null;
            } else
            {
                timer = 0f;
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
