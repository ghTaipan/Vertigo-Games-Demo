using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ZoneSpriteRenderer : UI_Zone
{
    [SerializeField] private SpriteRenderer _zoneSpriteRenderer;

    protected override void SetSprite(Sprite sprite)
    {
        _zoneSpriteRenderer.sprite = sprite;
    }
}