using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private ItemDataBase itemDataBase;

    private Dictionary<Item, int> numOfItem = new Dictionary<Item, int>();

    //アイテムに番号割振り
    private void Start()
    {
        for (int i = 0; i < itemDataBase.GetItemLists().Count; i++)
        {
            numOfItem.Add(itemDataBase.GetItemLists()[i], i);
            //Debug.Log(itemDataBase.GetItemLists()[i].GetItemName() + ": " + itemDataBase.GetItemLists()[i].GetWeaponType());
        }
        //Debug.Log(GetItem("鉄の剣").GetWeaponType());
        //Debug.Log(numOfItem[GetItem("鋼の槍")]);
    }

    //アイテム名から必要情報取得
    public Item GetItem(string searchName)
    {
        return itemDataBase.GetItemLists().Find(itemName => itemName.GetItemName() == searchName);
    }

    //アイテム番号取得
    public int GetitemNumber(string searchName)
    {
        return numOfItem[GetItem(searchName)];
    }
}
