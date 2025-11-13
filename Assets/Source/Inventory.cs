using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void ItemCountChangedDelegate(ItemData itemData);

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemCountChangedDelegate onItemCountChanged;

    private Dictionary<string, ItemData> itemMap = new ();

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
        SaveData data = SaveSystem.LoadGame();
        bool bValidData = data != null && data.bSafe;

        ItemData[] regularItems = GameManager.Instance.ItemDataTable.RegularItems;
        for (int i = 1; i < regularItems.Length; ++i)
        {
            ItemData temp = regularItems[i];
            temp._itemCount = bValidData?  data.itemMap[temp._itemName] : 0;       

            ChangeItemCount(temp, temp._itemCount, false);
        }

        foreach (ItemData itemData in GameManager.Instance.ItemDataTable.SpecialItems)
        {
            ItemData temp = itemData;
            temp._itemCount = bValidData ? data.itemMap[temp._itemName] : 0;

            ChangeItemCount(temp, temp._itemCount, false);
        }
    }

    void ChangeItemCount(ItemData itemData, int count, bool bSave = true)
    {
        ItemData temp = itemData;
        temp._itemCount = count;

        if (!itemMap.TryAdd(temp._itemName, temp))
        {
            ItemMap[temp._itemName] = temp;
        }

        if (bSave)
        { 
            GameManager.Instance.SaveGame();
        }

        onItemCountChanged?.Invoke(ItemMap[temp._itemName]);
    }

    public void ResetInventory()
    {
        KeyValuePair<string, ItemData> [] pairArray = itemMap.ToArray();
        foreach (var pair in pairArray)
        {
            ChangeItemCount(pair.Value, 0, false);
        }

        GameManager.Instance.SaveGame();
    }

    public void IncrementItem(ItemData itemData)
    {
        if (itemMap.TryGetValue(itemData._itemName, out ItemData item))
        {
            item._itemCount += itemData._itemCount;
            ItemMap[item._itemName] = item;

            onItemCountChanged?.Invoke(ItemMap[item._itemName]);
        }

        GameManager.Instance.SaveGame();
    }

    public Dictionary<string, ItemData> ItemMap {
        get { return itemMap; }
    }
}
