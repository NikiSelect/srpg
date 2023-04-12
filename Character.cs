using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Animator animator;
    [SerializeField] public Sprite figureImage;
    [SerializeField] private Text damageText;
    public Animator MyAnimator => animator;

    // キャラクター初期設定(インスペクタから入力)
    [Header("装備武器")] public Item weaponItem;
    public List<Item> Items;
    [Header("初期X位置")] public int initPos_X;
    [Header("初期Z位置")] public int initPos_Z;
    [Header("最大HP(初期HP)")] public int maxHP;
    [Header("名前")] public string charaName;
    [Header("攻撃")] public int atk;
    [Header("防御")] public int def;
    [Header("必殺")] public int crt;
    [Header("命中")] public int acr;
    [Header("回避")] public int avd;
    [Header("移動範囲")] public int initmoveRange;
    public EnemyPersonality enemyPersonality;
    [Header("敵フラグ")] public bool isEnemy;
    [Header("行動判定")] public bool active;
    [Header("死亡判定")] public bool death;

    // ゲーム中に変化するキャラクターデータ
    [HideInInspector] public int xPos;
    [HideInInspector] public int zPos;
    [HideInInspector] public int nowHP;
    [HideInInspector] public int nowAtk;
    [HideInInspector] public int nowDef;
    [HideInInspector] public int attackRange;
    [HideInInspector] public int moveRange;
    [HideInInspector] public float dirX;
    [HideInInspector] public float dirZ;

    //死亡判定
    public static event EventHandler OnAnyCharacterDead;
    public void Dead()
    {
        OnAnyCharacterDead?.Invoke(this, EventArgs.Empty);
    }

    //アニメーション
    public enum CharacterMotion
    {
        wait,
        idle,
        walk,
        stunby,
        attack,
        damage,
        dead,
        prevent
    }

    //敵キャラ行動パターン
    public enum EnemyPersonality
    {
        approach,
        protect,
        Intercept,
        stay
    }

    //初期設定
    private void Awake()
    {
        zPos = initPos_Z;
        xPos = initPos_X;
        nowHP = maxHP;
        moveRange = initmoveRange;
    }

    //初期設定
    void Start()
    {
        Vector3 pos = new Vector3();
        pos.x = initPos_X;
        pos.y = 1.0f;
        pos.z = initPos_Z;
        transform.position = pos;
        attackRange = weaponItem.range;
    }

    //キャラクター移動処理
    public void MovePosition(MapBlock goalBlock, Character selectChara, Action onMoveEnd)
    {
        List<MapBlock> moveRouteBlocks = mapManager.setMoveRoute(goalBlock, selectChara);
        float dulation = 0.2f;
        for (int i = 0; i < moveRouteBlocks.Count; i++)
        {
            Vector3 movePos = Vector3.zero;
            movePos.x = moveRouteBlocks[i].xPos - xPos;
            movePos.z = moveRouteBlocks[i].zPos - zPos;

            Tweener tweener = transform.DOMove(movePos, dulation).SetEase(Ease.Linear).SetRelative().SetDelay(dulation * i);
            tweener.OnStart(() =>
                {
                    Vector3 temp = movePos;
                    if (Mathf.Abs(temp.x) > 0.1f)
                    {
                        selectChara.MyAnimator.SetFloat("dirX", temp.x);
                    }
                    if (Mathf.Abs(temp.z) > 0.1f)
                    {
                        selectChara.MyAnimator.SetFloat("dirZ", temp.z);
                    }
                });
            if (i == moveRouteBlocks.Count - 1)
            {
                tweener.OnComplete(() => { onMoveEnd.Invoke(); });
            }
            xPos = moveRouteBlocks[i].xPos;
            zPos = moveRouteBlocks[i].zPos;
        }
    }

    //キャラクターの最大攻撃範囲更新
    public void itemAttackRange(Character Chara)
    {
        for (int i = 0; i < Chara.Items.Count; i++)
        {
            if (Chara.attackRange < Chara.Items[i].range)
            {
                Chara.attackRange = Chara.Items[i].range;
            }
        }
    }

    //キャラクターの装備攻撃範囲更新    
    public void updataAttackRange(Character Chara)
    {
        Chara.attackRange = Chara.weaponItem.range;
    }

    //選択キャラクターが向き合う様に変更
    public void characterDirectionChange(Character selectChara, Character opponentChara)
    {
        if (selectChara.xPos > opponentChara.xPos)
        {
            selectChara.MyAnimator.SetFloat("dirX", -1);
            opponentChara.MyAnimator.SetFloat("dirX", 1);

            if (selectChara.zPos > opponentChara.zPos)//左下向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", -1);
                opponentChara.MyAnimator.SetFloat("dirZ", 1);
            }
            else if (selectChara.zPos < opponentChara.zPos)//左上向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", 1);
                opponentChara.MyAnimator.SetFloat("dirZ", -1);
            }
            else if (selectChara.zPos == opponentChara.zPos)//左向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", -1);
                opponentChara.MyAnimator.SetFloat("dirZ", -1);
            }
        }
        else if (selectChara.xPos < opponentChara.xPos)
        {
            selectChara.MyAnimator.SetFloat("dirX", 1);
            animator.SetFloat("dirX", -1);
            if (selectChara.zPos > opponentChara.zPos)//右下向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", -1);
                opponentChara.MyAnimator.SetFloat("dirZ", 1);
            }
            else if (selectChara.zPos < opponentChara.zPos)//右上向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", 1);
                opponentChara.MyAnimator.SetFloat("dirZ", -1);
            }
            else if (selectChara.zPos == opponentChara.zPos)//右向き
            {
                selectChara.MyAnimator.SetFloat("dirZ", -1);
                opponentChara.MyAnimator.SetFloat("dirZ", -1);
            }
        }
        else if (selectChara.xPos == opponentChara.xPos)
        {
            if (selectChara.zPos > opponentChara.zPos)//上から攻撃
            {
                selectChara.MyAnimator.SetFloat("dirX", -1);
                selectChara.MyAnimator.SetFloat("dirZ", -1);
                opponentChara.MyAnimator.SetFloat("dirX", 1);
                opponentChara.MyAnimator.SetFloat("dirZ", 1);
            }
            else if (selectChara.zPos < opponentChara.zPos)//下から攻撃
            {
                selectChara.MyAnimator.SetFloat("dirX", 1);
                selectChara.MyAnimator.SetFloat("dirZ", 1);
                opponentChara.MyAnimator.SetFloat("dirX", -1);
                opponentChara.MyAnimator.SetFloat("dirZ", -1);
            }
        }
    }

    //キャラクターアニメーション切り替え
    public void setCharacterMotion(CharacterMotion motion)
    {
        CharacterMotion setMotion = motion;
        switch (setMotion)
        {
            case CharacterMotion.wait:
                animator.SetBool("idle", false);
                animator.SetBool("walk", false);
                animator.SetBool("stunby", false);
                animator.SetBool("wait", true);
                break;

            case CharacterMotion.idle:
                animator.SetBool("wait", false);
                animator.SetBool("walk", false);
                animator.SetBool("stunby", false);
                animator.SetBool("idle", true);
                break;

            case CharacterMotion.walk:
                animator.SetBool("wait", false);
                animator.SetBool("idle", false);
                animator.SetBool("stunby", false);
                animator.SetBool("walk", true);
                break;

            case CharacterMotion.stunby:
                animator.SetBool("wait", false);
                animator.SetBool("idle", false);
                animator.SetBool("walk", false);
                animator.SetBool("stunby", true);
                break;

            case CharacterMotion.attack:
                animator.SetTrigger("attack");
                break;

            case CharacterMotion.damage:
                animator.SetTrigger("damage");
                break;

            case CharacterMotion.dead:
                animator.SetTrigger("dead");
                break;

            case CharacterMotion.prevent:

                break;
        }
    }
}
