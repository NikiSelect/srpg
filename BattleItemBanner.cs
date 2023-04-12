using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleItemBanner : MonoBehaviour
{
    // UI関係
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image weaponIcon;

    // アイテム・キャラ情報保管
    private Item itemSlotData;
    private Character selectChara;

    // スクリプト連携
    private GameManager gameManager;

    // アイテム選択
    public void itemSelect()
    {
        selectChara.weaponItem = itemSlotData;
        gameManager.weaponSelect();
    }

    // アイテム情報取得
    public void setItemParameter(Character chara, Item itemData, GameManager manager)
    {
        itemName.text = itemData.itemName;
        itemSlotData = itemData;
        selectChara = chara;
        gameManager = manager;
        weaponIcon.sprite = itemData.sprite;
    }
}
