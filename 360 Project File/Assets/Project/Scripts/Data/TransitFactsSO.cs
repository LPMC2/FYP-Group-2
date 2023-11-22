using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class TransitFactsSO : ScriptableObject
{
    [SerializeField]
    private Entry[] m_Entries;
    public Entry[] Entries => m_Entries;

    [System.Serializable]
    public struct Entry
    {
        public LocalizedString titleKey;
        public LocalizedString bodyKey;
    }
}
