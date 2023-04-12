using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatusUI : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private BattleManager battleManager;

    //UI情報
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtAttack;
    [SerializeField] private TextMeshProUGUI txtDeffence;
    [SerializeField] private TextMeshProUGUI txtHp;
    [SerializeField] private TextMeshProUGUI txtWeaponName;
    [SerializeField] private Image HpGauge;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image nameBackGround;
    [SerializeField] private Sprite playerNameBackGround;
    [SerializeField] private Sprite enemyNameBackGround;

    //アニメーション
    private Animator myAnimator;
    public Animator MyAnimator => myAnimator ? myAnimator : (myAnimator = GetComponent<Animator>());

    //UI情報更新
    public void updateStatusWindow(Character character)
    {
        int atk = battleManager.getAttackValue(character);
        int def = battleManager.getDffenceValue(character);
        txtName.text = character.charaName;
        txtAttack.text = atk.ToString();
        txtDeffence.text = def.ToString();
        txtHp.text = $"{character.nowHP} / {character.maxHP}";
        float normalizedHp = (float)character.nowHP / character.maxHP;
        HpGauge.fillAmount = normalizedHp;
        weaponIcon.sprite = character.weaponItem.sprite;
        characterImage.sprite = character.figureImage;

        if (character.isEnemy == false)
        {
            nameBackGround.sprite = playerNameBackGround;
        }
        else
        {
            nameBackGround.sprite = enemyNameBackGround;
        }

        if (character.weaponItem == null)
        {
            txtWeaponName.text = "素手";
        }
        else
        {
            txtWeaponName.text = character.weaponItem.itemName;
        }
    }
}
