using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public string _itemName;
    public Sprite _itemIcon;
    public int _itemCount;
    public bool _bShowText;

    public ItemData(string name, Sprite icon = default, int count = 0, bool inShowText = true)
    {
        _itemName = name;
        _itemIcon = icon;
        _itemCount = count;
        _bShowText = inShowText;
    }
}

[CreateAssetMenu(menuName = "ScritpableObjects/Data/Item Data Table")]
public class ItemDataTable : ScriptableObject
{
    [SerializeField] private ItemData[] _regularItems;
    [SerializeField] private ItemData[] _specialItems;

    [SerializeField] private float _regularItemCountLevelMult = 1.0f;
    [SerializeField] private float _regularItemRandomRange = 5.0f;
    [SerializeField] private float _specialItemCountByLevelMult = 0.1f;
    [SerializeField] private float _specialItemRandomRange = 2.0f;

    public ItemData[] RegularItems
    {
        get { return _regularItems; }
    }

    public ItemData[] SpecialItems
    {
        get { return _specialItems; }
    }

    public float RegularItemCountLevelMult
    {
        get { return _regularItemCountLevelMult; }
    }

    public float RegularItemRandomRange
    {
        get { return _regularItemRandomRange; }
    }

    public float SpecialItemCountByLevelMult
    {
        get { return _specialItemCountByLevelMult; }
    }

    public float SpecialItemRandomRange
    {
        get { return _specialItemRandomRange; }
    }
}
