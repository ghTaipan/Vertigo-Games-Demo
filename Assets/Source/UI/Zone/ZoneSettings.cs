using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScritpableObjects/UI/Zone Settings")]

public class ZoneSettings : ScriptableObject
{
    [SerializeField] private Sprite _bronzeImage;
    [SerializeField] private Sprite _silverImage;
    [SerializeField] private Sprite _goldImage;

    public Sprite Bronze {
        get { return _bronzeImage;}
    }

    public Sprite Silver {
        get { return _silverImage;}
    }

    public Sprite Gold {
        get { return _goldImage;}
    }   
}
