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

    // Update is called once per frame
    void Update()
    {
        if(m_btnPressed)
        {
            m_buttonBar.fillAmount += Time.deltaTime * m_chargeSpeed;
            if(m_buttonBar.fillAmount >= 1.0f && !isFull)
            {
                m_onBarFullEvent?.Invoke();
                isFull = true;
            }
        } else if(!isFull)
        {
            m_buttonBar.fillAmount -= Time.deltaTime * m_chargeSpeed;
        }
    }
}
