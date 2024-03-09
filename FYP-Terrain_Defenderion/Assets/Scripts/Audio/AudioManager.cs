using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton;
    AudioSource audioSource;
    public void SetVolume(float value)
    {
        if(audioSource != null)
        audioSource.volume = value;
    }
    public void SetSFXVolume(float value)
    {
        if(sfxAudioSource != null)
        {
            sfxAudioSource.volume = value;
        }
    }
    public static float SoundEffectVolume { get { return SettingsManager.Singleton.SettingsData.Settings.MainVolume.Value; } }
    public static float MusicVolune { get { return SettingsManager.Singleton.SettingsData.Settings.MusicVolume.Value; } }
    private static float audioVolume = 1f;
    public static float AudioVolume { get { return audioVolume; } set { audioVolume = value; } }
    [SerializeField] private AudioSO audioSO;
    [SerializeField] private float m_LoopDelay = 3f;
    #region Audio Clip Getter
    /// <summary>
    /// Find requested Audio clip with name
    /// </summary>
    /// <param name="nameOfSound"></param>
    /// <returns></returns>
    public AudioClip GetAudioClip(string nameOfSound)
    {
        if (audioSO == null) return null;
        foreach(Audio audio in audioSO.Audios)
        {
            if(audio.Name == nameOfSound)
            {
                return audio.Clip;
            }
        }
        return null;
    }
    /// <summary>
    /// Find requested audio clip with name but certain tag only for faster calculation
    /// </summary>
    /// <param name="nameOfSound"></param>
    /// <param name="includedTag"></param>
    /// <returns></returns>
    public AudioClip GetAudioClip(string nameOfSound, Audio.Tag includedTag)
    {
        if (audioSO == null) return null;
        foreach (Audio audio in audioSO.Audios)
        {
            if (audio.AudioTag != includedTag) continue;
            if (audio.Name == nameOfSound)
            {
                return audio.Clip;
            }
        }
        return null;
    }
    /// <summary>
    /// Get all audio clips within a certain tag and returns with a list
    /// </summary>
    /// <param name="nameOfTag"></param>
    /// <returns></returns>
    public List<AudioClip> GetAudioClipsFromTag(Audio.Tag nameOfTag)
    {
        if (audioSO == null) return null;
        List<AudioClip> audioClips = new List<AudioClip>();
        foreach(Audio audio in audioSO.Audios)
        {
            if(audio.AudioTag == nameOfTag)
            {
                audioClips.Add(audio.Clip);
            }
        }
        return audioClips;
    }
    #endregion

    public float fadeDuration = 1.0f;

    private float initialVolume;
    private float fadeTimer;
    private AudioSource sfxAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sfxAudioSource = new GameObject("SFX Manager").AddComponent<AudioSource>();
        sfxAudioSource.transform.SetParent(transform);
        sfxAudioSource.priority = 130;
        sfxAudioSource.volume = 1f;
    }
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Singleton = this;
    }
    public void ModifyVolume(float value)
    {
        audioSource.volume = value;
    }
    public string GetAudioNameFromIndex(int index)
    {
        return audioSO.Audios[index].Name;
    }

    #region Play/Stop Audio Functions
    public void RandomPlayOneShotSound(List<string> listOfAudio)
    {
        int index = Mathf.CeilToInt(Random.Range(0, listOfAudio.Count));
        AudioClip clip = GetAudioClip(listOfAudio[index]);
        if(clip != null)
        {
            PlayOneShotSound(clip);
        } else
        {
#if UNITY_EDITOR
            Debug.LogError("Error: Unable to find " + listOfAudio[index]);
#endif
        }
    }
    public void PlayOneShotSound(string nameOfAudio)
    {
        AudioClip audioClip = GetAudioClip(nameOfAudio);
        if (audioClip != null)
        {
            sfxAudioSource.PlayOneShot(audioClip);
        }
    }
    public void PlayOneShotSound(string nameOfAudio, Vector3 position)
    {
        AudioClip audioClip = GetAudioClip(nameOfAudio);
        if (audioClip != null)
        {
            AudioSource audioSource = new GameObject("Sound Effect: " + nameOfAudio).AddComponent<AudioSource>();
            audioSource.transform.position = position;
            audioSource.volume = sfxAudioSource.volume;
            audioSource.PlayOneShot(audioClip);
            Destroy(audioSource.gameObject, audioClip.length);
        }
    }
    public void PlayOneShotSound(AudioClip audioClip, Vector3 position)
    {
        if (audioClip != null)
        {
            AudioSource audioSource = new GameObject("Sound Effect: " + audioClip.name).AddComponent<AudioSource>();
            audioSource.transform.position = position;
            audioSource.volume = sfxAudioSource.volume;
            audioSource.PlayOneShot(audioClip);
            Destroy(audioSource.gameObject, audioClip.length);
        }
    }
    public void PlayOneShotSound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
    public void PlayAudioNonRepeatable(string nameOfAudio)
    {
        if (audioSource == null ) return;
        AudioClip clip = GetAudioClip(nameOfAudio);
        if (audioSource.clip != null)
        {
            StartCoroutine(ResetAndPlay(clip));
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void PlayOneShotLoop(AudioClip audioClip, Audio.Tag audioType)
    {
        StartCoroutine(PlayOneShotLoopIEnumerator(audioClip, audioType));
    }
    private IEnumerator PlayOneShotLoopIEnumerator(AudioClip audioClip, Audio.Tag audioType)
    {
        Debug.Log("Start");
        yield return new WaitForSeconds(.01f);
        while (true)
        {
            Debug.Log("Loop");
            if (audioType == Audio.Tag.MUSIC) {
                audioSource.PlayOneShot(audioClip, MusicVolune);
            } else
            {
                audioSource.PlayOneShot(audioClip, SoundEffectVolume);
            }
            yield return new WaitForSeconds(audioClip.length + m_LoopDelay);
        }
    }
    public IEnumerator ResetAndPlay(AudioClip target)
    {
        StopMusic();
        yield return new WaitForSeconds(fadeDuration);
        audioSource.clip = target;
        audioSource.Play();
    }

    public void StopMusic()
    {
        // Start the fade-out process
        fadeTimer = fadeDuration;
        initialVolume = audioSource.volume;
    }
    #endregion

    private void Update()
    {
        // Gradually decrease volume during fade-out
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;

            if (fadeTimer <= 0)
            {
                // Stop the sound when fade-out is complete
                audioSource.volume = initialVolume;
                audioSource.Stop();
            }
            else
            {
                // Calculate the new volume based on the remaining fade duration
                float t = fadeTimer / fadeDuration;
                audioSource.volume = initialVolume * t;
            }
        }
    }
   
}
