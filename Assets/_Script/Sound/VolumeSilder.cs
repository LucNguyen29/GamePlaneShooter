using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSilder : MonoBehaviour
{
    public enum VolumeType { Music, SFX }
    public VolumeType volumeType;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {

        if (VolumeSyncManager.Instance == null) return;

        switch (volumeType)
        {
            case VolumeType.Music:
                slider.value = VolumeSyncManager.Instance.musicVolume;
                slider.onValueChanged.AddListener(VolumeSyncManager.Instance.SetMusicVolume);
                VolumeSyncManager.Instance.OnMusicVolumeChanged += UpdateSlider;
                break;

            case VolumeType.SFX:
                slider.value = VolumeSyncManager.Instance.sfxVolume;
                slider.onValueChanged.AddListener(VolumeSyncManager.Instance.SetSFXVolume);
                VolumeSyncManager.Instance.OnSFXVolumeChanged += UpdateSlider;
                break;
        }
    }

    private void OnDestroy()
    {
        if (VolumeSyncManager.Instance == null) return;

        switch (volumeType)
        {
            case VolumeType.Music:
                VolumeSyncManager.Instance.OnMusicVolumeChanged -= UpdateSlider;
                break;
            case VolumeType.SFX:
                VolumeSyncManager.Instance.OnSFXVolumeChanged -= UpdateSlider;
                break;
        }
    }

    private void UpdateSlider(float value)
    {
        if (slider.value != value)
            slider.value = value;
    }
}
