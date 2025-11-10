using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScritpableObjects/Data/Scene Music")]

public class SceneMusicData : ScriptableObject
{
    [SerializeField] private SoundClip[] _musicForScene;

    public SoundClip[] MusicForScene {
        get {return _musicForScene; }
    }
}
