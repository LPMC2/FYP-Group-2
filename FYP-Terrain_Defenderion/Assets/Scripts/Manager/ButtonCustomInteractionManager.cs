using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class ButtonCustomInteractionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private UnityEvent m_ButtonEnterEvent;
    [SerializeField] private UnityEvent m_ButtonExitEvent;
    [SerializeField] private UnityEvent m_ButtonSelectEvent;
    [SerializeField] private UnityEvent m_ButtonDeselectEvent;
    public void OnDeselect(BaseEventData eventData)
    {
        m_ButtonDeselectEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
        m_ButtonEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
        m_ButtonExitEvent.Invoke();   
    }

    public void OnSelect(BaseEventData eventData)
    {
        m_ButtonSelectEvent.Invoke();
    }


}
