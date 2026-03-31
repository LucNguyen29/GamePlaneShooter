using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static SFXController Ins { get; private set; }
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip popSound;
    public float volume = 1f;
    void Awake()
    {
        Debug.Log("SFXController Awake");
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Debug.Log("SFXController Awake - Instance set.");
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        if (sfxSource != null)
        {
            sfxSource.Stop();  // Tắt tất cả các âm thanh đang phát
        }
    }

    void Start()
    {
        SetVolume(VolumeSyncManager.Instance != null ? VolumeSyncManager.Instance.sfxVolume : 1f);
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol); // Đảm bảo nằm trong khoảng 0 - 1
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }

        Debug.Log("SFX volume set to: " + volume);
    }

    public float GetVolume()
    {
        return volume;
    }

    public AudioSource SFXSource
    {
        get { return sfxSource; }
    }
}
