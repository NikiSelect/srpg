using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : StateMachineBase<GameManager>
{
    //スクリプト連携
    [SerializeField] private CharactersManager charactersManager;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private EnemyTurnManager enemyTurnManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private SceneChange sceneChange;
    [SerializeField] private BattleWindowUI battleWindowUI;

    //ブロック情報
    public List<MapBlock> SearchBlocks = new List<MapBlock>();

    // ボタン処理
    private event EventHandler<MapBlock> onSelectBlock;
    private event EventHandler onAttackBottun;
    private event EventHandler onStanbyBottun;
    private event EventHandler onItemBottun;
    private event EventHandler onCancelBottun;
    private event EventHandler onWeaponSelect;

    //初期設定
    private void Start()
    {
        ChangeState(new GameManager.TurnInit(this));
    }

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GetMapBlockByTapPos();
        }
        base.Update();
    }

    //ブロック取得処理
    public void GetMapBlockByTapPos()
    {
        GameObject targetObject = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            targetObject = hit.collider.gameObject;
        }
        if (targetObject != null)
        {
            SelectBlock(targetObject.GetComponent<MapBlock>());
        }
    }

    public class enemyTurnWait : StateBase<GameManager>
    {
        public enemyTurnWait(GameManager _machine) : base(_machine)
        {
        }

        public override void OnEnterState()
        {
            Debug.Log("enemyTurnWait");
            machine.enemyTurnManager.BeginTurn(() =>
            {
                ChangeState(new GameManager.TurnInit(machine));
            }
            );
        }
    }
    public class TurnInit : StateBase<GameManager>
    {
        public TurnInit(GameManager _machine) : base(_machine)
        {
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnInit");
            machine.mapManager.AllSelectionModeClear();
            machine.mapManager.AllMoveRangeModeClear();
            machine.mapManager.AllAttackRangeModeClear();
            machine.guiManager.HideStatusWindow();
            machine.guiManager.HideBattlePredictWindow();
            machine.guiManager.HideBattleWindow();
            machine.charactersManager.ResetPlayerCharaActive();
            machine.charactersManager.ResetEnemyCharaActive();
            machine.charactersManager.setCharacterAttackRange();
            machine.guiManager.selectLogo("playerTurn", () =>
            {
                ChangeState(new GameManager.TurnStart(machine));
            });
        }
    }
    public class TurnStart : StateBase<GameManager>
    {
        public TurnStart(GameManager _machine) : base(_machine)
        {
        }

        public override void OnEnterState()
        {
            machine.mapManager.AllSelectionModeClear();
            machine.mapManager.AllMoveRangeModeClear();
            machine.mapManager.AllAttackRangeModeClear();
            machine.guiManager.HideStatusWindow();
            machine.guiManager.HideCommandButtons();
            machine.guiManager.HideBattlePredictWindow();
            machine.guiManager.HideBattleWindow();
            machine.charactersManager.setCharacterAttackRange();
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
            machine.onSelectBlock += Machine_onSelectBlock;
            bool noActive = machine.charactersManager.activePlayerCharaSeach();
            if (noActive == false)
            {
                Debug.Log("TurnStart");
            }
            else
            {
                ChangeState(new GameManager.enemyTurnWait(machine));
            }
        }

        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            machine.mapManager.AllSelectionModeClear();
            targetBlock.SetSelectionMode(true);
            var playerCharacter = machine.charactersManager.GetCharacterDataByPos(targetBlock.xPos, targetBlock.zPos);
            if (playerCharacter != null)
            {
                ChangeState(new GameManager.TurnCharacterSelect(machine, playerCharacter));
            }
        }

        public override void OnExitState()
        {
            machine.onSelectBlock -= Machine_onSelectBlock;
        }
    }
    public class TurnCharacterSelect : StateBase<GameManager>
    {
        private Character playerCharacter;
        private MapBlock cancelBlock;
        public TurnCharacterSelect(GameManager _machine, Character playerCharacter) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
        }

        public override void OnEnterState()
        {

            playerCharacter.setCharacterMotion(Character.CharacterMotion.idle);
            Debug.Log("TurnCharacterSelect");
            cancelBlock = machine.mapManager.getMapBlockAtPosition(playerCharacter.xPos, playerCharacter.zPos);
            machine.mapManager.moveStartSearch(cancelBlock.xPos, cancelBlock.zPos, playerCharacter.moveRange);
            machine.mapManager.setAttackSearchBlocks(playerCharacter);
            machine.mapManager.setMoveRangeBlocks(playerCharacter);
            machine.mapManager.setAttackRangeBlocks(playerCharacter);
            cancelBlock.moveRange = true;
            machine.guiManager.ShowStatusWindow(playerCharacter);
            cancelBlock.SetSelectionMode(false);

            machine.onSelectBlock += Machine_onSelectBlock;
        }

        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            bool moveBlock = machine.mapManager.selectMoveBlock(targetBlock.xPos, targetBlock.zPos);
            if (playerCharacter.active && moveBlock == true && playerCharacter.isEnemy == false)
            {
                ChangeState(new GameManager.TurnMove(machine, playerCharacter, targetBlock, cancelBlock));
            }
            else
            {
                playerCharacter.setCharacterMotion(Character.CharacterMotion.wait);
                ChangeState(new GameManager.TurnStart(machine));
            }
        }

        public override void OnExitState()
        {
            machine.onSelectBlock -= Machine_onSelectBlock;
        }
    }
    public class TurnMove : StateBase<GameManager>
    {
        private Character playerCharacter;
        private MapBlock targetBlock;
        private MapBlock cancelBlock;
        public TurnMove(GameManager _machine, Character playerCharacter, MapBlock targetBlock, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.targetBlock = targetBlock;
            this.cancelBlock = cancelBlock;
        }
        public override void OnEnterState()
        {
            Debug.Log("TurnMove");
            playerCharacter.setCharacterMotion(Character.CharacterMotion.walk);
            playerCharacter.MovePosition(targetBlock, playerCharacter, () =>
            {
                machine.mapManager.AllAttackRangeModeClear();
                machine.mapManager.AllMoveRangeModeClear();
                ChangeState(new GameManager.TurnCommandSelect(machine, playerCharacter, cancelBlock));
            });
        }
    }
    public class TurnCommandSelect : StateBase<GameManager>
    {
        Character playerCharacter;
        MapBlock targetBlock;
        MapBlock cancelBlock;
        public TurnCommandSelect(GameManager _machine, Character playerCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnCommand");
            playerCharacter.setCharacterMotion(Character.CharacterMotion.idle);
            machine.charactersManager.keepCharacterDirection(playerCharacter);
            machine.guiManager.ShowCommandButtons();
            machine.onAttackBottun += Machine_onAttackBlock;
            machine.onStanbyBottun += Machine_onStanbyBlock;
            machine.onCancelBottun += Machine_onCancelBlock;
            machine.onItemBottun += Machine_onItemBlock;
        }

        private void Machine_onAttackBlock(object sender, EventArgs e)
        {
            ChangeState(new GameManager.TurnWeaponSelect(machine, playerCharacter, cancelBlock));
        }
        private void Machine_onStanbyBlock(object sender, EventArgs e)
        {
            ChangeState(new GameManager.TurnStanbyCommand(machine, playerCharacter));
        }
        private void Machine_onItemBlock(object sender, EventArgs e)
        {
            ChangeState(new GameManager.TurnItemSelect(machine, playerCharacter, cancelBlock));
        }
        private void Machine_onCancelBlock(object sender, EventArgs e)
        {
            ChangeState(new GameManager.TurnCancelCommand(machine, playerCharacter, cancelBlock));
        }

        public override void OnExitState()
        {
            machine.guiManager.HideCommandButtons();
            machine.onAttackBottun -= Machine_onAttackBlock;
            machine.onStanbyBottun -= Machine_onStanbyBlock;
            machine.onCancelBottun -= Machine_onCancelBlock;
            machine.onItemBottun -= Machine_onItemBlock;
        }
    }
    public class TurnWeaponSelect : StateBase<GameManager>
    {
        private Character playerCharacter;
        private MapBlock cancelBlock;
        public TurnWeaponSelect(GameManager _machine, Character playerCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnWeaponSelect");
            playerCharacter.setCharacterMotion(Character.CharacterMotion.stunby);
            machine.guiManager.ShowItemList(playerCharacter);
            machine.onWeaponSelect += Machine_onWeaponSelect;
            machine.onSelectBlock += Machine_onSelectBlock;
        }

        private void Machine_onWeaponSelect(object sender, EventArgs e)
        {
            ChangeState(new GameManager.TurnEnemySelect(machine, playerCharacter, cancelBlock));
        }
        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            if (targetBlock != null)
            {
                machine.guiManager.HideItemList();
                ChangeState(new GameManager.TurnCommandSelect(machine, playerCharacter, cancelBlock));
            }
        }

        public override void OnExitState()
        {
            machine.mapManager.AllAttackRangeModeClear();
            playerCharacter.updataAttackRange(playerCharacter);
            machine.guiManager.HideItemList();
            machine.onSelectBlock -= Machine_onSelectBlock;
            machine.onWeaponSelect -= Machine_onWeaponSelect;
        }
    }
    public class TurnStanbyCommand : StateBase<GameManager>
    {
        private Character playerCharacter;
        public TurnStanbyCommand(GameManager _machine, Character playerCharacter) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnStanbyCommand");
            machine.charactersManager.SetactiveOut(playerCharacter);
            ChangeState(new GameManager.TurnStart(machine));
        }
    }
    public class TurnItemSelect : StateBase<GameManager>
    {
        private Character playerCharacter;
        private MapBlock targetBlock;
        private MapBlock cancelBlock;
        public TurnItemSelect(GameManager _machine, Character playerCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnItemSelect");
            machine.guiManager.ShowItemList(playerCharacter);
            machine.onSelectBlock += Machine_onSelectBlock;
        }

        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            if (targetBlock != null)
            {
                ChangeState(new GameManager.TurnCommandSelect(machine, playerCharacter, cancelBlock));
            }
        }

        public override void OnExitState()
        {
            machine.guiManager.HideItemList();
            machine.onSelectBlock -= Machine_onSelectBlock;
        }
    }
    public class TurnCancelCommand : StateBase<GameManager>
    {
        private MapBlock cancelBlock;
        private Character playerCharacter;
        public TurnCancelCommand(GameManager _machine, Character playerCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnCancelCommand");
            playerCharacter.MovePosition(cancelBlock, playerCharacter, () =>
            {
                machine.charactersManager.setCharacterDirection(playerCharacter);
                playerCharacter.setCharacterMotion(Character.CharacterMotion.wait);
                playerCharacter.itemAttackRange(playerCharacter);
                ChangeState(new GameManager.TurnStart(machine));
            });
        }
    }
    public class TurnEnemySelect : StateBase<GameManager>
    {
        private Character playerCharacter;
        private MapBlock cancelBlock;
        public TurnEnemySelect(GameManager _machine, Character playerCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnEnemySelect");
            machine.mapManager.attackStartSearch(playerCharacter.xPos, playerCharacter.zPos, playerCharacter.attackRange);
            machine.mapManager.setAttackRangeBlocks(playerCharacter);
            machine.onSelectBlock += Machine_onSelectBlock;
        }

        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            bool attackBlock = machine.mapManager.selectAttackBlock(targetBlock.xPos, targetBlock.zPos);
            var enemyCharacter = machine.charactersManager.GetCharacterDataByPos(targetBlock.xPos, targetBlock.zPos);
            if (attackBlock == true && enemyCharacter != null && enemyCharacter.isEnemy == true)
            {
                playerCharacter.characterDirectionChange(playerCharacter, enemyCharacter);
                ChangeState(new GameManager.TurnBattlePredict(machine, playerCharacter, enemyCharacter, cancelBlock));
            }
            else
            {
                ChangeState(new GameManager.TurnWeaponSelect(machine, playerCharacter, cancelBlock));
            }
        }

        public override void OnExitState()
        {
            machine.onSelectBlock -= Machine_onSelectBlock;
        }
    }
    public class TurnBattlePredict : StateBase<GameManager>
    {
        private Character playerCharacter;
        private Character enemyCharacter;
        private MapBlock cancelBlock;
        public TurnBattlePredict(GameManager _machine, Character playerCharacter, Character enemyCharacter, MapBlock cancelBlock) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.enemyCharacter = enemyCharacter;
            this.cancelBlock = cancelBlock;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnBattlePredict");
            enemyCharacter.setCharacterMotion(Character.CharacterMotion.stunby);
            machine.guiManager.ShowBattlePredictWindow(playerCharacter, enemyCharacter);
            machine.onSelectBlock += Machine_onSelectBlock;
        }

        private void Machine_onSelectBlock(object sender, MapBlock targetBlock)
        {
            bool attackBlock = machine.mapManager.selectAttackBlock(targetBlock.xPos, targetBlock.zPos);
            var enemySelect = machine.charactersManager.GetCharacterDataByPos(targetBlock.xPos, targetBlock.zPos);
            if (attackBlock == true)
            {
                if (enemySelect == enemyCharacter)
                {
                    machine.guiManager.HideStatusWindow();
                    ChangeState(new GameManager.TurnBattleStart(machine, playerCharacter, enemySelect));
                }
                else if (enemySelect != null)
                {
                    enemyCharacter.setCharacterMotion(Character.CharacterMotion.wait);
                    ChangeState(new GameManager.TurnBattlePredict(machine, playerCharacter, enemySelect, cancelBlock));
                }
            }

            if (enemySelect == null)
            {
                enemyCharacter.setCharacterMotion(Character.CharacterMotion.wait);
                ChangeState(new GameManager.TurnWeaponSelect(machine, playerCharacter, cancelBlock));
            }
        }

        public override void OnExitState()
        {
            machine.guiManager.HideBattlePredictWindow();
            machine.onSelectBlock -= Machine_onSelectBlock;
        }
    }
    public class TurnBattleStart : StateBase<GameManager>
    {
        private Character playerCharacter;
        private Character enemyCharacter;
        public TurnBattleStart(GameManager _machine, Character playerCharacter, Character enemyCharacter) : base(_machine)
        {
            this.playerCharacter = playerCharacter;
            this.enemyCharacter = enemyCharacter;
        }

        public override void OnEnterState()
        {
            Debug.Log("TurnBattleStart");
            machine.guiManager.ShowBattleWindow(playerCharacter, enemyCharacter);
            machine.battleManager.battleCalculation(playerCharacter, enemyCharacter, () =>
            {
                machine.guiManager.HideBattleWindow();
                if (playerCharacter != null)
                {
                    machine.charactersManager.SetactiveOut(playerCharacter);
                }
                ChangeState(new GameManager.TurnStart(machine));
            });
        }
    }

    //ブロック取得
    public void SelectBlock(MapBlock targetBlock)
    {
        onSelectBlock?.Invoke(this, targetBlock);
    }

    //攻撃コマンド選択
    public void AttakCommond()
    {
        onAttackBottun.Invoke(this, EventArgs.Empty);
    }
    //待機コマンド選択
    public void StanbyCommand()
    {
        onStanbyBottun.Invoke(this, EventArgs.Empty);
    }
    //キャンセルコマンド選択
    public void CancelCommand()
    {
        onCancelBottun.Invoke(this, EventArgs.Empty);
    }
    //アイテムコマンド選択
    public void ItemCommand()
    {
        onItemBottun.Invoke(this, EventArgs.Empty);
    }
    //武器選択
    public void weaponSelect()
    {
        onWeaponSelect.Invoke(this, EventArgs.Empty);
    }
}
