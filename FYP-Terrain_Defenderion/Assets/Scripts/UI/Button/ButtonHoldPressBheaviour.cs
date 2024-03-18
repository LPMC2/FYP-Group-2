using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoldPressBheaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool m_btnPressed = false;
    [SerializeField] private Image m_buttonBar;
    [SerializeField] private UnityEvent m_onBarFullEvent;
    [SerializeField] private UnityEvent m_onCancelEvent;
    [SerializeField] private UnityEvent m_onPressEvent;
    [SerializeField] private float m_chargeSpeed = 1f;
    private bool isFull = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        m_btnPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        m_btnPressed = false;
    }
    public void ResetBar()
    {
        m_buttonBar.fillAmount = 0f;
        isFull = false;
    }
    private bool isPressed = false;
    private bool isNotFull = false;
    // Update is called once per frame
    void Update()
    {
        if(m_btnPressed)
        {
            if (!isPressed)
            {
                m_onPressEvent?.Invoke();
                isPressed = true;
            }
            m_buttonBar.fillAmount += Time.deltaTime * m_chargeSpeed;
            if(m_buttonBar.fillAmount >= 1.0f && !isFull)
            {
                m_onBarFullEvent?.Invoke();
                isFull = true;
            }
        } else if(!isFull)
        {
            if(!isNotFull && m_buttonBar.fillAmount < 1.0f && m_buttonBar.fillAmount > 0)
            {
                isPressed = false;
                isNotFull = true;
                m_onCancelEvent?.Invoke();
            }
            m_buttonBar.fillAmount -= Time.deltaTime * m_chargeSpeed;
            if(m_buttonBar.fillAmount <= 0.0f) { isNotFull = false; }
        }
    }
}
