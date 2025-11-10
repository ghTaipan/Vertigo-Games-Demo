using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void ItemCountChangedDelegate(ItemData itemData);

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemCountChangedDelegate onItemCountChanged;

    private Dictionary<string, ItemData> itemMap = new Dictionary<string, ItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        ItemData[] regularItems = GameManager.Instance.ItemDataTable.RegularItems;
        for (int i = 1; i < regularItems.Length; ++i)
        {
            SetDefaultItem(regularItems[i]);
        }

        foreach (ItemData itemData in GameManager.Instance.ItemDataTable.SpecialItems)
        {
            SetDefaultItem(itemData);
        }
    }
    
    void SetDefaultItem(ItemData itemData)
    {
        ItemData temp = itemData;
        temp._itemCount = 0;

        if (itemMap.TryAdd(temp._itemName, temp))
        {
            //onItemCoundChanged.Invoke(temp);
        }
    }

    public void ResetInventory()
    {
        KeyValuePair<string, ItemData> [] pairArray = itemMap.ToArray();
        foreach (var pair in pairArray)
        {
            SetDefaultItem(pair.Value);
        }
    }

    public void IncrementItem(ItemData itemData)
    {
        if (itemMap.TryGetValue(itemData._itemName, out ItemData item))
        {
            item._itemCount += itemData._itemCount;
            onItemCountChanged?.Invoke(item);
        }
    }

    public Dictionary<string, ItemData> ItemMap {
        get { return itemMap; }
    }
}
