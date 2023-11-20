using UnityEngine;

public class ResolutionHelper : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private VoidEventChannelSO m_ScreenResolutionChanged;

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

        m_ScreenResolutionChanged.RaiseEvent();
        m_CurrentWidth = Screen.width;
        m_CurrentHeight = Screen.height;
    }
}
