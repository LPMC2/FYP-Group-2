using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEngine.Localization;

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
    private TMP_Text m_Title;
    [SerializeField]
    private TMP_Text m_Content;
}
