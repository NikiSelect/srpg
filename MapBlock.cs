using UnityEngine;
public class MapBlock : MonoBehaviour
{
    //各種ブロック
    [SerializeField] public GameObject selecting;
    [SerializeField] public GameObject moving;
    [SerializeField] public GameObject attacking;

    //ブロック内情報
    [HideInInspector] public int xPos;
    [HideInInspector] public int zPos;
    [SerializeField] public bool moveRange;
    [SerializeField] public bool attackRange;
    [SerializeField] public bool chooseableBlock;
    [SerializeField] public int blockCost;
    [SerializeField] public bool blockCheck;
    [SerializeField] public int range;
    [SerializeField] public int blockNumber;

    //選択ブロック白
    public void SetSelectionMode(bool selectFlag)
    {
        selecting.SetActive(selectFlag);
    }

    //移動範囲ブロック青
    public void SetMoveRangeMode(bool moveFlag)
    {
        moving.SetActive(moveFlag);
        moveRange = moveFlag;
    }

    //攻撃範囲ブロック赤
    public void SetAttackRangeMode(bool attackFlag)
    {
        attacking.SetActive(attackFlag);
        attackRange = attackFlag;
    }
}
