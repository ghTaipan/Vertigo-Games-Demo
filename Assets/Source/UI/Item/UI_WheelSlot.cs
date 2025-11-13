using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WheelSlot : UI_ItemSlot
{
    [SerializeField] private SpriteRenderer _itemIcon;
    [SerializeField] private TextMeshPro _text;

    public override ItemData Item
    {
        get => base.Item;
        set
        {
            base.Item = value;

            _itemIcon.sprite = item._itemIcon;
            _text.name = item._itemCount.ToString();
            _text.enabled = item._bShowText;
            _text.text = item._itemCount.ToString();
        }
    }
}
