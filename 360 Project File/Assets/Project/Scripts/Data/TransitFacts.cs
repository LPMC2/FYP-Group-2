using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class TransitFacts : ScriptableObject
{
    [SerializeField]
    private Entry[] m_Entries;
    public Entry[] Entries => m_Entries;

    [System.Serializable]
    public class Entry
    {
        public LocalizedString titleKey;
        public LocalizedString bodyKey;
    }
}
