using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class JourneyEntries : ScriptableObject
{
    [SerializeField]
    private LocalizedString[] m_Entries;
    public LocalizedString[] Entries => m_Entries;
}
