using NabaGame.Core.Runtime.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip[] Clip;
    public AudioClip[] musicClip;
    [SerializeField] private AudioSource _moveAudio;
    // ============================
    // SLIDER CONTROL
    // ============================

    private float _musicVolumeCache = 1f;

    public void SetMusic(bool isOn)
    {
        if (isOn)
        {
            musicSource.volume = _musicVolumeCache;
        }
        else
        {
            _musicVolumeCache = musicSource.volume;
            musicSource.volume = 0f;
        }
    }

    private float _sfxVolumeCache = 1f;

    public void SetSFX(bool isOn)
    {
        if (isOn)
        {
            sfxSource.volume = _sfxVolumeCache;
            _moveAudio.volume = _sfxVolumeCache;
        }
        else
        {
            _sfxVolumeCache = sfxSource.volume;
            sfxSource.volume = 0f;
            _moveAudio.volume = 0;
        }
    }


    public override void Init()
    {

    }
    private void Start()
    {
        PlayMusic(0);
    }
    public void PlayMusic(int index)
    {
        AudioClip clip = musicClip[index];

        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(SoundType type)
    {
        sfxSource.PlayOneShot(Clip[(int)type]);
    }
    public void PlayBtnSound()
    {
        PlaySFX(SoundType.Button);
    }
    public void PlaySoundMove()
    {
         _moveAudio.Play();

    }
    public void StopSoundMove()
    {
        _moveAudio.Stop();

    }
}
public enum SoundType : byte
{
    Button = 0,
    Bite,
    Force,
    Eat,
    Jump, 
    Recive,
    Upgrade,
    Sell
}