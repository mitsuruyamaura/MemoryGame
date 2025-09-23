using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using R3;
using DG.Tweening;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [SerializeField] private GSSReceiver gssReceiver;
    //[SerializeField] private StageGenerator stageGenerator;
    //[SerializeField] private SymbolGenerator symbolGenerator;

    //[SerializeField] private GameObject maskFieldObj;
    //[SerializeField] private GameObject imgLevelUpPrefab;

    //[SerializeField] private Transform overlayCanvasTran;
    //[SerializeField] private MoveTimeScaleController moveTimeScaleController;



    //[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private StageUIManager stageUIManager;
    //[SerializeField] private EnemySymbol bossSymbol;
    [SerializeField] private Ease ease;

    //private int levelupCount;
    //private SaveData saveData;

    public Subject<Unit> onTurnEnd = new();
    public Subject<Unit> onSuccessSettlement = new();

    private TurnState currentTurnState = TurnState.None;

    public TurnState CurrentTurnState {
        set => currentTurnState = value;
        get => currentTurnState;
    }

    private CancellationTokenSource cts;


    private void Start() {
        SetupGameLogic().Forget();
    }


    private async UniTask SetupGameLogic() {
        SoundManager.instance.PlayBGM(BGM_TYPE.Stage_0);

        var token = this.GetCancellationTokenOnDestroy();
        cts = new CancellationTokenSource();

        // TODO Player の Hp 設定


        await gssReceiver.PrepareGSSLoadStartAsync();


        BattleManager.instance.SetUp(stageUIManager);

        // SO のデータを取得するまで待機
        //await UniTask.WaitUntil(() => GSSReceiver.IsLoading);

        // デバッグ用。Title が追加されたら使わない
        //SceneStateManager.instance.PreparateStageSceneAsync();

        // ステージのランダム作成(StageData 作成後は StageData に登録されている StageType を渡す)
        //stageGenerator.GenerateStageFromRandomTiles(GameData.instance.currentStageData.stageType);

        // 通常のシンボルのランダム作成して List に追加
        //SymbolManager.instance.AllClearSymbolsList();

        //symbolGenerator.SetUp();
        //SymbolManager.instance.SymbolsList = symbolGenerator.GenerateSymbols(-1, stageGenerator.TilemapColision, stageGenerator.TilemapWalk, stageGenerator.terrainInfoDic);

        // 特殊シンボルのランダム作成して List に追加
        //SymbolManager.instance.SymbolsList.AddRange(stageGenerator.GenerateSpecialSymbols(GameData.instance.currentStageData.orbTypes));

        // 全シンボルの設定
        //SymbolManager.instance.SetUpAllSymbols();

        // スタミナの値をステージごとの初期値に設定(StageData 作成後)
        //GameData.instance.userData.Stamina.Value = GameData.instance.currentStageData.initStamina;

        // UI 初期設定
        stageUIManager?.SetupStageUIManager(GameData.instance.userData.Stamina.Value, GameData.instance.charaStatus.MaxHp.Value);

        // スタミナの値の購読開始
        //GameData.instance.userData.Stamina.Subscribe(stamina => stageUIManager.UpdateDisplayStaminaPoint(stamina));

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
        //DataBaseManager.instance.CreateDropItemDatasList(GameData.instance.currentStageData.dropTreasureLevel);

        //playerMovement.SetUp();
        //playerMovement.MovePlayerProc();
        //playerMovement.SetTerrainInfo(stageGenerator.terrainInfoDic);

        //playerMovement.OnPlayerMooveComplete.Subscribe(_ => ExecuteEnemyTurnAsync().Forget());

        FloatingViewGenerator.instance?.SetUp(gameObject);

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

        //EnemyInfoDisplayManager.instance.Setup(stageUIManager.enemyBackPackItemTran);

        //GameData.instance.userData.WalkCount.Subscribe(walkCount => 
        //{
        //    int waveNo = DataBaseManager.instance.GetWaveCount(walkCount, GameData.instance.userData.waveCount);
        //    if(waveNo > GameData.instance.userData.waveCount) {
        //        GameData.instance.userData.waveCount = waveNo;

        //        // シンボルの削除と再作成

        //    }
        //}).AddTo(this);

        //stageUIManager.SetSliderWalkCount(ConstData.WAVE_WALK_COUNT);  // GameData.instance.GetMaxWaveCount(GameData.instance.userData.waveNo)

        // 歩数の購読１。歩数表示更新
        //GameData.instance.userData.WalkCount
        //    .Where(_ => GameData.instance.userData.waveNo < 10)
        //    .Subscribe(walkCount => {
        //        stageUIManager.UpdateWalkCount(walkCount);

        //        // 次の Wave まで残り5歩(45/95/145/195..)になったら表示(バトルのエフェクトと重なる場合、こちらを優先)
        //        int offsetCount = (DataBaseManager.instance.GetWaveNo(walkCount, GameData.instance.userData.waveNo) - 1) * ConstData.WAVE_WALK_COUNT;
        //        if (walkCount != 0 && walkCount % (ConstData.WAVE_INFO_COUNT + offsetCount) == 0) {
        //            stageUIManager.NextWaveInfo();
        //        }
        //    }).AddTo(this);

        //// 歩数の購読２。歩数による WaveNo の更新と次の Wave 用のステージ、シンボルの作成
        //GameData.instance.userData.WalkCount
        //    .Select(walkCount => {
        //        int waveNo = DataBaseManager.instance.GetWaveNo(walkCount, GameData.instance.userData.waveNo);
        //        return new { walkCount, waveNo };
        //    })
        //    .Where(result => result.waveNo > GameData.instance.userData.waveNo)
        //    .Subscribe(async result => {

        //        //GameData.instance.gameState.Value = GameData.GameState.Battle;
        //        //await ExecuteEnemyTurnAsync();
        //        //await UniTask.WaitUntil(() => GameData.instance.gameState.Value == GameData.GameState.Play);
        //        GameData.instance.gameState.Value = GameData.GameState.Prepare;

        //        DebugLogger.Log($"WaveNo 更新 : { result.waveNo}");
        //        GameData.instance.userData.waveNo = result.waveNo;
        //        stageUIManager.UpdateWaveNo(GameData.instance.userData.waveNo);

        //        SceneStateManager.instance.FadeIn().Forget();
        //        await UniTask.Delay(1000, cancellationToken:token);

        //        // Wave のデータをもらう
        //        WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);
        //        stageGenerator.SetTilemapData(waveData);

        //        // タイル削除して新しく生成
        //        stageGenerator.GenerateStageFromRandomTiles(GameData.instance.currentStageData.stageType);

        //        // ランダムな SymbolRate と Wave のデータをもらう

        //        // シンボルの削除と再作成処理
        //        SymbolManager.instance.AllClearSymbolsList();
        //        SymbolManager.instance.SymbolsList = symbolGenerator.GenerateSymbols(-1, stageGenerator.TilemapColision, stageGenerator.TilemapWalk, stageGenerator.terrainInfoDic);
        //        SymbolManager.instance.SetUpAllSymbols();

        //        stageUIManager.ResetSliderWalkCount();
        //        stageUIManager.SetSliderWalkCount(ConstData.WAVE_WALK_COUNT);  // GameData.instance.GetMaxWaveCount(GameData.instance.userData.waveNo)

        //        // Player の移動用データ更新
        //        playerMovement.SetTerrainInfo(stageGenerator.terrainInfoDic);

        //        await UniTask.Delay(1500, cancellationToken: token);
        //        SymbolManager.instance.ResetEvent();
        //        //SceneStateManager.instance.FadeOut();
        //        GameData.instance.gameState.Value = GameData.GameState.Play;
        //    })
        //    .AddTo(this);

        //// ボスの設定
        //bossSymbol.OnEnterSymbol();

        GameData.instance.gameState.Value = GameData.GameState.Play;
    }

    //public async UniTask ExecuteEnemyTurnAsync() {
    //    GameData.instance.gameState.Value = GameData.GameState.Battle;
    //    //while (CurrentTurnState == TurnState.Enemy) {    // あとで GameState に変える

    //    //if (CurrentTurnState == TurnState.Enemy) {
    //    //Debug.Log("敵の移動　開始");
    //    //yield return StartCoroutine(symbolManager.EnemisMove());

    //    //Debug.Log("すべての敵の移動 完了");

    //    // バトルイベントを発生させる
    //    //BattleResultType battleResultType = await SymbolManager.instance.ExecuteEnemySymbolEventAsync(CurrentTurnState, cts.Token);

    //    //DebugLogger.Log(battleResultType.ToString());

    //    if (CurrentTurnState == TurnState.Boss) {
    //        if (battleResultType == BattleResultType.Win) {
    //            SoundManager.instance.PlayBGM(BGM_TYPE.Clear);
    //            DebugLogger.Log("ゲームクリア");

    //            // データセーブ
    //            //PlayerPrefsHelper.ConditionalSave(saveData);

    //            await UniTask.Delay(1000, cancellationToken: cts.Token);
    //            SoundManager.instance.PlayVoice(VOICE_TYPE.Win);

    //            stageUIManager.ShowRestartMessage();

    //            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);

    //            // TODO 一旦タイトルへ
    //            SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
    //        } else {
    //            DebugLogger.Log("ゲームオーバー");

    //            SoundManager.instance.PlayVoice(VOICE_TYPE.Loose);
    //            stageUIManager.ShowRestartMessage();

    //            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);
    //            SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
    //        }
    //        return;
    //    }

    //    // コンディションの残り時間を減算するためのイベントを発火(ここで減算しないと追加時に1回分すぐに消費してしまうため)
    //    SymbolManager.instance.onTurnEnd.OnNext(default);

    //    // バトル以外のイベントを発生させる
    //    await SymbolManager.instance.TriggerEventProc();

    //    GameData.instance.gameState.Value = GameData.GameState.Play;
    //    GameData.instance.userData.WalkCount.Value++;
    //    SymbolManager.instance.ResetEvent();

    //    // 直前にスタミナ回復を取っている場合にはボスにならないことを確認済
    //    // ターンの状態を確認
    //    if (battleResultType != BattleResultType.Lose) {
    //        // スタミナが 0 の場合、TurnState を Boss に切り替える
    //        CheckTurn();
    //        //CheckTreasureBox();
    //    } else if (battleResultType == BattleResultType.Lose) {
    //        DebugLogger.Log("ゲームオーバー");

    //        SoundManager.instance.PlayVoice(VOICE_TYPE.Loose);
    //        stageUIManager.ShowRestartMessage();

    //        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);
    //        SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
    //        return;
    //    }

    //    if (CurrentTurnState == TurnState.Boss) {
    //        DebugLogger.Log("Boss 出現");

    //        // ボスの出現演出
    //        //PreparateBossEffect();
    //        return;
    //    }
    //    await UniTask.Delay(50, cancellationToken: cts.Token);
    //    DebugLogger.Log("Player 移動再開");

    //    // プレイヤーの移動再開
    //    //playerMovement.MovePlayerProc();
    //}

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
        //maskFieldObj.SetActive(isSwitch);
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
}