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
    public static float SoundEffectVolune { get { return SettingsManager.Singleton.SettingsData.Settings.MainVolume.Value; } }
    private static float audioVolume = 1f;
    public static float AudioVolume { get { return audioVolume; } set { audioVolume = value; } }
    [SerializeField] private List<Audio> audios = new List<Audio>();
    #region Audio Clip Getter
    public AudioClip GetAudioClip(string nameOfSound)
    {
        foreach(Audio audio in audios)
        {
            if(audio.Name == nameOfSound)
            {
                return audio.Clip;
            }
        }
        return null;
    }
    public List<AudioClip> GetAudioClipsFromTag(Audio.Tag nameOfTag)
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        foreach(Audio audio in audios)
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

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
    public void PlayOneShotSound(string nameOfAudio)
    {
        AudioClip audioClip = GetAudioClip(nameOfAudio);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip, SoundEffectVolune);
        }
    }
    public void PlayOneShotSound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, SoundEffectVolune);
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
}
