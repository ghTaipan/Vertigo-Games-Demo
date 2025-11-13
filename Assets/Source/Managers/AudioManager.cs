using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_UISound
{
    ButtonHover,
    ButtonClick
}

[System.Serializable]
public class MusicProps
{
    [SerializeField] private AudioSource _musicSource;
    // Each index corresponds to a scene (for some reason dictionary is not serializable)
    [SerializeField] private SceneMusicData[] _sceneMusic;
    [SerializeField] private  float _fadeOutDecrement = 0.05f;

    private int currentMusicIndex = 0;
    private AudioSource currentMusicSource;

    public AudioSource MusicSource {
        get { return _musicSource; }
    }

    public int CurrentMusicIndex
    {
        set { currentMusicIndex = value; }
        get { return currentMusicIndex; }
    }

    public AudioSource CurrentMusicSource
    {
        set { currentMusicSource = value; }
        get { return currentMusicSource; }
    }

    public SceneMusicData this[E_Scene scene]
    {
        get { return _sceneMusic[(int)scene]; }
    }

    public SoundClip this[E_Scene scene, int index]
    {
        get { return _sceneMusic[(int)scene].MusicForScene[index]; }
    }

    public float FadeOutDecrement {
        get { return _fadeOutDecrement; }
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
 
    [SerializeField] private AudioSource _sfxPrefab;
    [SerializeField] private MusicProps _musicProps;
    // Each index corresponds to an E_UISound Element
    [SerializeField] private SoundClip[] _uiDefaultSFX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneLoader.Instance.onSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        if (SceneLoader.Instance)
        {
            SceneLoader.Instance.onSceneChanged -= OnSceneChanged;
        }
    }

    // It appears that unity doesnt provide any virtual member function or callbacks after an audio is finished?
    // Couldnt use timer in case audio is being paused and unpaused.
    private void Update()
    {
        if (_musicProps.CurrentMusicSource && _musicProps.CurrentMusicSource.clip && _musicProps.CurrentMusicSource.time >= _musicProps.CurrentMusicSource.clip.length)
        {
            PlayNextMusic();
        }
    }

    private void OnSceneChanged(E_Scene newScene)
    {
        StartCoroutine(ChangeMusic(newScene));
    }

    IEnumerator ChangeMusic(E_Scene newScene)
    {
        if (_musicProps.CurrentMusicSource)
        {
            yield return StartCoroutine(FadeOutMusic());
            Destroy(_musicProps.CurrentMusicSource.gameObject);
        }

        _musicProps.CurrentMusicSource = Instantiate(_musicProps.MusicSource, default, Quaternion.identity);
        _musicProps.CurrentMusicIndex = 0;
        SoundClip soundClip = _musicProps[newScene, 0];
        _musicProps.CurrentMusicSource.clip = soundClip.AudioClip;
        _musicProps.CurrentMusicSource.volume = soundClip.Volume;
        _musicProps.CurrentMusicSource.Play();
    }

    IEnumerator FadeOutMusic()
    {
        while (_musicProps.CurrentMusicSource.volume > 0.01f)
        {
            _musicProps.CurrentMusicSource.volume -= _musicProps.FadeOutDecrement;
            yield return null; 
        }
    }

    private void PlayNextMusic()
    {
        _musicProps.CurrentMusicIndex = ++_musicProps.CurrentMusicIndex >= _musicProps[SceneLoader.Instance.CurrentScene].MusicForScene.Length ? 0 : _musicProps.CurrentMusicIndex;
        SoundClip soundClip = _musicProps[SceneLoader.Instance.CurrentScene, _musicProps.CurrentMusicIndex];
        _musicProps.CurrentMusicSource.clip = soundClip.AudioClip;
        _musicProps.CurrentMusicSource.volume = soundClip.Volume;
        _musicProps.CurrentMusicSource.Play();       
    }

    public void AdjustMusic(bool bPlay)
    {
        if (_musicProps.CurrentMusicSource)
        {
            if (bPlay)
            {
                _musicProps.CurrentMusicSource.UnPause();
            }
            else
            {
                _musicProps.CurrentMusicSource.Pause();
            }
        }
    }

    // Plays sound, returns length
    public float PlayAudioClip(AudioClip audioClip, float volume = 1.0f, float playSpeed = 1.0f, Vector3 location = default, Quaternion rotation = default)
    {
        AudioSource audioSource = Instantiate(_sfxPrefab, location, rotation);
        audioSource.clip = audioClip;
        audioSource.pitch = playSpeed;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length / playSpeed;
        Destroy(audioSource.gameObject, clipLength);

        return clipLength;
    }

    public float PlaySoundClip(SoundClip soundClip, float playSpeed = 1.0f, Vector3 location = default, Quaternion rotation = default)
    {
        return soundClip ? PlayAudioClip(soundClip.AudioClip, soundClip.Volume, playSpeed, location, rotation) : 0.0f;
    }

    public void PlayUISound(E_UISound uiSound, SoundClip soundClip)
    {
        SoundClip sc = soundClip && soundClip.AudioClip ? soundClip : _uiDefaultSFX[(int) uiSound];
        PlaySoundClip(sc);
    }
}
