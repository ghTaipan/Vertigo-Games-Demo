using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCount;

    private ItemData item;

    void Start()
    {
        Inventory.Instance.onItemCountChanged += OnItemCoundChanged;
    }

    private void OnDestroy()
    {
        Inventory.Instance.onItemCountChanged -= OnItemCoundChanged;
    }

    void OnItemCoundChanged(ItemData itemData)
    {
        _itemCount.text = itemData._itemCount.ToString();
    }

    public ItemData Item
    {
        set 
        { 
            item = value;
            _itemIcon.sprite = item._itemIcon;

            Color color = _itemIcon.color;
            color.a = _itemIcon.sprite ? 255.0f : 0.0f;
            _itemIcon.color = color;

            _itemName.text = item._itemName;
            _itemCount.text = item._itemCount.ToString();
        }
    }
}
