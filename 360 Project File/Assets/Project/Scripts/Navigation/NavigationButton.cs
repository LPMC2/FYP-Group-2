using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField]
    private Sprite m_UnlockedSprite;
    [SerializeField]
    private Sprite m_LockedSprite;

    [Header("UI")]
    [SerializeField]
    private Image m_Sprite;

    public NavigationPoint NavPoint { get; set; }
    private bool m_Locked;
    public bool Locked
    {
        get => m_Locked;
        set
        {
            if (m_Locked == value)
                return;

            m_Sprite.sprite = value ? m_LockedSprite : m_UnlockedSprite;
            m_Locked = value;
        }
    }

    public void Interact()
        => NavPoint?.Navigate();
}
