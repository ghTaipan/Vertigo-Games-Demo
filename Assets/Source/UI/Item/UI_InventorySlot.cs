using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_InventorySlot : UI_ItemSlot
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCount;

    void Start()
    {
        Inventory.Instance.onItemCountChanged += OnItemCountChanged;
    }

    void OnDestroy()
    {
        if (Inventory.Instance)
        { 
            Inventory.Instance.onItemCountChanged += OnItemCountChanged; 
        }
    }

    void OnItemCountChanged(ItemData itemData)
    {
        if (item._itemName != "" && itemData._itemName == item._itemName)
        {
            Item = itemData;
        }
    }

    public override ItemData Item
    {
        get => base.Item;
        set
        {
            base.Item = value;

            _itemIcon.sprite = item._itemIcon;

            Color color = _itemIcon.color;
            color.a = _itemIcon.sprite != null ? 255.0f : 0.0f;
            _itemIcon.color = color;

            _itemName.text = item._itemName.ToString();

            _itemCount.text = item._itemCount.ToString();
        }
    }
}