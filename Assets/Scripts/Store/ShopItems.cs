using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct ItemInfo
{
    public string Name;
    public string Description;
    public float Price;

    public Sprite sprite;
    public GameObject dropObject;
}

[Serializable]
public class ItemEntry
{
    public string key;
    public ItemInfo value;

    public ItemEntry(string key, ItemInfo value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
[CreateAssetMenu(fileName = "ShopData", menuName = "Game/ShopData")]
public class ShopItems : ScriptableObject // ShopData
{
    [SerializeField]
    private List<ItemEntry> itemList = new List<ItemEntry>();


    private Dictionary<string, ItemInfo> _items;
    public Dictionary<string, ItemInfo> Items 
    {
        get
        { 
            if (_items == null)
            {
                foreach (ItemEntry individual_ItemEntry in itemList)
                {
                    if (!_items.ContainsKey(individual_ItemEntry.key))
                        _items.Add(individual_ItemEntry.key, individual_ItemEntry.value);
                }
            }
            return _items;
        }
        set => _items = value;
    }


    private void OnEnable()
    {
        _items = new Dictionary<string, ItemInfo>();

        foreach (ItemEntry entry in itemList)
        {
            if (!_items.ContainsKey(entry.key))
            {
                _items.Add(entry.key, entry.value);
            }
        }
    }
}
