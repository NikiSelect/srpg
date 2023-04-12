using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private CharactersManager charactersManager;

    //ブロック情報
    public MapBlock[,] mapBlocks;
    public List<MapBlock> mainMapBlocks = new List<MapBlock>();
    public List<MapBlock> MoveBlocks = new List<MapBlock>();
    public List<MapBlock> AttackBlocks = new List<MapBlock>();
    public List<MapBlock> AttackSearchBlocks = new List<MapBlock>();

    // 定数定義
    public const int MAP_WIDTH = 12; // マップの横幅
    public const int MAP_HEIGHT = 12; // マップの縦幅

    //初期設定
    public void Start()
    {
        mapBlocks = new MapBlock[MAP_WIDTH, MAP_HEIGHT];

        int i = 0;
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int z = 0; z < MAP_HEIGHT; z++)
            {
                int posX = (int)mainMapBlocks[i].transform.position.x;
                int posZ = (int)mainMapBlocks[i].transform.position.z;
                mapBlocks[x, z] = mainMapBlocks[i];
                mapBlocks[x, z].xPos = posX;
                mapBlocks[x, z].zPos = posZ;
                i++;
            }
        }
    }

    public MapBlock getMapBlockAtPosition(int x, int z)
    {
        return mapBlocks[Mathf.Clamp(x, 0, MAP_WIDTH - 1), Mathf.Clamp(z, 0, MAP_HEIGHT - 1)];
    }

    //全選択ブロック初期化
    public void AllSelectionModeClear()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int z = 0; z < MAP_HEIGHT; z++)
            {
                mapBlocks[x, z].SetSelectionMode(false);
            }
        }
    }

    //全移動ブロック初期化
    public void AllMoveRangeModeClear()
    {
        MoveBlocks.Clear();
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int z = 0; z < MAP_HEIGHT; z++)
            {
                mapBlocks[x, z].SetMoveRangeMode(false);
                mapBlocks[x, z].blockCheck = false;
                mapBlocks[x, z].range = 0;
            }
        }
    }

    //全攻撃ブロック初期化
    public void AllAttackRangeModeClear()
    {
        AttackBlocks.Clear();
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int z = 0; z < MAP_HEIGHT; z++)
            {
                mapBlocks[x, z].SetAttackRangeMode(false);
                mapBlocks[x, z].blockCheck = false;
            }
        }
    }

    //移動範囲計算
    public void moveStartSearch(int posX, int posZ, int searchRange)
    {
        MapBlock SearchBlock = mapBlocks[posX, posZ];
        SearchBlock.range = searchRange;
        Character selectChara = charactersManager.GetCharacterDataByPos(posX, posZ);
        setMoveBlocks(SearchBlock);
        SearchBlock.SetMoveRangeMode(true);
        SearchBlock.blockCheck = true;
        moveSearch4(posX, posZ, searchRange, selectChara);
    }
    private void moveSearch4(int posX, int posZ, int searchRange, Character selectedChara)
    {

        if (0 <= posX && posX <= MAP_WIDTH && 0 <= posZ && posZ <= MAP_HEIGHT)
        {
            moveSearch(posX, posZ - 1, searchRange, selectedChara);
            moveSearch(posX, posZ + 1, searchRange, selectedChara);
            moveSearch(posX - 1, posZ, searchRange, selectedChara);
            moveSearch(posX + 1, posZ, searchRange, selectedChara);
        }
    }
    private void moveSearch(int posX, int posZ, int searchRange, Character selectedChara)
    {
        if (posX < 0 || MAP_WIDTH <= posX) return;
        if (posZ < 0 || MAP_HEIGHT <= posZ) return;

        MapBlock searchBlock = mapBlocks[posX, posZ];
        var charaData = charactersManager.GetCharacterDataByPos(searchBlock.xPos, searchBlock.zPos);
        if (searchBlock.blockCheck == false)
        {
            setMoveBlocks(searchBlock);
            searchBlock.blockCheck = true;
            if (charaData != null && charaData.isEnemy != selectedChara.isEnemy || searchBlock.chooseableBlock == false)
            {
                searchBlock.blockCheck = false;
                deleteMoveBlocks(searchBlock);
            }
        }

        if ((searchRange - 1) <= searchBlock.range) return;
        searchRange = searchRange + searchBlock.blockCost;

        if (charaData != null && charaData.isEnemy != selectedChara.isEnemy)
        {
            searchRange = searchRange - 10;
        }

        if (searchRange > 0)
        {
            searchBlock.range = searchRange;
            moveSearch4(posX, posZ, searchRange, selectedChara);
        }
        else
        {
            searchRange = 0;
        }
    }

    //移動可能か判定
    public bool selectMoveBlock(int Xpos, int Zpos)
    {
        bool selectMoveBlock = mapBlocks[Xpos, Zpos].moveRange;
        return selectMoveBlock;
    }

    //移動可能ブロックをリストへ追加
    public void setMoveBlocks(MapBlock searchBlock)
    {
        MoveBlocks.Add(searchBlock);
    }

    //移動可能ブロックをリストから削除
    public void deleteMoveBlocks(MapBlock searchBlock)
    {
        MoveBlocks.Remove(searchBlock);
    }

    //移動可能ブロックリスト取得
    public List<MapBlock> getMoveBlocks()
    {
        return MoveBlocks;
    }

    //移動可能ブロックを設定
    public void setMoveRangeBlocks(Character Chara)
    {
        for (int i = 0; i < MoveBlocks.Count; i++)
        {
            MoveBlocks[i].SetMoveRangeMode(true);
            var CharaData = charactersManager.GetCharacterDataByPos(MoveBlocks[i].xPos, MoveBlocks[i].zPos);
            if (CharaData != null && Chara.isEnemy == CharaData.isEnemy)
            {
                MoveBlocks[i].moveRange = false;
            }
        }
    }

    //移動するブロックの順番を計算
    public List<MapBlock> setMoveRoute(MapBlock goalBlock, Character selectChara)
    {
        var moveRouteBlocks = new List<MapBlock>();
        MapBlock setBlock = goalBlock;
        setBlock.blockNumber = 99;//Math.Abs(setBlock.xPos - selectChara.xPos) + Math.Abs(setBlock.zPos - selectChara.zPos);
        moveRouteBlocks.Add(setBlock);
        if (selectChara.moveRange != goalBlock.range)
        {
            for (int i = 0; i < MoveBlocks.Count; i++)
            {
                if (MoveBlocks[i] != goalBlock)
                {
                    MoveBlocks[i].blockNumber = Math.Abs(MoveBlocks[i].xPos - selectChara.xPos) + Math.Abs(MoveBlocks[i].zPos - selectChara.zPos);
                }
            }
            for (int n = goalBlock.range + 1; n < selectChara.moveRange; n++)
            {
                for (int k = 0; k < MoveBlocks.Count; k++)
                {
                    if ((MoveBlocks[k].xPos == setBlock.xPos && MoveBlocks[k].zPos == setBlock.zPos - 1)
                            || (MoveBlocks[k].xPos == setBlock.xPos && MoveBlocks[k].zPos == setBlock.zPos + 1)
                            || (MoveBlocks[k].xPos == setBlock.xPos - 1 && MoveBlocks[k].zPos == setBlock.zPos)
                            || (MoveBlocks[k].xPos == setBlock.xPos + 1 && MoveBlocks[k].zPos == setBlock.zPos)
                            )
                    {
                        if (n == MoveBlocks[k].range)
                        {
                            setBlock = MoveBlocks[k];
                            break;
                        }
                    }
                }
                moveRouteBlocks.Add(setBlock);
            }
            moveRouteBlocks.Sort((b, a) => b.blockNumber - a.blockNumber);
        }
        return moveRouteBlocks;
    }

    //攻撃範囲計算
    public void attackStartSearch(int posX, int posZ, int searchRange)
    {
        MapBlock SearchBlock = mapBlocks[posX, posZ];
        SearchBlock.blockCheck = true;
        attackSearch4(posX, posZ, searchRange);
    }
    private void attackSearch4(int posX, int posZ, int searchRange)
    {

        if (0 <= posX && posX <= MAP_WIDTH && 0 <= posZ && posZ <= MAP_HEIGHT)
        {
            attackSearch(posX, posZ - 1, searchRange);
            attackSearch(posX, posZ + 1, searchRange);
            attackSearch(posX - 1, posZ, searchRange);
            attackSearch(posX + 1, posZ, searchRange);
        }
    }
    private void attackSearch(int posX, int posZ, int searchRange)
    {
        if (posX < 0 || MAP_WIDTH <= posX) return;
        if (posZ < 0 || MAP_HEIGHT <= posZ) return;

        MapBlock searchBlock = mapBlocks[posX, posZ];
        if (searchBlock.blockCheck == false && searchBlock.chooseableBlock == true)
        {
            setAttackBlocks(searchBlock);
            searchBlock.blockCheck = true;
        }

        if ((searchRange - 1) <= searchBlock.range) return;

        searchRange = searchRange - 1;

        if (searchRange > 0)
        {
            attackSearch4(posX, posZ, searchRange);
        }
        else
        {
            searchRange = 0;
        }
    }

    //攻撃可能か判定
    public bool selectAttackBlock(int Xpos, int Zpos)
    {
        bool selectAttackBlock = mapBlocks[Xpos, Zpos].attackRange;
        return selectAttackBlock;
    }

    //攻撃可能ブロックをリストへ追加
    public void setAttackBlocks(MapBlock searchBlock)
    {
        AttackBlocks.Add(searchBlock);
    }

    //攻撃可能ブロックをリストから削除    
    public List<MapBlock> getAttackBlocks()
    {
        return AttackBlocks;
    }

    //攻撃可能ブロックを取得
    public void deleteAttackBlocks(MapBlock searchBlock)
    {
        AttackBlocks.Remove(searchBlock);
    }

    //攻撃可能ブロックを設定
    public void setAttackRangeBlocks(Character Chara)
    {
        for (int i = 0; i < AttackBlocks.Count; i++)
        {
            AttackBlocks[i].SetAttackRangeMode(true);
            var CharaData = charactersManager.GetCharacterDataByPos(AttackBlocks[i].xPos, AttackBlocks[i].zPos);
            if (CharaData != null && Chara.isEnemy == CharaData.isEnemy)
            {
                AttackBlocks[i].attackRange = false;
            }
        }
    }

    //最大移動後の攻撃範囲表示
    public void setAttackSearchBlocks(Character selectChara)
    {
        for (int i = 0; i < MoveBlocks.Count; i++)
        {
            attackStartSearch(MoveBlocks[i].xPos, MoveBlocks[i].zPos, selectChara.attackRange);
        }
    }


}
