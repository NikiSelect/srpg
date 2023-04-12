using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "CreateItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField] private List<Item> itemLists = new List<Item>();

    public List<Item> GetItemLists()
    {
        return itemLists;
    }

    public Item GetItemData(string itemName)
    {
        foreach (var item in itemLists)
        {
            if (item.GetItemName() == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
