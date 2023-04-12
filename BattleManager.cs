using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private Item item;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BattleWindowUI battleWindowUI;
    [SerializeField] private CharactersManager charactersManager;

    //Hpゲージ
    [SerializeField] private Image playerCharacterHpGageImage;
    [SerializeField] private Image enemyCharacterHpGageImage;

    //命中値取得
    public int getAccuracyValue(Character atkChara, Character defChara)
    {
        string damageRate = item.weaponDamageRate(atkChara, defChara);
        switch (damageRate)
        {
            case "noRate":
                {
                    int acr = atkChara.acr + atkChara.weaponItem.accuracy - defChara.avd;
                    acr = Mathf.Clamp(acr, 0, 999);
                    return acr;
                }
            case "strongRate":
                {
                    int acr = atkChara.acr + atkChara.weaponItem.accuracy - defChara.avd + 10;
                    acr = Mathf.Clamp(acr, 0, 999);
                    return acr;
                }
            case "weakRate":
                {
                    int acr = atkChara.acr + atkChara.weaponItem.accuracy - defChara.avd - 10;
                    acr = Mathf.Clamp(acr, 0, 999);
                    return acr;
                }
        }
        return 0;
    }

    //必殺値取得
    public int getCriticalValue(Character atkChara, Character defChara)
    {
        int crt = atkChara.crt + atkChara.weaponItem.critical;
        crt = Mathf.Clamp(crt, 0, 999);
        return crt;
    }

    //攻撃値取得
    public int getAttackValue(Character atkChara)
    {
        int atk = atkChara.atk + atkChara.weaponItem.attack;
        return atk;
    }

    //防御値取得
    public int getDffenceValue(Character defChara)
    {
        int def = (int)(defChara.def + defChara.weaponItem.deffence);
        return def;
    }

    //与ダメージ取得
    public int getDamageValue(Character atkChara, Character defChara)
    {
        int atttackDamage = getAttackValue(atkChara);
        string damageRate = item.weaponDamageRate(atkChara, defChara);
        switch (damageRate)
        {
            case "noRate":
                {
                    break;
                }
            case "strongRate":
                {
                    atttackDamage = atttackDamage + 3;
                    break;
                }
            case "weakRate":
                {
                    atttackDamage = atttackDamage - 3;
                    break;
                }
        }
        int deffenceValue = getDffenceValue(defChara);
        int damageValue = atttackDamage - deffenceValue;
        damageValue = Mathf.Clamp(damageValue, 0, atttackDamage);
        return damageValue;
    }

    //被ダメ後Hp値取得
    public int getNowHpValue(Character defChara, int attackDamage)
    {
        int nowHP = Mathf.Clamp(defChara.nowHP - attackDamage, 0, defChara.maxHP);
        return nowHP;
    }

    //バトル演出・処理
    public void battleCalculation(Character atkChara, Character defChara, Action onAttackEnd)
    {
        int accuracy = Random.Range(0, 100);
        int critical = Random.Range(0, 100);

        int atkCharaHitAttack = getAccuracyValue(atkChara, defChara);
        int atkCharaHitCritical = getCriticalValue(atkChara, defChara);
        int atkCharaAttackDamage = getDamageValue(atkChara, defChara);
        if (atkCharaAttackDamage != 0 && critical <= atkCharaHitCritical && critical != 0)
        {
            atkCharaAttackDamage = atkCharaAttackDamage * 3;
        }
        int defCharaMaxHp = defChara.maxHP;
        int defCharaNowHp = getNowHpValue(defChara, atkCharaAttackDamage);
        float defCharaAmount = (float)defChara.nowHP / defChara.maxHP;
        float defCharaEndAmount = (float)defCharaNowHp / defChara.maxHP;

        int defCharaHitAttack = getAccuracyValue(defChara, atkChara);
        int defCharaCriticalHit = getCriticalValue(defChara, atkChara);
        int defCharaAttackDamage = getDamageValue(defChara, atkChara);
        if (defCharaAttackDamage != 0 && critical <= defCharaCriticalHit && critical != 0)
        {
            defCharaAttackDamage = defCharaAttackDamage * 3;
        }
        int atkCharaMaxHp = atkChara.maxHP;
        int atkCharaNowHp = getNowHpValue(atkChara, defCharaAttackDamage);
        float atkCharaAmount = (float)atkChara.nowHP / atkChara.maxHP;
        float atkCharaEndAmount = (float)atkCharaNowHp / atkChara.maxHP;

        Sequence sequence = DOTween.Sequence();
        Character playerCharacter;
        Character enemyCharacter;
        charactersManager.keepCharacterDirection(atkChara);
        charactersManager.keepCharacterDirection(defChara);
        if (atkChara.isEnemy == false)
        {
            playerCharacter = atkChara;
            enemyCharacter = defChara;
            atkChara.MyAnimator.SetFloat("dirX", 1);
            atkChara.MyAnimator.SetFloat("dirZ", -1);
            defChara.MyAnimator.SetFloat("dirX", -1);
            defChara.MyAnimator.SetFloat("dirZ", -1);
        }
        else
        {
            playerCharacter = defChara;
            enemyCharacter = atkChara;
            defChara.MyAnimator.SetFloat("dirX", 1);
            defChara.MyAnimator.SetFloat("dirZ", -1);
            atkChara.MyAnimator.SetFloat("dirX", -1);
            atkChara.MyAnimator.SetFloat("dirZ", -1);
        }
        cameraManager.setBattleCamera(playerCharacter, enemyCharacter, () =>
        {
            if (accuracy <= atkCharaHitAttack && accuracy != 0)
            {
                defChara.nowHP = defCharaNowHp;
                sequence.Append(DOVirtual.DelayedCall(0f, () =>
                atkChara.setCharacterMotion(Character.CharacterMotion.attack), false).OnStart(() =>
                {
                    if (defChara.nowHP > 0)
                    {
                        defChara.setCharacterMotion(Character.CharacterMotion.damage);
                    }
                    else
                    {
                        defChara.setCharacterMotion(Character.CharacterMotion.dead);
                        DOVirtual.DelayedCall(1f, () => defChara.Dead(), false);
                    }
                }));
                sequence.Join(DOVirtual.DelayedCall(0.3f, () => battleWindowUI.updateHpText(defChara), false));
                sequence.Join(DOTween.To(() => defCharaAmount, (n) => defCharaAmount = n, defCharaEndAmount, 1.0f)
                .OnUpdate(() =>
                {
                    if (atkChara.isEnemy == false)
                    {
                        enemyCharacterHpGageImage.fillAmount = defCharaAmount;
                    }
                    else
                    {
                        playerCharacterHpGageImage.fillAmount = defCharaAmount;
                    }
                })
                .OnComplete(() =>
                {
                    if (defChara.nowHP > 0)
                    {
                        if (accuracy <= defCharaHitAttack && accuracy != 0)
                        {
                            atkChara.nowHP = atkCharaNowHp;
                            sequence.Append(DOVirtual.DelayedCall(0f, () =>
                            defChara.setCharacterMotion(Character.CharacterMotion.attack), false).OnStart(() =>
                            {
                                if (atkChara.nowHP > 0)
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.damage);
                                }
                                else
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.dead);
                                    DOVirtual.DelayedCall(1f, () => atkChara.Dead(), false);
                                }
                            }));
                            sequence.Join(DOVirtual.DelayedCall(0.3f, () => battleWindowUI.updateHpText(atkChara), false));
                            sequence.Join(DOTween.To(() => atkCharaAmount, (n) => atkCharaAmount = n, atkCharaEndAmount, 1.0f)
                            .OnUpdate(() =>
                            {
                                if (defChara.isEnemy == false)
                                {
                                    enemyCharacterHpGageImage.fillAmount = atkCharaAmount;
                                }
                                else
                                {
                                    playerCharacterHpGageImage.fillAmount = atkCharaAmount;
                                }
                            })
                            .OnComplete(() =>
                            {
                                StartCoroutine(cameraManager.cameraMoveOut(atkChara, defChara, () =>
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    defChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    charactersManager.setCharacterDirection(atkChara);
                                    charactersManager.setCharacterDirection(defChara);
                                    onAttackEnd.Invoke();
                                }));
                            }));
                        }
                        else
                        {
                            StartCoroutine(cameraManager.cameraMoveOut(atkChara, defChara, () =>
                            {
                                atkChara.setCharacterMotion(Character.CharacterMotion.wait);
                                defChara.setCharacterMotion(Character.CharacterMotion.wait);
                                charactersManager.setCharacterDirection(atkChara);
                                charactersManager.setCharacterDirection(defChara);
                                onAttackEnd.Invoke();
                            }));
                        }
                    }
                    else
                    {
                        StartCoroutine(cameraManager.cameraMoveOut(atkChara, defChara, () =>
                        {
                            atkChara.setCharacterMotion(Character.CharacterMotion.wait);
                            defChara.setCharacterMotion(Character.CharacterMotion.wait);
                            charactersManager.setCharacterDirection(atkChara);
                            charactersManager.setCharacterDirection(defChara);
                            onAttackEnd.Invoke();
                        }));
                    }
                }));
            }
            else
            {
                sequence.Append(DOVirtual.DelayedCall(0f, () => atkChara.setCharacterMotion(Character.CharacterMotion.attack), false));
                sequence.Join(DOVirtual.DelayedCall(0f, () => defChara.setCharacterMotion(Character.CharacterMotion.prevent), false));
                sequence.Join(DOTween.To(() => defCharaAmount, (n) => defCharaAmount = n, defCharaEndAmount, 1.0f)
                .OnComplete(() =>
                    {
                        if (accuracy <= defCharaHitAttack && accuracy != 0)
                        {
                            atkChara.nowHP = atkCharaNowHp;
                            sequence.Append(DOVirtual.DelayedCall(0f, () =>
                            defChara.setCharacterMotion(Character.CharacterMotion.attack), false).OnStart(() =>
                            {
                                if (atkChara.nowHP > 0)
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.damage);
                                }
                                else
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.dead);
                                    DOVirtual.DelayedCall(1f, () => atkChara.Dead(), false);
                                }
                            }));
                            sequence.Join(DOVirtual.DelayedCall(0.3f, () => battleWindowUI.updateHpText(atkChara), false));
                            sequence.Join(DOTween.To(() => atkCharaAmount, (n) => atkCharaAmount = n, atkCharaEndAmount, 1.0f)
                            .OnUpdate(() =>
                            {
                                if (defChara.isEnemy == false)
                                {
                                    enemyCharacterHpGageImage.fillAmount = atkCharaAmount;
                                }
                                else
                                {
                                    playerCharacterHpGageImage.fillAmount = atkCharaAmount;
                                }
                            })
                            .OnComplete(() =>
                            {
                                StartCoroutine(cameraManager.cameraMoveOut(atkChara, defChara, () =>
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    defChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    charactersManager.setCharacterDirection(atkChara);
                                    charactersManager.setCharacterDirection(defChara);
                                    onAttackEnd.Invoke();
                                }));
                            }));
                        }
                        else
                        {
                            sequence.Append(DOVirtual.DelayedCall(0f, () => defChara.setCharacterMotion(Character.CharacterMotion.attack), false));
                            sequence.Join(DOVirtual.DelayedCall(0f, () => atkChara.setCharacterMotion(Character.CharacterMotion.prevent), false));
                            sequence.Join(DOTween.To(() => atkCharaAmount, (n) => atkCharaAmount = n, atkCharaEndAmount, 1.0f)
                            .OnComplete(() =>
                            {
                                StartCoroutine(cameraManager.cameraMoveOut(atkChara, defChara, () =>
                                {
                                    atkChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    defChara.setCharacterMotion(Character.CharacterMotion.wait);
                                    charactersManager.setCharacterDirection(atkChara);
                                    charactersManager.setCharacterDirection(defChara);
                                    onAttackEnd.Invoke();
                                }));
                            }));
                        }
                    }));
            }
        });
    }
}
