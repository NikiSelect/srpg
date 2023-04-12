using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnManager : StateMachineBase<EnemyTurnManager>
{
    //スクリプト連携
    [SerializeField] private CharactersManager charactersManager;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private BattleWindowUI battleWindowUI;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private SceneChange sceneChange;
    [SerializeField] private GameManager gameManager;

    //ターン終了用
    private Action turnEndHandler;

    //初期設定
    void Start()
    {
        ChangeState(new EnemyTurnManager.playerTurnWait(this));
    }

    //敵ターン呼び出し
    public void BeginTurn(Action onCompleted)
    {
        turnEndHandler = onCompleted;
        ChangeState(new EnemyTurnManager.TurnInit(this));
    }

    //待機状態
    private class playerTurnWait : StateBase<EnemyTurnManager>
    {
        public playerTurnWait(EnemyTurnManager _machine) : base(_machine)
        {
        }

        public override void OnEnterState()
        {
            Debug.Log("playerTurnWait");
        }
    }

    private class TurnInit : StateBase<EnemyTurnManager>
    {
        public TurnInit(EnemyTurnManager _machine) : base(_machine)
        {
        }
        public override void OnEnterState()
        {
            machine.charactersManager.ResetPlayerCharaActive();
            machine.charactersManager.ResetEnemyCharaActive();
            machine.mapManager.AllSelectionModeClear();
            machine.mapManager.AllMoveRangeModeClear();
            machine.mapManager.AllAttackRangeModeClear();
            machine.guiManager.HideStatusWindow();
            machine.guiManager.HideCommandButtons();
            machine.guiManager.HideBattleWindow();
            machine.guiManager.HideBattlePredictWindow();
            machine.guiManager.selectLogo("enemyTurn", () =>
            {
                ChangeState(new EnemyTurnManager.TurnStart(machine));
            });
        }
        public override void OnExitState()
        {
        }
    }

    private class TurnStart : StateBase<EnemyTurnManager>
    {
        public TurnStart(EnemyTurnManager _machine) : base(_machine)
        {
        }
        public override void OnEnterState()
        {
            machine.mapManager.AllSelectionModeClear();
            machine.mapManager.AllMoveRangeModeClear();
            machine.mapManager.AllAttackRangeModeClear();
            machine.guiManager.HideStatusWindow();
            machine.guiManager.HideCommandButtons();
            machine.guiManager.HideBattleWindow();
            machine.guiManager.HideBattlePredictWindow();
            bool gameEnd = machine.charactersManager.GameOver();
            if (gameEnd == true)
            {
                machine.guiManager.selectLogo("GameOver", () =>
                {
                    machine.sceneChange.GameOverSceneChange();
                });
            }
            gameEnd = machine.charactersManager.GameClear();
            if (gameEnd == true)
            {
                machine.guiManager.selectLogo("GameClear", () =>
                {
                    machine.sceneChange.GameClearSceneChange();
                });
            }
            bool noActive = machine.charactersManager.activeEnemyCharaSeach();
            if (noActive == false)
            {
                Character enemyChara = machine.charactersManager.GetFirstActiveEnemy();
                ChangeState(new EnemyTurnManager.TurnMove(machine, enemyChara));
            }
            else
            {
                ChangeState(new EnemyTurnManager.TurnEnd(machine));
            }
        }
    }

    private class TurnMove : StateBase<EnemyTurnManager>
    {
        private Character enemyCharacter;
        public TurnMove(EnemyTurnManager _machine, Character enemyCharacter) : base(_machine)
        {
            this.enemyCharacter = enemyCharacter;
        }

        public override void OnEnterState()
        {
            bool exitFlag = false;
            var moveBlocks = new List<MapBlock>();
            var attackBlocks = new List<MapBlock>();
            enemyCharacter.setCharacterMotion(Character.CharacterMotion.idle);
            machine.mapManager.moveStartSearch(enemyCharacter.xPos, enemyCharacter.zPos, enemyCharacter.moveRange);
            machine.mapManager.setMoveRangeBlocks(enemyCharacter);
            moveBlocks = machine.mapManager.getMoveBlocks();
            foreach (MapBlock moveBlock in moveBlocks)
            {

                Character Chara = machine.charactersManager.GetCharacterDataByPos(moveBlock.xPos, moveBlock.zPos);
                if (Chara == null)
                {
                    machine.mapManager.attackStartSearch(moveBlock.xPos, moveBlock.zPos, enemyCharacter.attackRange);
                    machine.mapManager.setAttackRangeBlocks(enemyCharacter);
                    attackBlocks = machine.mapManager.getAttackBlocks();
                }
                foreach (MapBlock attackBlock in attackBlocks)
                {
                    Character playerChara = machine.charactersManager.GetCharacterDataByPos(attackBlock.xPos, attackBlock.zPos);
                    if (playerChara != null && playerChara.isEnemy == false)
                    {
                        exitFlag = true;
                        enemyCharacter.MovePosition(moveBlock, enemyCharacter, () =>
                        {
                            ChangeState(new EnemyTurnManager.TurnAttack(machine, enemyCharacter, playerChara));
                        });
                        break;
                    }
                }
                if (exitFlag)
                {
                    break;
                }
            }
            if (exitFlag == false)
            {
                MapBlock moveBlock = machine.charactersManager.enemyPersonalityMove(enemyCharacter, moveBlocks);
                enemyCharacter.MovePosition(moveBlock, enemyCharacter, () =>
                {
                    moveBlocks.Clear();
                    attackBlocks.Clear();
                    machine.mapManager.AllMoveRangeModeClear();
                    machine.mapManager.AllAttackRangeModeClear();
                    machine.charactersManager.SetactiveOut(enemyCharacter);
                    ChangeState(new EnemyTurnManager.TurnStart(machine));
                });
            }
        }
    }

    private class TurnAttack : StateBase<EnemyTurnManager>
    {
        private Character enemyCharacter;
        private Character playerCharacter;
        public TurnAttack(EnemyTurnManager _machine, Character enemyCharacter, Character playerCharacter) : base(_machine)
        {
            this.enemyCharacter = enemyCharacter;
            this.playerCharacter = playerCharacter;
        }

        public override void OnEnterState()
        {
            playerCharacter.setCharacterMotion(Character.CharacterMotion.stunby);
            enemyCharacter.setCharacterMotion(Character.CharacterMotion.stunby);
            enemyCharacter.characterDirectionChange(playerCharacter, enemyCharacter);
            machine.mapManager.AllMoveRangeModeClear();
            machine.mapManager.AllAttackRangeModeClear();
            DOVirtual.DelayedCall(
            0.1f,
            () =>
            {
                playerCharacter.setCharacterMotion(Character.CharacterMotion.stunby);
                machine.guiManager.ShowBattleWindow(playerCharacter, enemyCharacter);
                machine.battleManager.battleCalculation(enemyCharacter, playerCharacter, () =>
                    {
                        machine.charactersManager.SetactiveOut(enemyCharacter);
                        machine.guiManager.HideBattleWindow();
                        ChangeState(new EnemyTurnManager.TurnStart(machine));
                    });

            });
        }
    }

    private class TurnEnd : StateBase<EnemyTurnManager>
    {
        public TurnEnd(EnemyTurnManager _machine) : base(_machine)
        {
        }

        public override void OnEnterState()
        {
            Debug.Log("EnemyTurnManager_TrunEnd");
            machine.turnEndHandler.Invoke();
            machine.turnEndHandler = null;
            ChangeState(new EnemyTurnManager.playerTurnWait(machine));
        }
    }
}
