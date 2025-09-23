using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;
using System;
using Unity.Cinemachine;
using R3.Triggers;

public enum EntityType {
    Player,
    Enemy
}


public enum BattleResultType {
    Win,
    Lose,
    Timeout,
    Ready,
    Battle,    
    Runaway
}

public class BattleManager : AbstractSingleton<BattleManager> {

    public RectTransform playerFloatingViewTran;
    public RectTransform enemyFloatingViewTran;

    public SerializableReactiveProperty<int> PlayerHP = new ();
    public SerializableReactiveProperty<int> EnemyHP = new ();
    public SerializableReactiveProperty<float> BattleDuration = new(5.0f); // バトルの制限時間（秒）

    private CancellationTokenSource cts;
    public CancellationTokenSource Cts => cts;

    // プレイヤー用のアイテムリスト
    public List<BackPackInItem> playerBackPackItemList = new List<BackPackInItem>();

    // 敵用のアイテムリスト
    public List<BackPackInItem> enemyBackPackItemList = new List<BackPackInItem>();

    [Header("デバッグ"), Space(2)]
    [SerializeField] private List<int> playerItemNoList = new();
    [SerializeField] private List<int> enemyItemNoList = new();

    [SerializeField] private CinemachineCamera virtualCamera;
    public CinemachineCamera VirtualCamera => virtualCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    
    [SerializeField] private GameObject battleEffectSetObj;

    [SerializeField] private float amplitude = 1.0f, frequency = 1.0f;

    public Subject<(CancellationTokenSource, BattleResultType)> OnBattleStart = new();
    public Subject<BattleResultType> OnBattleEnd = new();
    private Channel<BattleResultType> channel;
    private CompositeDisposable enemyDisposables;
    private BattleResultType battleResultType;

    public SerializableReactiveProperty<int> PlayerShieldHP = new ();
    public SerializableReactiveProperty<int> EnemyShieldHP = new ();
    [SerializeField] private int maxShield;
    [SerializeField] private float defaultBattleTime = 5.0f;
    public float bossBattleTime = 20.0f;
    public StageUIManager stageUIManager;

    private int enemyMaxHp = 0;


