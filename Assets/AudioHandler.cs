using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [Header("Audios")]
    public List<Audio> audios = new List<Audio>();
    [Header("Settings")]
    public bool SupressMUSIC;
    public bool SupressSFX;
    public float MUSIC_Volume;
    public float SFX_Volume;
    
    public Audio GetAudioByName(string name)
    {
        foreach(Audio audio in audios)
        {
            if (audio.name == name)
            {
                return audio;
            }
        }
        return null;
    }

    public void SetAudioGroupVolume(Audio.AudioType group, float volume)
    {
        foreach(Audio audio in audios)
        {
            if (audio.group == group) { audio.volume = volume; }
        }
    }

    public void UpdateAudios()
    {
        if (!SupressMUSIC)
        {
            SetAudioGroupVolume(Audio.AudioType.MUSIC, MUSIC_Volume);
        } else
        {
            SetAudioGroupVolume(Audio.AudioType.MUSIC, 0f);
        }
        if (!SupressSFX)
        {
            SetAudioGroupVolume(Audio.AudioType.SFX, SFX_Volume);
        } else
        {
            SetAudioGroupVolume(Audio.AudioType.SFX, 0f);
        }
    }
}

[Serializable]
public class Audio
{
    public string name;
    public AudioSource source;
    public float volume = 1f;
    public float pitch = 1f;
    public AudioType group;

    public enum AudioType { SFX, MUSIC }

    public Audio(string name, AudioSource source, float volume, float pitch, AudioType group)
    {
        this.name = name;
        this.source = source;
        this.volume = volume;
        this.pitch = pitch;
        this.group = group;
        UpdateSourceAttributes();
    }

    public void UpdateSourceAttributes()
    {
        source.volume = volume;
        source.pitch = pitch;
    }

    public void Play()
    {
        UpdateSourceAttributes();
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public IEnumerator FadeIn()
    {
        Play();
        for (float i = 0; i <= volume; i += Time.deltaTime)
        {
            source.volume = i;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float startingVolume = source.volume;
        for (float i = startingVolume; i > 0; i -= Time.deltaTime)
        {
            source.volume = i;
            yield return null;
        }
        source.Stop();
    }
}