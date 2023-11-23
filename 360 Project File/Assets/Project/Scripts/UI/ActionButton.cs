using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActionButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private LocalizeStringEvent m_TextLocalizeEvent;
    public LocalizedString TextKey
    {
        get => m_TextLocalizeEvent.StringReference;
        set => m_TextLocalizeEvent.StringReference = value;
    }
    
    [Header("Event Channels")]
    [SerializeField]
    private JourneyEventChannelSO m_JourneyEventChannelSO;

    public JourneyEntrySO JourneyEntry { get; set; }

    private Button m_Button;

    private void Awake()
        => m_Button = GetComponent<Button>();

    private void OnEnable()
        => m_Button.onClick.AddListener(OnButtonClicked);

    private void OnDisable()
        => m_Button.onClick.RemoveListener(OnButtonClicked);

    private void OnButtonClicked()
    {
        if (JourneyEntry == null)
            return;

        m_JourneyEventChannelSO.OnEntryCompleted?.Invoke(JourneyEntry);
    }
}
