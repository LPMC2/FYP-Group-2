using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarouselPage : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;
    public float Alpha
    {
        get => m_CanvasGroup.alpha;
        set => m_CanvasGroup.alpha = value;
    }
    public bool BlockRaycasts
    {
        get => m_CanvasGroup.blocksRaycasts;
        set => m_CanvasGroup.blocksRaycasts = value;
    }

    [SerializeField]
    private Image m_Image;
    public Sprite Image
    {
        get => m_Image.sprite;
        set => m_Image.sprite = value;
    }

    [SerializeField]
    private TMP_Text m_Caption;
    public string Caption
    {
        get => m_Caption.text;
        set => m_Caption.text = value;
    }
}
