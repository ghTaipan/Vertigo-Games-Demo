using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Image _inventorySlots;
    [SerializeField] private int _slotCount = 18;

    public void Start()
    {
        KeyValuePair<string, ItemData>[] pairArray = Inventory.Instance.ItemMap.ToArray();
        for (int i = 0; i < _slotCount; ++i)
        {
            GameObject childObject = Instantiate(_inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            childObject.transform.SetParent(_inventorySlots.transform);

            if (i < pairArray.Length)
            {
                UI_InventorySlot inventorySlot = childObject.GetComponent<UI_InventorySlot>();
                inventorySlot.Item = pairArray[i].Value;
            }
        }
    }
}