    protected override void Awake() {
        base.Awake();

        if (virtualCamera != null) {
            virtualCameraNoise = (CinemachineBasicMultiChannelPerlin)virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise);
            //battleEffectSetObj = virtualCamera.transform.GetChild(2).gameObject;

            if(virtualCameraNoise != null) {
                StopBattleShake();
            }
        }   
    }

    //private void OnEnable() {
    //    PlayerHP.Subscribe(hp => 
    //    {
    //        // UI 更新

    //        CheckEndCondition();
    //    });

    //    EnemyHP.Subscribe(hp => CheckEndCondition());
    //}

    //void Start() {
    //    SetUp();
    //}

    public void SetUp(StageUIManager stageUIManager) {
        cts = new CancellationTokenSource();

        this.stageUIManager = stageUIManager;

        // デバッグ用
        //if (playerItemNoList.Count > 0) {
        //    for (int i = 0; i < playerItemNoList.Count; i++) {
        //        ItemData itemData = DataBaseManager.instance.GetItemData(playerItemNoList[i]);
        //        playerBackPackItemList[i].ItemData.Value = itemData;
        //    }
        //}

        // バトルのたびにセットではなく、アイテム取得時にセットし、OnNext でスタートさせる
        // プレイヤー用のアイテムを処理
        //playerBackPackItemList.ForEach(item =>
        //{
        //    item.SetUpBackPackItem();

        //    // 所持アイテムの最新情報を渡す
        //    OnBattleStart.Subscribe(_ => item.ExecuteBackPackItem(item.itemData, cts.Token, EntityType.Player).Forget());
        //});



        // 敵用のアイテムを処理
        //enemyBackPackItemList.ForEach(item => item.Hoge(item.itemData, cts.Token, EntityType.Enemy).Forget());

        UpdatePlayerHp(GameData.instance.debugMaxHp, EffectType.Heal, false, false);

        PlayerHP
            .Zip(PlayerHP.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(hp => {
                // UI 更新
                stageUIManager.UpdateDisplayPlayerHp(hp.oldValue, hp.newValue);
                CheckEndCondition();
            });

        PlayerShieldHP
            .Subscribe(shield => {
                // UI 更新
                stageUIManager.UpdatePlayerShieldHp(shield);
            }).AddTo(this);
    }

    /// <summary>
    /// バトル開始
    /// </summary>
    /// <param name="enemySymbol"></param>
    /// <returns></returns>
    public async UniTask<BattleResultType> StartBattle(EnemyData enemyData, TurnState turnState) {
        GameData.instance.gameState.Value = GameData.GameState.Battle;

        // バトル時間設定
        if(turnState == TurnState.Boss) {
            BattleDuration.Value = bossBattleTime;
        } else {
            BattleDuration.Value = defaultBattleTime;
        }

        // 新しいトークンソースを生成
        // 初期化しないと Cancel 状態のままのトークンを利用してしまうため、BackPackItem の処理がスキップされてしまう
        cts = new CancellationTokenSource();
        enemyDisposables = new();

        // 敵の装備情報を取得
        List<int> equipItemNoList = enemyData.GetEquipItemNoList();

        EnemyInfoDisplayManager.instance.ShowEnemyInfo(enemyData, equipItemNoList, GameData.GameState.Battle);
        EnemyInfoDisplayManager.instance.NoShadeEnemy();

        enemyMaxHp = enemyData.hp;

        // 敵の Hp 表示更新
        EnemyHP.Value = enemyData.hp;

        EnemyHP
            .Zip(EnemyHP.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(hp => {
                EnemyInfoDisplayManager.instance.UpdateDisplayEnemyHp(hp.oldValue, hp.newValue);
                CheckEndCondition();
            }).AddTo(enemyDisposables);

        // 敵のシールド 表示更新
        EnemyShieldHP.Value = enemyData.shieldPower;

        EnemyShieldHP
            .Subscribe(shield => {
                // UI 更新
                EnemyInfoDisplayManager.instance.UpdateEnemyShieldHp(shield);
             }).AddTo(enemyDisposables);

        battleResultType = BattleResultType.Battle;

        // 都度 cts を渡さないと外部では連動して停止しない
        OnBattleStart.OnNext((cts, battleResultType));
        //Debug.Log("StartBattle");

        // バトルエフェクトの表示と再生
        battleEffectSetObj.SetActive(true);
        StartBattleShake(amplitude, frequency);

        // バトル制限時間を管理するタスクを開始
        ManageBattleDuration((int)BattleDuration.Value, cts.Token).Forget();

        float interval = 0.2f; // SEを再生する間隔（秒）
        float timer = 0; ;

        // バトル時間の計測開始
        IDisposable updateSubscribe = this.UpdateAsObservable()
            .Where(_ => !cts.IsCancellationRequested)
            .Where(_ => BattleDuration.Value > 0)
            .Subscribe(_ => {
                BattleDuration.Value -= Time.deltaTime;
                timer += Time.deltaTime;

                if (timer >= interval) {
                    timer = 0;

                    int seIndex = UnityEngine.Random.Range(0, 5);
                    // SEを再生
                    SoundManager.instance.PlaySE((SE_TYPE)seIndex);
                    DebugLogger.Log("SE");
                }
            });

        channel = Channel.CreateSingleConsumerUnbounded<BattleResultType>();
        battleResultType = await channel.Reader.ReadAsync();

        channel.Writer.TryComplete();
        updateSubscribe?.Dispose();

        await UniTask.Delay(1000);

        EnemyInfoDisplayManager.instance.HideEnemyInfo();

        PlayerShieldHP.Value = 0;
        EnemyShieldHP.Value = 0;

        //updateSubscribe?.Dispose();

        //GameData.instance.gameState.Value = GameData.GameState.Play;
        return battleResultType;
    }

    /// <summary>
    /// バトル停止
    /// </summary>
    /// <param name="battleResultType"></param>
    public void StopBattle(BattleResultType battleResultType) {
        if (cts != null) {
            cts.Cancel();  // すべての Hoge メソッドの実行をキャンセルする
            cts.Dispose(); // 古いトークンソースを破棄
            cts = null;    // 参照をクリア
            DebugLogger.Log("Cancel");
        }
        DebugLogger.Log(battleResultType);
        cts = new CancellationTokenSource();
        OnBattleEnd.OnNext(battleResultType);

        enemyDisposables?.Clear();
        EnemyHP.Value = 0;

        // カメラとマスクの制御　元に戻す
        StopBattleShake();

        // バトルエフェクト非表示
        battleEffectSetObj.SetActive(false);

        channel.Writer.TryWrite(battleResultType);
    }


    private async UniTask ManageBattleDuration(int battleTime, CancellationToken token) {
        try {
            // キャンセルされない限り
            while (!cts.IsCancellationRequested) {
                await UniTask.Delay((int)(battleTime * 1000), cancellationToken: token);
                StopBattle(BattleResultType.Timeout);
            }
        }
        catch (OperationCanceledException) {
            // バトルが手動でキャンセルされた時もここに来る
            DebugLogger.Log("バトル キャンセル");
        }
    }

    /// <summary>
    /// プレイヤーのシールド値更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdatePlayerShieldHp(int amount, bool isCritical) {
        //if (GameData.instance.gameState.Value == GameData.GameState.Play)
        //{
        //    return;            
        //}
        PlayerShieldHP.Value = Mathf.Min(PlayerShieldHP.Value + amount, maxShield);

        FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(playerFloatingViewTran);
        FloatingViewType floatingViewType = isCritical == true ? FloatingViewType.critical : FloatingViewType.shield;
        floatingView.SetColor(floatingViewType);
        floatingView.SetViewFontSize(floatingViewType);
        floatingView.UpdateText(amount.ToString()).Forget();
    }

    /// <summary>
    /// 敵のシールド値更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateEnemyShieldHp(int amount, bool isCritical) {
        //if (GameData.instance.gameState.Value == GameData.GameState.Play) {
        //    return;
        //}
        EnemyShieldHP.Value = Mathf.Min(EnemyShieldHP.Value + amount, maxShield);

        FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(enemyFloatingViewTran);
        FloatingViewType floatingViewType = isCritical == true ? FloatingViewType.critical : FloatingViewType.shield;
        floatingView.SetColor(floatingViewType);
        floatingView.SetViewFontSize(floatingViewType);
        floatingView.UpdateText(amount.ToString()).Forget();
    }

    /// <summary>
    /// プレイヤーの Hp 更新
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="effectType"></param>
    /// <param name="isfloatViewOn"></param>
    public void UpdatePlayerHp(int amount, EffectType effectType, bool isCritical, bool isfloatViewOn = true) {
        //if (GameData.instance.gameState.Value == GameData.GameState.Play) {
        //    return;
        //}

        // 物理攻撃の場合のみシールドを使う
        if (effectType == EffectType.Physical) {
            PlayerShieldHP.Value = Mathf.Max(PlayerShieldHP.Value + amount, 0);
            amount = Mathf.Min(amount + PlayerShieldHP.Value, 0);
        }

        // Passive 装備をうしなった場合、すでに Hp 最大値は減らしてあるので、下の処理で現在値を制限する(二重で減算しない)
        if (effectType == EffectType.Passive && amount <= 0) {
            amount = 0;
        }

        // Hp 計算と演出
        PlayerHP.Value = Mathf.Clamp(PlayerHP.Value + amount, 0, GameData.instance.charaStatus.MaxHp.Value);

        if (!isfloatViewOn) return;

        FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(playerFloatingViewTran);
        FloatingViewType floatingViewType = isCritical == true ? FloatingViewType.critical : effectType == EffectType.Heal ? FloatingViewType.heal : FloatingViewType.normalDamage;
        floatingView.SetColor(floatingViewType);
        floatingView.SetViewFontSize(floatingViewType);
        floatingView.UpdateText(amount.ToString()).Forget();
    }

    /// <summary>
    /// 敵の Hp 更新
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="effectType"></param>
    /// <param name="isfloatViewOn"></param>
    public void UpdateEnemyHp(int amount, EffectType effectType, bool isCritical, bool isfloatViewOn = true) {
        //if (GameData.instance.gameState.Value == GameData.GameState.Play) {
        //    return;
        //}

        // 物理攻撃の場合のみシールドを使う
        if (effectType == EffectType.Physical) {
            EnemyShieldHP.Value = Mathf.Max(EnemyShieldHP.Value + amount, 0);
            amount = Mathf.Min(amount + EnemyShieldHP.Value, 0);
        }

        EnemyHP.Value = Mathf.Clamp(EnemyHP.Value + amount, 0, enemyMaxHp);

        if (!isfloatViewOn) return;

        FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(enemyFloatingViewTran);
        FloatingViewType floatingViewType = isCritical == true ? FloatingViewType.critical : effectType == EffectType.Heal ? FloatingViewType.heal : FloatingViewType.normalDamage;
        floatingView.SetColor(floatingViewType);
        floatingView.SetViewFontSize(floatingViewType);
        floatingView.UpdateText(amount.ToString()).Forget();
    }

    /// <summary>
    /// バトル終了の状態か判定
    /// 現在戻り値未使用
    /// </summary>
    /// <returns></returns>
    public bool CheckEndCondition() {
        if (battleResultType != BattleResultType.Battle) {
            return true;
        }

        if (PlayerHP.Value <= 0) {
            StopBattle(BattleResultType.Lose);
            return true;
        } else if (EnemyHP.Value <= 0) {
            StopBattle(BattleResultType.Win);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 逃亡
    /// </summary>
    private void RunawayBattle() {
        StopBattle(BattleResultType.Runaway);
    }

    /// <summary>
    /// ダメージを与えるタイミングで揺らすか、ずっと揺らすか検討する
    /// カエルの為は、ダメージのタイミングのみ
    /// </summary>
    /// <param name="amplitude"></param>
    /// <param name="frequency"></param>
    public void StartBattleShake(float amplitude, float frequency) {
        if (virtualCameraNoise != null) {
            virtualCameraNoise.AmplitudeGain = amplitude;
            virtualCameraNoise.FrequencyGain = frequency;
        }
    }

    public void StopBattleShake() {
        if (virtualCameraNoise != null) {
            virtualCameraNoise.AmplitudeGain = 0f;
            virtualCameraNoise.FrequencyGain = 0f;
        }
    }
}