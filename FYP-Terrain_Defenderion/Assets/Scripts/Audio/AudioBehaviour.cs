using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioBehaviour : IAudioBehaviour
{

    [SerializeField] private AudioSO m_audioSO;

    [SerializeField] private List<string> audioList = new List<string>();

    public void PlayAudio(int index)
    {
        AudioManager.Singleton.PlayOneShotSound(audioList[index]);
    }
    public string GetList()
    {
        string listText = "";
        for(int i=0; i< m_audioSO.Audios.Count; i++)
        {
            listText += (i + 1) + " - " + m_audioSO.Audios[i].Name + "\n";
        }
        return listText;
    }

}
public interface IAudioBehaviour
{
    string GetList();
}
