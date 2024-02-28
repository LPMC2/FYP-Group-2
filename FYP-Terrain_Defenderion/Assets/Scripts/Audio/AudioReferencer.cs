using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReferencer : MonoBehaviour
{
    [SerializeField] private string m_nameOfSound;
    public void PlayOneShot(string nameOfSound)
    {
        if(AudioManager.Singleton != null)
            AudioManager.Singleton.PlayOneShotSound(nameOfSound);
    }
    public void PlayOneShot()
    {
        if (AudioManager.Singleton != null)
            AudioManager.Singleton.PlayOneShotSound(m_nameOfSound);
    }
}
