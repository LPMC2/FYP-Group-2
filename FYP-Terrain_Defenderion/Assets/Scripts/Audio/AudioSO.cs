using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="AudioData", menuName ="Audio/CreateAudioData")]
public class AudioSO : ScriptableObject
{
    [SerializeField] private List<Audio> audios = new List<Audio>();
    public List<Audio> Audios { get { return audios; } }


}
[System.Serializable]
public class Audio
{
    [SerializeField] private string m_Name;
    public string Name { get { return m_Name; } }
    [SerializeField] private AudioClip m_audioClip;
    public AudioClip Clip { get { return m_audioClip; } }
    [SerializeField] private Tag m_audioTag;
    public Tag AudioTag { get { return m_audioTag; } }
    public enum Tag
    {
        DEFAULT,
        MUSIC,
        SOUND_EFFECT
    }
}