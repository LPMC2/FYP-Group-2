using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapFloor : ScriptableObject
{
    [SerializeField]
    private Texture2D m_MapTexture;
    public Texture2D MapTexture => m_MapTexture;

    [SerializeField]
    private List<MapLandmark> m_Landmarks;
    public List<MapLandmark> Landmarks => m_Landmarks;
}
