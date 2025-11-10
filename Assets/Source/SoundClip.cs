using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScritpableObjects/Audio")]
public class SoundClip : ScriptableObject
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private float _volume = 1.0f;

    public AudioClip AudioClip {
        get { return _audioClip; }
    }
    public float Volume
    {
        get { return _volume; }
    }
}
