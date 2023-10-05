using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private NavigationTypeMapping m_NavTypeMapping;
    [SerializeField]
    private Image m_Sprite;

    private NavigationPoint m_NavPoint;
    public NavigationPoint NavPoint
    {
        get => m_NavPoint;
        set
        {
            m_NavPoint = value;
            m_Sprite.sprite = m_NavTypeMapping.GetSprite(m_NavPoint.NavigationType);
        }
    }

    public void Interact()
    {
        switch (m_NavPoint.NavigationType)
        {
            case NavigationPoint.Type.Navigate:
                NavigationManager.Instance?.NavigateTowards(m_NavPoint.Destination);
                break;
            case NavigationPoint.Type.Information:
                Debug.Log(m_NavPoint.InfoLocalizeKey);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var navUI = NavigationPointUI.Instance;
        if (navUI != null)
            navUI.LocationDisplay = m_NavPoint.name;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var navUI = NavigationPointUI.Instance;
        if (navUI != null)
            navUI.LocationDisplay = string.Empty;
    }
}
