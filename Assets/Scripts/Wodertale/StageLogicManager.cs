using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using R3;
using DG.Tweening;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// ターンの種類
/// </summary>
public enum TurnState {
    None,
    Player,
    Enemy,
    Boss
}

public class StageLogicManager : MonoBehaviour {

    //[SerializeField] private GSSReceiver GSSReceiver;
    [SerializeField] private StageGenerator stageGenerator;
    [SerializeField] private SymbolGenerator symbolGenerator;

    [SerializeField] private GameObject maskFieldObj;
    [SerializeField] private GameObject imgLevelUpPrefab;

    [SerializeField] private Transform overlayCanvasTran;
    [SerializeField] private MoveTimeScaleController moveTimeScaleController;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private StageUIManager stageUIManager;
    [SerializeField] private EnemySymbol bossSymbol;
    [SerializeField] private Ease ease;

    private int levelupCount;
    private SaveData saveData;

    private TurnState currentTurnState = TurnState.None;

    public TurnState CurrentTurnState {
        set => currentTurnState = value;
        get => currentTurnState;
    }

    private CancellationTokenSource cts;


    void Start() {
        SoundManager.instance.PlayBGM(BGM_TYPE.Stage_0);

        var token = this.GetCancellationTokenOnDestroy();
        cts = new CancellationTokenSource();

        // TODO Player の Hp 設定


        BattleManager.instance.SetUp(stageUIManager);

        // SO のデータを取得するまで待機
        //await UniTask.WaitUntil(() => GSSReceiver.IsLoading);

        // デバッグ用。Title が追加されたら使わない
        //SceneStateManager.instance.PreparateStageSceneAsync();

        // ステージのランダム作成(StageData 作成後は StageData に登録されている StageType を渡す)
        stageGenerator.GenerateStageFromRandomTiles(GameData.instance.currentStageData.stageType);

        // 通常のシンボルのランダム作成して List に追加
        //SymbolManager.instance.AllClearSymbolsList();

        symbolGenerator.SetUp();
        SymbolManager.instance.SymbolsList = symbolGenerator.GenerateSymbols(-1, stageGenerator.TilemapColision, stageGenerator.TilemapWalk, stageGenerator.terrainInfoDic);

        // 特殊シンボルのランダム作成して List に追加
        //SymbolManager.instance.SymbolsList.AddRange(stageGenerator.GenerateSpecialSymbols(GameData.instance.currentStageData.orbTypes));

        // 全シンボルの設定
        SymbolManager.instance.SetUpAllSymbols();

        // スタミナの値をステージごとの初期値に設定(StageData 作成後)
        //GameData.instance.userData.Stamina.Value = GameData.instance.currentStageData.initStamina;

        // UI 初期設定
        stageUIManager.SetupStageUIManager(GameData.instance.userData.Stamina.Value, GameData.instance.charaStatus.MaxHp.Value);

        // スタミナの値の購読開始
        GameData.instance.userData.Stamina.Subscribe(stamina => stageUIManager.UpdateDisplayStaminaPoint(stamina));

        //GameData.instance.maxHp = GameData.instance.hp;

        GameData.instance.InitPlayerCombatData();


        // TOOD Hp 購読処理



        // Hp表示更新
        //StartCoroutine(UpdateDisplayHp());

        // プレイヤーレベルと経験値の表示更新
        //UpdateDisplayPlayerLevel();
        //UpdateDisplayExp(true);

        // プレイヤーの設定
        //mapMoveController.SetUpMapMoveController(this);
        //inputButtonManager.SetUpInputButtonManager(mapMoveController);

        CurrentTurnState = TurnState.Player;

        // プレイヤーの移動の監視(OnEnable でやっている)
        //StartCoroutine(ObserveEnemyTurnState());

        //SymbolManager.instance.SwitchEnemyCollider(true);

        // アビリティ選択用ウインドウの生成
        //CreateSelectAbilityPopUp();

        //btnPlayerLevel.onClick.AddListener(OnClickPlayerLevel);
        //moveTimeScaleController.SetUpMoveButtonController();

        // ドロップするトレジャーの情報を準備
        DataBaseManager.instance.CreateDropItemDatasList(GameData.instance.currentStageData.dropTreasureLevel);

        playerMovement.SetUp();
        playerMovement.MovePlayerProc();
        playerMovement.SetTerrainInfo(stageGenerator.terrainInfoDic);

        playerMovement.OnPlayerMooveComplete.Subscribe(_ => ExecuteEnemyTurnAsync().Forget());

        FloatingViewGenerator.instance.SetUp(gameObject);

        //BattleManager.instance.UpdatePlayerHp(GameData.instance.debugMaxHp, false);
        //BattleManager.instance.PlayerHP
        //    .Zip(BattleManager.instance.PlayerHP.Skip(1), (oldValue, newValue) => (oldValue, newValue))
        //    .Subscribe(hp => {
        //        // UI 更新
        //        stageUIManager.UpdateDisplayPlayerHp(hp.oldValue, hp.newValue);
        //        //BattleManager.instance.CheckEndCondition();
        //    });

        //BattleManager.instance.UpdateEnemyHp(GameData.instance.debugMaxHp, false);
        //BattleManager.instance.EnemyHP
        //    .Zip(BattleManager.instance.EnemyHP.Skip(1), (oldValue, newValue) => (oldValue, newValue))
        //    .Subscribe(hp => {

        //        // TODO 敵用のメソッドに変える
        //        stageUIManager.UpdateDisplayPlayerHp(hp.oldValue, hp.newValue);
        //        //BattleManager.instance.CheckEndCondition();
        //    });

        // BackPackInItem のオブジェクトプールの初期化、List の購読などを設定
        PlayerInventoryManager.instance.Setup(stageUIManager.playerBackPackItemTran);

        EnemyInfoDisplayManager.instance.Setup(stageUIManager.enemyBackPackItemTran);

        //GameData.instance.userData.WalkCount.Subscribe(walkCount => 
        //{
        //    int waveNo = DataBaseManager.instance.GetWaveCount(walkCount, GameData.instance.userData.waveCount);
        //    if(waveNo > GameData.instance.userData.waveCount) {
        //        GameData.instance.userData.waveCount = waveNo;

        //        // シンボルの削除と再作成

        //    }
        //}).AddTo(this);

        stageUIManager.SetSliderWalkCount(ConstData.WAVE_WALK_COUNT);  // GameData.instance.GetMaxWaveCount(GameData.instance.userData.waveNo)

        // 歩数の購読１。歩数表示更新
        GameData.instance.userData.WalkCount
            .Where(_ => GameData.instance.userData.waveNo < 10)
            .Subscribe(walkCount => {
                stageUIManager.UpdateWalkCount(walkCount);

                // 次の Wave まで残り5歩(45/95/145/195..)になったら表示(バトルのエフェクトと重なる場合、こちらを優先)
                int offsetCount = (DataBaseManager.instance.GetWaveNo(walkCount, GameData.instance.userData.waveNo) - 1) * ConstData.WAVE_WALK_COUNT;
                if (walkCount != 0 && walkCount % (ConstData.WAVE_INFO_COUNT + offsetCount) == 0) {
                    stageUIManager.NextWaveInfo();
                }
            }).AddTo(this);

        // 歩数の購読２。歩数による WaveNo の更新と次の Wave 用のステージ、シンボルの作成
        GameData.instance.userData.WalkCount
            .Select(walkCount => {
                int waveNo = DataBaseManager.instance.GetWaveNo(walkCount, GameData.instance.userData.waveNo);
                return new { walkCount, waveNo };
            })
            .Where(result => result.waveNo > GameData.instance.userData.waveNo)
            .Subscribe(async result => {

                //GameData.instance.gameState.Value = GameData.GameState.Battle;
                //await ExecuteEnemyTurnAsync();
                //await UniTask.WaitUntil(() => GameData.instance.gameState.Value == GameData.GameState.Play);
                GameData.instance.gameState.Value = GameData.GameState.Prepare;

                DebugLogger.Log($"WaveNo 更新 : { result.waveNo}");
                GameData.instance.userData.waveNo = result.waveNo;
                stageUIManager.UpdateWaveNo(GameData.instance.userData.waveNo);

                SceneStateManager.instance.FadeIn().Forget();
                await UniTask.Delay(1000, cancellationToken:token);

                // Wave のデータをもらう
                WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);
                stageGenerator.SetTilemapData(waveData);

                // タイル削除して新しく生成
                stageGenerator.GenerateStageFromRandomTiles(GameData.instance.currentStageData.stageType);

                // ランダムな SymbolRate と Wave のデータをもらう

                // シンボルの削除と再作成処理
                SymbolManager.instance.AllClearSymbolsList();
                SymbolManager.instance.SymbolsList = symbolGenerator.GenerateSymbols(-1, stageGenerator.TilemapColision, stageGenerator.TilemapWalk, stageGenerator.terrainInfoDic);
                SymbolManager.instance.SetUpAllSymbols();

                stageUIManager.ResetSliderWalkCount();
                stageUIManager.SetSliderWalkCount(ConstData.WAVE_WALK_COUNT);  // GameData.instance.GetMaxWaveCount(GameData.instance.userData.waveNo)

                // Player の移動用データ更新
                playerMovement.SetTerrainInfo(stageGenerator.terrainInfoDic);

                await UniTask.Delay(1500, cancellationToken: token);
                SymbolManager.instance.ResetEvent();
                //SceneStateManager.instance.FadeOut();
                GameData.instance.gameState.Value = GameData.GameState.Play;
            })
            .AddTo(this);

