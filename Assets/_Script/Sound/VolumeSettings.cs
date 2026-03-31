using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    // Start is called before the first frame update

    private void Start()
    {
        // Lấy giá trị hiện tại từ SyncManager
        sliderMusic.value = VolumeSyncManager.Instance.musicVolume;
        sliderSFX.value = VolumeSyncManager.Instance.sfxVolume;

        // Khi slider thay đổi → gọi cập nhật
        sliderMusic.onValueChanged.AddListener((v) =>
        {
            VolumeSyncManager.Instance.SetMusicVolume(v);
            SetMusicVolume(v);
        });

        sliderSFX.onValueChanged.AddListener((v) =>
        {
            VolumeSyncManager.Instance.SetSFXVolume(v);
            SetSFXVolume(v);
        });

        // Khi giá trị bị thay đổi từ nơi khác → cập nhật slider UI
        VolumeSyncManager.Instance.OnMusicVolumeChanged += (v) =>
        {
            sliderMusic.SetValueWithoutNotify(v);
            SetMusicVolume(v);
        };

        VolumeSyncManager.Instance.OnSFXVolumeChanged += (v) =>
        {
            sliderSFX.SetValueWithoutNotify(v);
            SetSFXVolume(v);
        };
    }

    private void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        Music.Instance?.SetVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        SFXController.Ins?.SetVolume(volume);
    }
}
