using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleWindowUI : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private BattleManager battleManager;

    [Header("Player Character UI")]
    [SerializeField] private TextMeshProUGUI txtPlayerName;
    [SerializeField] private TextMeshProUGUI txtPlayerAttack;
    [SerializeField] private TextMeshProUGUI txtPlayerCritical;
    [SerializeField] private TextMeshProUGUI txtPlayerHp;
    [SerializeField] private TextMeshProUGUI txtPlayerAccuracy;
    [SerializeField] private TextMeshProUGUI txtPlayerWeaponName;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerHpGauge;

    [Header("Enemy Character UI")]
    [SerializeField] private TextMeshProUGUI txtEnemyName;
    [SerializeField] private TextMeshProUGUI txtEnemyAttack;
    [SerializeField] private TextMeshProUGUI txtEnemyCritical;
    [SerializeField] private TextMeshProUGUI txtEnemyHp;
    [SerializeField] private TextMeshProUGUI txtEnemyAccuracy;
    [SerializeField] private TextMeshProUGUI txtEnemyWeaponName;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Image enemyHpGauge;

    //アニメーション
    private Animator myAnimator;
    public Animator MyAnimator => myAnimator ? myAnimator : (myAnimator = GetComponent<Animator>());

    //UI表示更新
    public void updateBattleWindow(Character playerChara, Character enemyChara)
    {
        int playerDamageValue = battleManager.getDamageValue(playerChara, enemyChara);
        int enemyDamageValue = battleManager.getDamageValue(enemyChara, playerChara);
        int playerCriticalValue = battleManager.getCriticalValue(playerChara, enemyChara);
        int enemyCriticalValue = battleManager.getCriticalValue(enemyChara, playerChara);
        int playerAccuracyValue = battleManager.getAccuracyValue(playerChara, enemyChara);
        int enemyAccuracyValue = battleManager.getAccuracyValue(enemyChara, playerChara);

        txtPlayerName.text = playerChara.charaName;
        txtPlayerAttack.text = playerDamageValue.ToString();
        txtPlayerCritical.text = playerCriticalValue.ToString();
        txtPlayerHp.text = $"{playerChara.nowHP} / {playerChara.maxHP}";
        txtPlayerAccuracy.text = playerAccuracyValue.ToString();
        txtPlayerWeaponName.text = playerChara.weaponItem.itemName;
        playerImage.sprite = playerChara.figureImage;

        float playerHpfillAmout = Mathf.Clamp((float)playerChara.nowHP / (float)playerChara.maxHP, 0f, 1f);
        playerHpGauge.fillAmount = playerHpfillAmout;

        txtEnemyName.text = enemyChara.charaName;
        txtEnemyAttack.text = enemyDamageValue.ToString();
        txtEnemyCritical.text = enemyCriticalValue.ToString();
        txtEnemyHp.text = $"{enemyChara.nowHP} / {enemyChara.maxHP}";
        txtEnemyAccuracy.text = enemyAccuracyValue.ToString();
        txtEnemyWeaponName.text = enemyChara.weaponItem.itemName;
        enemyImage.sprite = enemyChara.figureImage;

        float enemyHpfillAmout = Mathf.Clamp((float)enemyChara.nowHP / (float)enemyChara.maxHP, 0f, 1f);
        enemyHpGauge.fillAmount = enemyHpfillAmout;
    }

    //Hp表示変更
    public void updateHpText(Character character)
    {
        if (character.isEnemy == false)
        {
            txtPlayerHp.text = $"{character.nowHP} / {character.maxHP}";
        }
        else
        {
            txtEnemyHp.text = $"{character.nowHP} / {character.maxHP}";
        }
    }
}
