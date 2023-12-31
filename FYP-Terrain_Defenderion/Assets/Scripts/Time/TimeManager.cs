using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
public class TimeManager : NetworkBehaviour
{
    [Header("Unit: second")]
    [SerializeField] private NetworkVariable<float> m_TimeRemain = new NetworkVariable<float>();
    public float TimeRemain { get { return m_TimeRemain.Value; } }
    [SerializeField] private float m_MaxTime = 600;
    [SerializeField] private bool isActive = false;
    [SerializeField] public UnityEvent m_EndTimeEvent;
    public void AddEndTimeEvent(UnityAction unityEvent)
    {
        m_EndTimeEvent.AddListener(unityEvent);
    }
    [SerializeField] private float returnTime = 5f;
    public float ReturnTime { get { return returnTime; } }
    [SerializeField] private bool isReturning = false;
    [Header("Display Settings")]
    [SerializeField] private TMP_Text m_DisplayText;
    public TMP_Text DisplayText { get { return m_DisplayText; } }
    [SerializeField] private string m_TimeDisplayHeader = "";
    public string m_ReturnDisplayHeader = "";
    [SerializeField] private float m_ResetTimer = 0f;
    DisplayBehaviour displayBehaviour;
    [Header("Debug Settings")]
    [SerializeField] private bool debugActive = false;
    [SerializeField] private bool debugEndTimer = false;
    public delegate void CustomEvent();
    public event CustomEvent EndTimeEvent;
    private void Start()
    {
        m_TimeRemain.Value = 0f;
    }
    public void ActiveTimer(float customTime = default)
    {
        if (customTime == default)
            m_TimeRemain.Value = m_MaxTime;
        else if (customTime > 0)
        {
            m_TimeRemain.Value = customTime;
            Debug.Log(customTime);
        }
        if (customTime > 0 || customTime == default)
        {
            Debug.Log("Active!");
            isActive = true;
        }
        if(m_DisplayText != null)
            displayBehaviour = m_DisplayText.GetComponent<DisplayBehaviour>();
    }
    public void ActiveReturnTimer()
    {
        m_TimeRemain.Value = returnTime;
        isActive = true;
        isReturning = true;
        if (m_DisplayText != null)
            displayBehaviour = m_DisplayText.GetComponent<DisplayBehaviour>();
    }
    public void Reset()
    {
        isActive = false;
        m_TimeRemain.Value = 0f;
    }
    private void EndTimeInvoke()
    {
        if(!isReturning)
         m_EndTimeEvent.Invoke();
        isActive = false;
        m_TimeRemain.Value = 0f;
        isReturning = false;
    }
    private void Update()
    {
        #if UNITY_EDITOR
            if(debugActive)
            {
                ActiveTimer();
                debugActive = false;
            }
            if(debugEndTimer)
            {
                EndTimeInvoke();
                debugEndTimer = false;
            }
        #endif
            if(isActive)
        {
            RunTimer();
        }
    }
    private void RunTimer()
    {
        if (m_TimeRemain.Value > 0)
        {
            m_TimeRemain.Value -= Time.deltaTime;
            if (!isReturning)
                SetDisplayText(m_TimeDisplayHeader + "\n" + TimeUnit.getTimeUnit(m_TimeRemain.Value));
            else
            {
                string text = m_DisplayText.text = m_ReturnDisplayHeader + "\n" + "Return in " + (int)(m_TimeRemain.Value) + "s";
                if (displayBehaviour == null)
                    SetDisplayText(text);
                else
                    displayBehaviour.StartFadeInText(text, Color.white, -1f, 1f, 0.5f);
            }
        } else
        {

            StartCoroutine(EndTimerText());
            EndTimeInvoke();
        }

    }
    private IEnumerator EndTimerText()
    {
        
        yield return new WaitForSeconds(m_ResetTimer);
        SetDisplayText("");
    }
    public void SetDisplayText(string value)
    {
        if(m_DisplayText != null) {
            m_DisplayText.text = value;
        }
    }
    public void SetDisplayTMP_Text(TMP_Text text)
    {
        m_DisplayText = text;
    }
}
