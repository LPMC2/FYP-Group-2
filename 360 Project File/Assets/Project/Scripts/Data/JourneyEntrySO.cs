using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class JourneyEntrySO : ScriptableObject
{
    [SerializeField]
    private LocalizedString m_StepKey;
    public LocalizedString StepKey => m_StepKey;
}
