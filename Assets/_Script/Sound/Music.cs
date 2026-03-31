using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundClip;
    [SerializeField] private AudioClip gameClip;
    public static Music Instance;
    private AudioSource audioSource;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PlayBGM()
    {

        PlayClip(backgroundClip);
    }

    private void Start()
    {
        SetVolume(VolumeSyncManager.Instance != null ? VolumeSyncManager.Instance.musicVolume : 1f);
        PlayBGM();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu"))
        {
            PlayClip(backgroundClip); 

        }
        else if (scene.name.Contains("Map"))
        {
            PlayClip(gameClip);
        }
    }

    public void Silence()
    {
        if (audioSource.clip == gameClip && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }


    public void PlayClip(AudioClip clip)
    {
        if (clip == null || (audioSource.clip == clip && audioSource.isPlaying))
            return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}
