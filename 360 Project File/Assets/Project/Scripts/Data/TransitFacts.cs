using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class TransitFacts : ScriptableObject
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
