using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class NavigationTypeMapping : ScriptableObject
{
    public TypeMapping[] m_TypeMapping;

    public Sprite GetSprite(NavigationPoint.Type type)
        => m_TypeMapping.First(m => m.m_Type == type).m_Sprite;

    [System.Serializable]
    public struct TypeMapping
    {
        public NavigationPoint.Type m_Type;
        public Sprite m_Sprite;
    }
}
