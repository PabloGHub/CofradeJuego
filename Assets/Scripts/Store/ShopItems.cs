using System;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Nazareno, PowerUp
}

[Serializable]
public struct ItemInfo
{
    public string Name;
    public string Description;
    public ItemType Type;
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
public class ShopData : ScriptableObject
{
    [SerializeField]
    private List<ItemEntry> itemList = new List<ItemEntry>();


    private Dictionary<string, ItemInfo> m_Items;
    public Dictionary<string, ItemInfo> Items => m_Items;


    private void OnEnable()
    {
        m_Items = new Dictionary<string, ItemInfo>();

        foreach (ItemEntry entry in itemList)
        {
            if (!m_Items.ContainsKey(entry.key))
            {
                m_Items.Add(entry.key, entry.value);
            }
        }
    }
}
