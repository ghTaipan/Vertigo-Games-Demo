using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ZoneImage : UI_Zone
{
    [SerializeField] private Image _zoneImage;

    protected override void SetSprite(Sprite sprite)
    {
        _zoneImage.sprite = sprite;
    }
}