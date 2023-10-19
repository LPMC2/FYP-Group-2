using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class JourneyEntry : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField]
    private LocalizeStringEvent m_StepLocalizeEvent;
    public LocalizedString StepKey
    {
        get => m_StepLocalizeEvent.StringReference;
        set => m_StepLocalizeEvent.StringReference = value;
    }

    [Header("UI")]
    [SerializeField]
    private TMP_Text m_Index;
    public int Index
    {
        get => int.Parse(m_Index.text);
        set => m_Index.text = value.ToString();
    }
    [SerializeField]
    private TMP_Text m_Step;
}
