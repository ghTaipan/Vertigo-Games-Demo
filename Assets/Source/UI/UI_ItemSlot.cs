using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    private ItemData item;

    public ItemData Item {
        get { return item; }
        set 
        { 
            item = value;
            _image.sprite = item._itemIcon;
            _text.name = item._itemCount.ToString();
            _text.enabled = item._bShowText;
            _text.text = item._itemCount.ToString();
        }
    }
}
