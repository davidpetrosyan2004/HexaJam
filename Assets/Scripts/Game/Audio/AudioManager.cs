using System;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public SoundEffect[] sounds;
    public static AudioManager Instance;
    public bool isHaptics = true;
    private void Awake()
    {
        Vibration.Init();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    public void Start()
    {
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.loop = sound.loop;
            sound.source.volume = sound.volume;
            sound.source.name = sound.name;
            sound.source.clip = sound.clip;
        }
        PlaySound("Theme");
        PlaySound("WaveSound");
    }

    public void PlaySound(string soundName, bool oneShot = false)
    {
        SoundEffect sound = Array.Find(sounds, x => x.name == soundName);
        if (sound != null)
        {
            if (oneShot) sound.source.PlayOneShot(sound.source.clip);
            else sound.source.Play();
        }
    }
    public void StopSound(string soundName)
    {
        SoundEffect sound = Array.Find(sounds, x => x.name == soundName);
        if (sound != null)
        {
            sound.source.Stop();
        }
    }

    public void MusicOff()
    {
        SoundEffect sound = Array.Find(sounds, x => x.name == "Theme");
        if (sound != null)
        {
            sound.source.volume = 0;
        }
    }
    public void SoundOff()
    {
        foreach (var sound in sounds)
        {
            if (sound.name != "Theme")
            {
                sound.source.volume = 0;
            }
        }
    }
    public void MusicOn()
    {
        SoundEffect sound1 = Array.Find(sounds, x => x.name == "Theme");
        if (sound1 != null)
        {
            sound1.source.volume = 0.05f;
        }
        SoundEffect sound2 = Array.Find(sounds, x => x.name == "WaveSound");
        if (sound2 != null)
        {
            sound2.source.volume = 0.05f;
        }

    }
    public void SoundOn()
    {
        foreach (var sound in sounds)
        {
            if (sound.name != "Theme" && sound.name != "WaveSound" && sound.name != "ButtonClick")
            {
                sound.source.volume = 0.5f;
            }
        }
        SoundEffect sound2 = Array.Find(sounds, x => x.name == "ButtonClick");
        if (sound2 != null)
        {
            sound2.source.volume = 0.8f;
        }
    }

    public void EncreaseVolumePitch()
    {
        SoundEffect sound = Array.Find(sounds, x => x.name == "TurtlesMatch");
        sound.source.pitch += 0.05f;
    }

    public void ClearPitch()
    {
        SoundEffect sound = Array.Find(sounds, x => x.name == "TurtlesMatch");
        sound.source.pitch = 1f;
    }

    public void Vibrate()
    {
        if(isHaptics)
            Vibration.VibratePop();
    }
}
