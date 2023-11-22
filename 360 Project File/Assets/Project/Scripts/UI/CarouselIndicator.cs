using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CarouselIndicator : MonoBehaviour
{
    public static Color k_InactiveColor = new Color(0.78f, 0.78f, 0.78f);
    public static Color k_ActiveColor = new Color(0f, 0f, 0f);

    private bool m_Active;
    public bool Active
    {
        get => m_Active;
        set
        {
            m_Image.color = value ? k_ActiveColor : k_InactiveColor;
            m_Active = value;
        }
    }

    private Image m_Image;

    private void Awake()
        => m_Image = GetComponent<Image>();
}
