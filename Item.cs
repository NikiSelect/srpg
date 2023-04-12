using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{

    //アイテム情報
    public string itemName; //名前
    public ItemType itemType; //種類
    public int attack; //攻撃力
    public int deffence; //防御力
    public int accuracy; //命中率
    public int weight; //重さ
    public int critical; //必殺率
    public int durableValue; //耐久値
    public int range; //射程距離
    public WeaponType weaponType; //武器種
    public Sprite sprite; //画像

    //列挙型
    public enum ItemType
    {
        none,
        WeaponItem,
        RecoveryItem,
        auxiliaryItem,
    }

    public enum WeaponType
    {
        none,
        Sword,
        Axe,
        Spear,
        Magic,
    }

    public Item(Item item)
    {
        this.name = item.itemName;
        this.itemType = item.itemType;
        this.attack = item.attack;
        this.deffence = item.deffence;
        this.accuracy = item.accuracy;
        this.weight = item.weight;
        this.critical = item.critical;
        this.weaponType = item.weaponType;
        this.range = item.range;
    }

    //アイテム情報取得
    public string GetItemName()
    {
        return itemName;
    }
    public ItemType GetItemType()
    {
        return itemType;
    }
    public int GetAttack()
    {
        return attack;
    }
    public int GetDefffence()
    {
        return deffence;
    }
    public int GetAccracy()
    {
        return accuracy;
    }
    public int GetCritical()
    {
        return critical;
    }
    public int GetWeight()
    {
        return weight;
    }
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    //武器相性
    public string weaponDamageRate(Character attackChara, Character deffenceChara)
    {
        const string noRate = "noRate";
        const string strongRate = "strongRate";
        const string weakRate = "weakRate";

        if (attackChara.weaponItem.weaponType == WeaponType.Sword)
        {
            if (deffenceChara.weaponItem.weaponType == WeaponType.Axe)
            {
                return strongRate;
            }
            else if (deffenceChara.weaponItem.weaponType == WeaponType.Spear)
            {
                return weakRate;
            }
        }
        if (attackChara.weaponItem.weaponType == WeaponType.Spear)
        {
            if (deffenceChara.weaponItem.weaponType == WeaponType.Sword)
            {
                return strongRate;
            }
            else if (deffenceChara.weaponItem.weaponType == WeaponType.Axe)
            {
                return weakRate;
            }
        }
        if (attackChara.weaponItem.weaponType == WeaponType.Axe)
        {
            if (deffenceChara.weaponItem.weaponType == WeaponType.Spear)
            {
                return strongRate;
            }
            else if (deffenceChara.weaponItem.weaponType == WeaponType.Sword)
            {
                return weakRate;
            }
        }
        return noRate;
    }
}