        // ボスの設定
        bossSymbol.OnEnterSymbol();

        GameData.instance.gameState.Value = GameData.GameState.Play;
    }

    public async UniTask ExecuteEnemyTurnAsync() {
        GameData.instance.gameState.Value = GameData.GameState.Battle;
        //while (CurrentTurnState == TurnState.Enemy) {    // あとで GameState に変える

        //if (CurrentTurnState == TurnState.Enemy) {
        //Debug.Log("敵の移動　開始");
        //yield return StartCoroutine(symbolManager.EnemisMove());

        //Debug.Log("すべての敵の移動 完了");

        // バトルイベントを発生させる
        BattleResultType battleResultType = await SymbolManager.instance.ExecuteEnemySymbolEventAsync(CurrentTurnState, cts.Token);

        DebugLogger.Log(battleResultType.ToString());

        if (CurrentTurnState == TurnState.Boss) {
            if (battleResultType == BattleResultType.Win) {
                SoundManager.instance.PlayBGM(BGM_TYPE.Clear);
                DebugLogger.Log("ゲームクリア");

                // データセーブ
                PlayerPrefsHelper.ConditionalSave(saveData);

                await UniTask.Delay(1000, cancellationToken: cts.Token);
                SoundManager.instance.PlayVoice(VOICE_TYPE.Win);

                stageUIManager.ShowRestartMessage();

                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);

                // TODO 一旦タイトルへ
                SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
            } else {
                DebugLogger.Log("ゲームオーバー");

                SoundManager.instance.PlayVoice(VOICE_TYPE.Loose);
                stageUIManager.ShowRestartMessage();

                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);
                SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
            }
            return;
        }

        // コンディションの残り時間を減算するためのイベントを発火(ここで減算しないと追加時に1回分すぐに消費してしまうため)
        SymbolManager.instance.onTurnEnd.OnNext(default);

        // バトル以外のイベントを発生させる
        await SymbolManager.instance.TriggerEventProc();

        GameData.instance.gameState.Value = GameData.GameState.Play;
        GameData.instance.userData.WalkCount.Value++;
        SymbolManager.instance.ResetEvent();

        // 直前にスタミナ回復を取っている場合にはボスにならないことを確認済
        // ターンの状態を確認
        if (battleResultType != BattleResultType.Lose) {
            // スタミナが 0 の場合、TurnState を Boss に切り替える
            CheckTurn();
            //CheckTreasureBox();
        } else if (battleResultType == BattleResultType.Lose) {
            DebugLogger.Log("ゲームオーバー");

            SoundManager.instance.PlayVoice(VOICE_TYPE.Loose);
            stageUIManager.ShowRestartMessage();

            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);
            SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
            return;
        }

        if (CurrentTurnState == TurnState.Boss) {
            DebugLogger.Log("Boss 出現");

            // ボスの出現演出
            PreparateBossEffect();
            return;
        }
        await UniTask.Delay(50, cancellationToken: cts.Token);
        DebugLogger.Log("Player 移動再開");

        // プレイヤーの移動再開
        playerMovement.MovePlayerProc();
    }

    /// <summary>
    /// ターンの確認。プレイヤーのターンに切り替え。コンディションの更新
    /// </summary>
    private void CheckTurn() {
        if (GameData.instance.userData.Stamina.Value <= 0) {
            CurrentTurnState = TurnState.Boss;
        } else {
            CurrentTurnState = TurnState.Player;

            // コンディションの残り時間の更新(いまは MapController 内の２箇所でやっているので、ここで一本化する)
            //UpdateConditionsDuration();

            // TODO UI を押せるようにする
            // 移動ボタンと足踏みボタンを押せる状態にする
            //ActivateInputButtons();

            // コンディションの効果を適用
            ApplyEffectConditions();
        }
        DebugLogger.Log(CurrentTurnState);
    }

    /// <summary>
    /// プレイヤーレベルのボタンを押下した際の処理
    /// </summary>
    private void OnClickPlayerLevel() {
        //Debug.Log("Show SelectAbilityPopUp");

        // フィールドを隠す
        SwitchMaskField(false);

        //selectAbilityPopUp.ShowPopUp();
    }

    /// <summary>
    /// マスクで切り抜いて表示しているフィールドの表示/非表示の切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchMaskField(bool isSwitch) {
        maskFieldObj.SetActive(isSwitch);
    }

    /// <summary>
    /// アビリティ選択用ウインドウの生成と初期設定
    /// </summary>
    

    /// <summary>
    /// アビリティ強化時のエフェクトをプレイヤー上で再生
    /// </summary>
    public IEnumerator PlayAbilityPowerUpEffect() {
        //GameObject effect_1 = Instantiate(EffectManager.instance.abilityPowerUpPrefab_1, mapMoveController.transform.position, EffectManager.instance.abilityPowerUpPrefab_1.transform.rotation);
        //effect_1.transform.position = new Vector3(effect_1.transform.position.x, effect_1.transform.position.y - 0.35f, effect_1.transform.position.z);
        //Destroy(effect_1, 1.5f);

        yield return new WaitForSeconds(1.5f);

        //GameObject effect_2 = Instantiate(EffectManager.instance.abilityPowerUpPrefab_2, mapMoveController.transform.position, EffectManager.instance.abilityPowerUpPrefab_2.transform.rotation);
        ////effect_2.transform.position = new Vector3(effect_2.transform.position.x, effect_2.transform.position.y - 0.5f, effect_2.transform.position.z);
        //Destroy(effect_2, 1.0f);
    }

    /// <summary>
    /// コンディションの効果を適用
    /// </summary>
    private void ApplyEffectConditions() {
        foreach (PlayerConditionBase condition in GameData.instance.playerCombatData.conditionList) {

            //condition.ApplyEffect();

            //// 睡眠(移動不可)の場合
            //if (condition.GetConditionType() == ConditionType.Sleep) {
            //    // 足踏みしか出来ないように入力制限する 
            //    inputButtonManager.SwitchActivateMoveButtons(false);
            //}
            //// 混乱(停止不可)の場合
            //if (condition.GetConditionType() == ConditionType.Confusion) {
            //    // 足踏み不可
            //    inputButtonManager.SwitchActivateSteppingButton(false);

            //    // ランダムな移動しか出来ないように入力制限する => Map側で制御

            //}
            //// 毒の場合
            //if (condition.GetConditionType() == ConditionType.Poison) {

            //    // 体力を減らす
            //    condition.ApplyEffect();

            //    // 表示更新
            //    StartCoroutine(UpdateDisplayHp(1.0f));
            //}
        }

        // 疲労の場合は攻撃力が半減(これはコンディションでの効果)

        // 病気の場合は移動速度が半減(これはコンディションでの効果)

        // 呪い(アイテム取得不可)の場合はエネミーのシンボルのみのエンカウント(これは MapController )

    }

    /// <summary>
    /// デバフの付与
    /// </summary>
    private void AddDebuff(ConditionType conditionType) {

        //ConditionData conditionData = DataBaseManager.instance.conditionDataSO.conditionDatasList.Find(x => x.conditionType == conditionType);

        //// すでに同じコンディションが付与されているか確認
        //if (mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == conditionType)) {
        //    // すでに付与されている場合は、持続時間を更新し、効果は上書きして処理を終了する
        //    mapMoveController.GetConditionsList().Find(x => x.GetConditionType() == conditionType).ExtentionCondition(conditionData.duration, conditionData.conditionValue);
        //    return;
        //}

        //// 付与するコンディションが睡眠かつ、すでに混乱のコンディションが付与されているときには、睡眠のコンディションは無視する(操作不能になるため)
        //if (conditionType == ConditionType.Sleep && mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == ConditionType.Confusion)) {
        //    return;
        //}

        //// 付与されていないコンディションの場合は、付与する準備する
        //PlayerConditionBase playerCondition;

        //// Player にコンディションを付与
        //playerCondition = conditionType switch {

        //    ConditionType.View_Wide => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
        //    ConditionType.View_Narrow => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
        //    ConditionType.Hide_Symbols => mapMoveController.gameObject.AddComponent<PlayerCondition_HideSymbol>(),
        //    ConditionType.Untouchable => mapMoveController.gameObject.AddComponent<PlayerCondition_Untouchable>(),
        //    ConditionType.Walk_through => mapMoveController.gameObject.AddComponent<PlayerCondition_WalkThrough>(),
        //    ConditionType.Sleep => mapMoveController.gameObject.AddComponent<PlayerCondition_Sleep>(),
        //    ConditionType.Confusion => mapMoveController.gameObject.AddComponent<PlayerCondition_Confusion>(),
        //    ConditionType.Curse => mapMoveController.gameObject.AddComponent<PlayerCondition_Curse>(),
        //    ConditionType.Poison => mapMoveController.gameObject.AddComponent<PlayerCondition_Poison>(),
        //    ConditionType.Disease => mapMoveController.gameObject.AddComponent<PlayerCondition_Disease>(),
        //    ConditionType.Fatigue => mapMoveController.gameObject.AddComponent<PlayerCondition_Fatigue>(),
        //    _ => null
        //};

        //// 初期設定を実行
        //playerCondition.AddCondition(conditionType, conditionData.duration, conditionData.conditionValue, mapMoveController);

        //// コンディション用の List に追加
        //mapMoveController.AddConditionsList(playerCondition);
    }

    /// <summary>
    /// レベルアップ演出
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateLebelUpEffect() {

        DebugLogger.Log("レベルアップ演出");

        yield return new WaitForSeconds(1.0f);

        GameObject levelUpLogo = Instantiate(EffectManager.instance.LevelUpLogoPrefab, overlayCanvasTran, false);
        GameObject levelUpEffect = Instantiate(EffectManager.instance.levelUpPrefab, transform.position, EffectManager.instance.levelUpPrefab.transform.rotation);
        Destroy(levelUpEffect, 2.5f);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(levelUpLogo.transform.DOLocalMoveY(325.0f, 1.0f).SetEase(Ease.OutQuart));
        sequence.Append(levelUpLogo.transform.DOShakeScale(0.15f, 0.5f, 5).SetEase(Ease.InQuart));
        sequence.AppendInterval(0.5f).OnComplete(() => { Destroy(levelUpLogo); });
    }

    /// <summary>
    /// ボス出現のエフェクト生成の準備
    /// </summary>
    /// <returns></returns>
    private void PreparateBossEffect() {
        PlayBossEffectAsync().Forget();
    }

    /// <summary>
    /// ボス出現のエフェクト
    /// </summary>
    /// <returns></returns>
    private async UniTask PlayBossEffectAsync() {
        SoundManager.instance.PlayBGM(BGM_TYPE.Boss);

        // ボス戦の前にセーブデータを作成(ItemData は別インスタンスで渡す。そうしないと、使用回数などが変化してしまう)
        List<ItemData> itemDatalist = new(PlayerInventoryManager.instance.PlayerBackPackItemList.ToList().Select(data => new ItemData(data.itemData)).ToList());
        List<int> enhanceLevelList = PlayerInventoryManager.instance.PlayerBackPackItemList.ToList().Select(data => data.EnhanceLevel.Value).ToList();
        saveData = new(GameData.instance.userData, itemDatalist, enhanceLevelList);

        // エフェクト作成して再生。再生が終わるまで処理を待機
        BossEffect bossEffect = Instantiate(EffectManager.instance.bossEffectPrefab, overlayCanvasTran, false);
        await bossEffect.PlayEffectAsync(cts.Token);

        // ボスバトルであることを記録
        //GameData.instance.isBossBattled = true;

        //PlayerPrefsHelper.SaveGameData(saveData);

        // ボス表示
        EnterBossAsync().Forget();
    }

    /// <summary>
    /// ボス表示してバトル開始
    /// </summary>
    /// <returns></returns>
    private async UniTask EnterBossAsync() {
        // シンボルの削除
        SymbolManager.instance.AllClearSymbolsList();
        bossSymbol.isSymbolTriggerd = false;
        bossSymbol.transform.position = playerMovement.transform.position;
        float scale = bossSymbol.transform.localScale.x;
        bossSymbol.transform.localScale = Vector3.zero;
        bossSymbol.gameObject.SetActive(true);

        // 画面サイズを元に戻す
        SymbolManager.instance.DefaultViewSizeProc().Forget();

        // ボス表示
        bossSymbol.transform.DOScale(Vector3.one * scale, 1.0f).SetEase(ease).SetLink(gameObject);
        await UniTask.Delay(1000, cancellationToken: cts.Token);

        SoundManager.instance.PlayVoice(VOICE_TYPE.EnterBoss);

        // バトル時間延長
        //BattleManager.instance.BattleDuration.Value = 20;
        stageUIManager.SetBossBattleTime(BattleManager.instance.bossBattleTime);

        // 一瞬待機させて OnTriggerEnter を発動させて SymbolManager にボスの情報を登録させる
        await UniTask.Delay(100, cancellationToken: cts.Token);

        // OnNext を発火して ExecuteEnemyTurnAsync を実行する
        playerMovement.EnterdBoss();
    }

    /// <summary>
    /// Overlay 設定の Canvas の情報を取得
    /// </summary>
    /// <returns></returns>
    public Transform GetOverlayCanvasTran() {
        return overlayCanvasTran;
    }
}