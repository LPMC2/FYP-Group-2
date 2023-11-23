using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class CarouselPage : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField]
    private LocalizeStringEvent m_TitleLocalizeEvent;
    public LocalizedString TitleKey
    {
        get => m_TitleLocalizeEvent.StringReference;
        set => m_TitleLocalizeEvent.StringReference = value;
    }

    [SerializeField]
    private LocalizeStringEvent m_ContentLocalizeEvent;
    public LocalizedString ContentKey
    {
        get => m_ContentLocalizeEvent.StringReference;
        set => m_ContentLocalizeEvent.StringReference = value;
    }

    [Header("UI")]
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
    private TMP_Text m_Title;
    [SerializeField]
    private TMP_Text m_Content;
}
