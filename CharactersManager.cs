using System;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    //戦闘キャラクターリスト
    public List<Character> characters;

    //初期設定
    void Start()
    {
        Character.OnAnyCharacterDead += Character_OnAnyCharacterDead;
        ResetPlayerCharaActive();
        ResetEnemyCharaActive();
    }

    //キャラクターの死亡処理
    private void Character_OnAnyCharacterDead(object sender, EventArgs e)
    {
        Character character = sender as Character;
        if (characters.Contains(character))
        {
            character.gameObject.SetActive(false);
            characters.Remove(character);
        }
    }

    //キャラクター情報取得
    public Character GetCharacterDataByPos(int xPos, int zPos)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].xPos == xPos && characters[i].zPos == zPos)
            {
                return characters[i];
            }
        }
        return null;
    }

    //味方キャラクターが行動済みか確認
    public bool activePlayerCharaSeach()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].active == true && characters[i].isEnemy == false)
            {
                return false;
            }
        }
        return true;
    }

    //敵キャラクターが行動済みか確認
    public bool activeEnemyCharaSeach()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].active == true && characters[i].isEnemy == true)
            {
                return false;
            }
        }
        return true;
    }

    //味方キャラクターを未行動に変更
    public void ResetPlayerCharaActive()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isEnemy == false)
            {
                characters[i].active = true;
                characters[i].MyAnimator.SetTrigger("statusNormal");
            }
        }
    }

    //敵キャラクターを未行動に変更
    public void ResetEnemyCharaActive()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isEnemy == true)
            {
                characters[i].active = true;
                characters[i].MyAnimator.SetTrigger("statusNormal");
            }
        }
    }

    //キャラクターを行動済みに変更
    public void SetactiveOut(Character Chara)
    {
        Chara.active = false;
        Chara.setCharacterMotion(Character.CharacterMotion.wait);
        Chara.MyAnimator.SetTrigger("statusMoved");
    }

    public void setCharacterAttackRange()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].itemAttackRange(characters[i]);
        }
    }

    //ゲーム終了条件
    public bool GameClear()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isEnemy == true && characters[i].nowHP > 0)
            {
                return false;
            }
        }
        return true;
    }
    public bool GameOver()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isEnemy == false && characters[i].nowHP > 0)
            {
                return false;
            }
        }
        return true;
    }

    //未行動の敵キャラクターを選択
    public Character GetFirstActiveEnemy()
    {
        foreach (Character character in characters)
        {
            if (character.isEnemy && character.active)
            {
                return character;
            }
        }
        return null;
    }

    //現在のキャラクター向き情報を保存
    public void keepCharacterDirection(Character selectChara)
    {
        selectChara.dirX = selectChara.MyAnimator.GetFloat("dirX");
        selectChara.dirZ = selectChara.MyAnimator.GetFloat("dirZ");
    }

    //保存したキャラクター向きへ変更
    public void setCharacterDirection(Character selectChara)
    {
        selectChara.MyAnimator.SetFloat("dirX", selectChara.dirX);
        selectChara.MyAnimator.SetFloat("dirZ", selectChara.dirZ);
    }

    //敵キャラクターが攻撃しないときの行動
    public MapBlock enemyPersonalityMove(Character selectChara, List<MapBlock> moveBlocks)
    {
        Character nearCharacter = null;
        int selectCharaPosX = selectChara.xPos;
        int selectCharaPosZ = selectChara.zPos;
        int distance = 99;
        for (int i = 0; i < characters.Count; i++)
        {
            int characterDistancePosX = Math.Abs(characters[i].xPos - selectCharaPosX);
            int characterDistancePosZ = Math.Abs(characters[i].zPos - selectCharaPosZ);
            int characterDistance = characterDistancePosX + characterDistancePosZ;
            if (selectChara.isEnemy != characters[i].isEnemy && distance > characterDistance)
            {
                nearCharacter = characters[i];
                distance = characterDistance;
                Debug.Log($"{characters[i].name}:{characterDistance}");
            }
        }
        int nearPosX = nearCharacter.xPos;
        int nearPosZ = nearCharacter.zPos;
        float nearDistance = 99f;
        MapBlock goalBlock = null;

        switch (selectChara.enemyPersonality)
        {

            case Character.EnemyPersonality.approach:

                for (int i = 0; i < moveBlocks.Count; i++)
                {
                    float moveBlockDistance = Vector2.Distance(new Vector2(nearPosX, nearPosZ), new Vector2(moveBlocks[i].xPos, moveBlocks[i].zPos));
                    if (nearDistance > moveBlockDistance)
                    {
                        goalBlock = moveBlocks[i];
                        nearDistance = moveBlockDistance;
                        Debug.Log($"{moveBlocks[i]}:{nearDistance}");
                    }
                }
                return goalBlock;

            case Character.EnemyPersonality.protect:

                return goalBlock;

            case Character.EnemyPersonality.Intercept:

                return goalBlock;

            case Character.EnemyPersonality.stay:

                return goalBlock;
        }
        return goalBlock;
    }
}
