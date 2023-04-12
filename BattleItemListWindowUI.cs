using UnityEngine;

public class BattleItemListWindowUI : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private BattleItemBanner battleItemBanner;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private GameManager gameManager;

    //アイテムボタン
    [SerializeField] private Transform rootItemList;
    [SerializeField] private GameObject itemSlotPrefab;

    //アニメーション
    private Animator myAnimator;
    public Animator MyAnimator => myAnimator ? myAnimator : (myAnimator = GetComponent<Animator>());

    //アイテムスロット生成
    public void setItemSlot(Character character)
    {
        MyAnimator.SetBool("isShow", true);
        clearItemSlots();
        foreach (var item in character.Items)
        {
            BattleItemBanner banner = Instantiate(itemSlotPrefab, rootItemList).GetComponent<BattleItemBanner>();
            banner.setItemParameter(character, item, gameManager);
        }
    }

    //アイテムスロットの削除
    public void clearItemSlots()
    {
        foreach (Transform itemSlot in rootItemList)
        {
            Destroy(itemSlot.gameObject);
        }
    }
}
