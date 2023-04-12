using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GUIManager : MonoBehaviour
{
    //スクリプト連携
    public CharacterStatusUI statusWindow;
    public BattlePredictWindowUI battlepredictWindow;
    public BattleWindowUI battleWindowUI;
    public BattleItemListWindowUI battleItemListWindowUI;
    public WindowUICommand windowUICommand;
    public GameManager gameManager;

    //オブジェクト参照
    public GameObject CommandButtons;
    public GameObject StatusWindow;
    public GameObject BattleWindow;
    public GameObject BattlePredictWindow;
    public GameObject ItemList;

    //シーン切り替えロゴ
    public Image logoImage;
    public Animator logoAnimator;

    //ステータスウィンドウ表示切替
    public void ShowStatusWindow(Character charaData)
    {
        statusWindow.MyAnimator.SetBool("isShow", true);
        statusWindow.updateStatusWindow(charaData);
    }
    public void HideStatusWindow()
    {
        statusWindow.MyAnimator.SetBool("isShow", false);
    }

    //コマンドボタン表示切替
    public void ShowCommandButtons()
    {
        windowUICommand.MyAnimator.SetBool("isShow", true);
    }
    public void HideCommandButtons()
    {
        windowUICommand.MyAnimator.SetBool("isShow", false);
    }

    //バトルウィンドウ表示切替
    public void ShowBattleWindow(Character attackChara, Character deffenceChara)
    {
        battleWindowUI.MyAnimator.SetBool("isShow", true);
        battleWindowUI.updateBattleWindow(attackChara, deffenceChara);
    }
    public void HideBattleWindow()
    {
        battleWindowUI.MyAnimator.SetBool("isShow", false);
    }

    //バトル予測表示切替
    public void ShowBattlePredictWindow(Character playerChara, Character enemyChara)
    {
        battlepredictWindow.MyAnimator.SetBool("isShow", true);
        battlepredictWindow.UpdateBattlePredictWindow(playerChara, enemyChara);
    }
    public void HideBattlePredictWindow()
    {
        battlepredictWindow.MyAnimator.SetBool("isShow", false);
    }

    //アイテム欄表示
    public void ShowItemList(Character playerChara)
    {
        battleItemListWindowUI.MyAnimator.SetBool("isShow", true);
        battleItemListWindowUI.setItemSlot(playerChara);
    }
    public void HideItemList()
    {
        battleItemListWindowUI.MyAnimator.SetBool("isShow", false);
    }

    //ロゴ画像の表示
    public void selectLogo(string logoName, Action onEnd)
    {
        switch (logoName)
        {
            case "playerTurn":
                DOVirtual.DelayedCall(0.25f, () => logoAnimator.SetTrigger("PlayerTurn"), false);
                DOVirtual.DelayedCall(2.5f, () => onEnd.Invoke(), false);
                break;
            case "enemyTurn":
                DOVirtual.DelayedCall(0.25f, () => logoAnimator.SetTrigger("EnemyTurn"), false);
                DOVirtual.DelayedCall(2.5f, () => onEnd.Invoke(), false);
                break;
            case "GameClear":
                DOVirtual.DelayedCall(0.25f, () => logoAnimator.SetTrigger("GameClear"), false);
                DOVirtual.DelayedCall(2.5f, () => onEnd.Invoke(), false);
                break;
            case "GameOver":
                DOVirtual.DelayedCall(0.25f, () => logoAnimator.SetTrigger("GameOver"), false);
                DOVirtual.DelayedCall(2.5f, () => onEnd.Invoke(), false);
                break;
        }
    }
}
