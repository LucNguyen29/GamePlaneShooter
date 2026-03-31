using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSyncManager : MonoBehaviour
{
    public static VolumeSyncManager Instance;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    public delegate void VolumeChanged(float value);
    public event VolumeChanged OnMusicVolumeChanged;
    public event VolumeChanged OnSFXVolumeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load từ PlayerPrefs
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("musicVolume", value);
        OnMusicVolumeChanged?.Invoke(value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        OnSFXVolumeChanged?.Invoke(value);
    }
}
