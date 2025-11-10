using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;
using static UnityEditor.FilePathAttribute;

[System.Serializable]
public class MusicProps
{
    [SerializeField] private AudioSource _musicSource;
    // Each index corrusponds to a scene (for some reason dictionary is not serializable)
    [SerializeField] private SceneMusicData[] _sceneMusic;
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
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
 
    [SerializeField] private AudioSource _sfxPrefab;
    [SerializeField] private MusicProps _musicProps;

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
        SceneLoader.onSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneLoader.onSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(E_Scene newScene)
    {
        if (_musicProps.CurrentMusicSource)
        {
            Destroy(_musicProps.CurrentMusicSource.gameObject);
        }
        _musicProps.CurrentMusicSource = Instantiate(_musicProps.MusicSource, default, Quaternion.identity);
        _musicProps.CurrentMusicIndex = 0;
        SoundClip soundClip = _musicProps[newScene, 0];
        _musicProps.CurrentMusicSource.clip = soundClip.AudioClip;
        _musicProps.CurrentMusicSource.volume = soundClip.Volume;
        _musicProps.CurrentMusicSource.Play();
        StartCoroutine(WaitForAudioEnd(_musicProps.CurrentMusicSource, newScene, 0));
    }

    private IEnumerator WaitForAudioEnd(AudioSource source, E_Scene currentScene, int currentIndex)
    {
        yield return new WaitForSeconds(source.clip.length);
        if (source == _musicProps.CurrentMusicSource && currentIndex == _musicProps.CurrentMusicIndex)
        {
            _musicProps.CurrentMusicIndex = ++_musicProps.CurrentMusicIndex >= _musicProps[currentScene].MusicForScene.Length ? 0 : _musicProps.CurrentMusicIndex;
            SoundClip soundClip = _musicProps[currentScene, _musicProps.CurrentMusicIndex];
            source.clip = soundClip.AudioClip;
            source.volume = soundClip.Volume;
            source.Play();
            StartCoroutine(WaitForAudioEnd(source, currentScene, _musicProps.CurrentMusicIndex));
        }
    }

    // Plays sound, returns length
    public float PlaySound(AudioClip audioClip, float playSpeed = 1.0f, float volume = 1.0f, Vector3 location = default, Quaternion rotation = default)
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
}
