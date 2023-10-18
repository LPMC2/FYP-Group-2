using UnityEngine;
using UnityEngine.Events;

public class ResolutionHelper : MonoBehaviour
{
    public static event UnityAction resolutionChanged;

    private int m_CurrentWidth;
    private int m_CurrentHeight;

    private void Start()
    {
        m_CurrentWidth = Screen.width;
        m_CurrentHeight = Screen.height;
    }

    private void LateUpdate()
    {
        if (Screen.width == m_CurrentWidth && Screen.height == m_CurrentHeight)
            return;

        resolutionChanged?.Invoke();
        m_CurrentWidth = Screen.width;
        m_CurrentHeight = Screen.height;
    }
}
