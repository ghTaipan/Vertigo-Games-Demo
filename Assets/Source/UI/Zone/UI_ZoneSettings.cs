using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Zone
{
    Regular,
    Safe,
    Super
}

[CreateAssetMenu(menuName = "ScritpableObjects/UI/Zone Settings")]
public class UI_ZoneSettings : ScriptableObject
{
    [SerializeField] private Sprite _regularZoneImage;
    [SerializeField] private Sprite _safeZoneImage;
    [SerializeField] private Sprite _superZoneImage;

    public Sprite Regular {
        get { return _regularZoneImage;}
    }

    public Sprite Safe {
        get { return _safeZoneImage;}
    }

    public Sprite Super {
        get { return _superZoneImage;}
    }   
}
