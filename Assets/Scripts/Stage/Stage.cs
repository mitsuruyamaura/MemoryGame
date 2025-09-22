using System.Collections;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Coffee.UIExtensions;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Stage : MonoBehaviour {

    //[SerializeField]
    //private Text txtStaminaPoint;

    //[SerializeField]
    //private Image[] imgOrbs;

    //[SerializeField]
    //private Slider sliderHp;

    //[SerializeField]
    //private Text txtHp;

    //[SerializeField]
    //private Text txtExp;

    //[SerializeField]
    //private Text txtPlayerLevel;

    //[SerializeField]
    //private ShinyEffectForUGUI shinyEffectImgPlayerLevelFrame;

    //[SerializeField]
    //private Slider sliderExp;

    //[SerializeField]
    //private StageGenerator stageGenerator;

    //[SerializeField]
    //private SymbolManager symbolManager;

    //[SerializeField]
    //private InputButtonManager inputButtonManager;

    //[SerializeField]
    //private MapMoveController mapMoveController;

    //[SerializeField]
    //private Button btnPlayerLevel;

    //[SerializeField]
    //private GameObject maskFieldObj;

    //[SerializeField]
    //private SelectAbilityPopUp selectAbilityPopUpPrefab;

    //[SerializeField]
    //private Transform canvasTran;

    //private SelectAbilityPopUp selectAbilityPopUp;


    //private float sliderAnimeDuration = 0.5f;

    //int levelupCount;

    //public enum TurnState {
    //    None,
    //    Player,
    //    Enemy,
    //    Boss
    //}

    //private TurnState currentTurnState = TurnState.None;

    //public TurnState CurrentTurnState
    //{
    //    set => currentTurnState = value;
    //    get => currentTurnState;
    //}

    //[SerializeField] private GameObject imgLevelUpPrefab;

    //[SerializeField] private Transform overlayCanvasTran;

    //[SerializeField] private MoveTimeScaleController moveTimeScaleController;

    //[SerializeField] private PlayerMovement playerMovement;


    //void Start() {
    //    // Stage の情報設定
    //    SceneStateManager.instance.stage = this;

    //    // ステージのランダム作成(StageData 作成後は StageData に登録されている StageType を渡す)
    //    stageGenerator.GenerateStageFromRandomTiles(GameData.instance.currentStageData.stageType);

    //    // 通常のシンボルのランダム作成して List に追加
    //    symbolManager.AllClearSymbolsList();
    //    symbolManager.SymbolsList = stageGenerator.GenerateSymbols(-1);

    //    // 特殊シンボルのランダム作成して List に追加
    //    symbolManager.SymbolsList.AddRange(stageGenerator.GenerateSpecialSymbols(GameData.instance.currentStageData.orbTypes));

    //    // 全シンボルの設定
    //    symbolManager.SetUpAllSymbols();

    //    // スタミナの値をステージごとの初期値に設定(StageData 作成後)
    //    //GameData.instance.staminaPoint.Value = GameData.instance.currentStageData.initStamina;

    //    // スタミナの値の購読開始
    //    GameData.instance.userData.Stamina.Subscribe(_ => UpdateDisplayStaminaPoint());

    //    // 一旦、獲得したオーブの情報を非表示
    //    //for (int i = 0; i < imgOrbs.Length; i++) {
    //    //    imgOrbs[i].enabled = false;
    //    //}

    //    // オーブの情報作成
    //    //for (int i = 0; i < GameData.instance.currentStageData.orbTypes.Length; i++) {
    //    //    imgOrbs[i].enabled = true;
    //    //    imgOrbs[i].sprite = DataBaseManager.instance.orbDataSO.orbDatasList.Find(x => x.orbType == symbolManager.specialSymbols[i].orbType).spriteOrb;
    //    //    GameData.instance.orbs.Add(i, false);
    //    //}

    //    // オーブの購読開始
    //    //GameData.instance.orbs.ObserveReplace().Subscribe((DictionaryReplaceEvent<int, bool> x) => UpdateDisplayOrbs(x.Key, x.NewValue));

    //    //GameData.instance.maxHp = GameData.instance.hp;

    //    GameData.instance.InitPlayerCombatData();


    //    // Hp表示更新
    //    //StartCoroutine(UpdateDisplayHp());

    //    // プレイヤーレベルと経験値の表示更新
    //    //UpdateDisplayPlayerLevel();
    //    //UpdateDisplayExp(true);

    //    // プレイヤーの設定
    //    //mapMoveController.SetUpMapMoveController(this);
    //    //inputButtonManager.SetUpInputButtonManager(mapMoveController);

    //    CurrentTurnState = TurnState.Player;

    //    // プレイヤーの移動の監視(OnEnable でやっている)
    //    //StartCoroutine(ObserveEnemyTurnState());

    //    symbolManager.SwitchEnemyCollider(true);

    //    // アビリティ選択用ウインドウの生成
    //    //CreateSelectAbilityPopUp();

    //    //btnPlayerLevel.onClick.AddListener(OnClickPlayerLevel);
    //    //moveTimeScaleController.SetUpMoveButtonController();

    //    // ドロップするトレジャーの情報を準備
    //    DataBaseManager.instance.CreateDropItemDatasList(GameData.instance.currentStageData.dropTreasureLevel);

    //    playerMovement.SetUp();
    //    playerMovement.MovePlayerProc();

    //    playerMovement.OnPlayerMooveComplete.Subscribe(_ => ExecuteEnemyTurnAsync().Forget());
    //}

    ///// <summary>
    ///// エネミーのターン経過監視処理
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator ObserveEnemyTurnState() {
    //    //while (CurrentTurnState == TurnState.Enemy) {    // あとで GameState に変える

    //    //if (CurrentTurnState == TurnState.Enemy) {
    //    Debug.Log("敵の移動　開始");
    //    yield return StartCoroutine(symbolManager.EnemisMove());

    //    Debug.Log("すべての敵の移動 完了");

    //    // シンボルのイベントを発生させる
    //    bool isEnemyTriggerEvent = mapMoveController.CallBackEnemySymbolTriggerEvent();

    //    Debug.Log(isEnemyTriggerEvent);

    //    // ターンの状態を確認
    //    if (!isEnemyTriggerEvent) {
    //        CheckTurn();
    //        CheckTreasureBox();
    //    }

    //    if (CurrentTurnState == TurnState.Boss) {

    //        // ボスの出現
    //        Debug.Log("Boss 出現");

    //        // TODO 演出
    //        PreparateBossEffect();
    //    }
    //    //}

    //    //yield return null;
    //    //}
    //}

    //public async UniTask ExecuteEnemyTurnAsync() {
    //    //while (CurrentTurnState == TurnState.Enemy) {    // あとで GameState に変える

    //    //if (CurrentTurnState == TurnState.Enemy) {
    //    Debug.Log("敵の移動　開始");
    //    //yield return StartCoroutine(symbolManager.EnemisMove());

    //    Debug.Log("すべての敵の移動 完了");

    //    // シンボルのイベントを発生させる
    //    bool isEnemyTriggerEvent = await mapMoveController.ExecuteSymbolEventAsync();

    //    Debug.Log(isEnemyTriggerEvent);

    //    // ターンの状態を確認
    //    if (!isEnemyTriggerEvent) {
    //        CheckTurn();
    //        CheckTreasureBox();
    //    }

    //    if (CurrentTurnState == TurnState.Boss) {

    //        // ボスの出現
    //        Debug.Log("Boss 出現");

    //        // TODO 演出
    //        PreparateBossEffect();
    //    }
    //    //}

    //    //yield return null;
    //    //}

    //    // TODO プレイヤーの移動再開
    //    playerMovement.MovePlayerProc();
    //}


    ///// <summary>
    ///// スタミナポイントの表示更新
    ///// </summary>
    //private void UpdateDisplayStaminaPoint(int stamina) {

    //    // TODO スタミナ回復をアニメさせたい
    //    //txtStaminaPoint.DOCounter(GameData.instance.staminaPoint, 1.0f).SetEase(Ease.Linear);

    //    txtStaminaPoint.text = stamina.ToString();

    //    if (stamina <= 0) {
    //        //Debug.Log("ボス戦");

    //        // 購読停止
    //        //GameData.instance.staminaPoint.Dispose();

    //        //GameData.instance.orbs.Dispose();


    //        // 移動禁止


    //        // TODO ボスとのバトルシーンへ遷移


    //        Debug.Log("ゲーム終了");
    //    }
    //}

    ///// <summary>
    ///// 取得しているオーブの表示更新
    ///// </summary>
    ///// <param name="index"></param>
    ///// <param name="isSwich"></param>
    //public void UpdateDisplayOrbs(int index, bool isSwich) {

    //    Debug.Log(index);
    //    Debug.Log(isSwich);

    //    imgOrbs[index].color = isSwich ? Color.white : new Color(1, 1, 1, 0.5f);

    //    // 獲得した場合
    //    if (isSwich) {
    //        // 光る演出を再生
    //        imgOrbs[index].gameObject.GetComponent<ShinyEffectForUGUI>().Play();
    //    }
    //}

    ///// <summary>
    ///// Hp表示更新
    ///// </summary>
    ///// <param name="waitTime"></param>
    ///// <returns></returns>
    //public IEnumerator UpdateDisplayHp(float waitTime = 0.0f) {
    //    txtHp.text = GameData.instance.hp + "/ " + GameData.instance.maxHp;

    //    yield return new WaitForSeconds(waitTime);

    //    sliderHp.DOValue((float)GameData.instance.hp / GameData.instance.maxHp, sliderAnimeDuration).SetEase(Ease.Linear);

    //    Debug.Log("Hp 表示更新");
    //}

    //private void OnEnable() {
    //    Debug.Log("OnEneble");

    //    // バトル前の Hp からアニメして表示するために待機時間を作る
    //    StartCoroutine(UpdateDisplayHp(1.0f));

    //    // バトル後にレベルアップした時のカウントの初期化
    //    levelupCount = 0;

    //    // レベルアップするか確認
    //    CheckExpNextLevel();

    //    // レベルアップしていたら
    //    if (levelupCount > 0) {

    //        Debug.Log("レベルアップのボーナス発生");

    //        // レベルアップのボーナス


    //        // レベルアップ演出
    //        StartCoroutine(GenerateLebelUpEffect());
    //    }

    //    // デバッグ  レベルアップ演出
    //    //GenerateLebelUpEffect();

    //    //// バトルから戻った場合
    //    //if (CurrentTurnState == TurnState.Enemy) {
    //    //    // プレイヤーの番にする
    //    //    CurrentTurnState = TurnState.Player;
    //    //}

    //    // バトルで付与されたデバフの確認と付与
    //    CheckDebuffConditions();

    //    // ターンの確認とプレイヤーのターンに切り替え。コンディションの更新
    //    CheckTurn();

    //    // オーブを獲得している場合は獲得処理を実行
    //    CheckOrb();

    //    // トレジャーボックスをエネミーの下に置いた場合に使う
    //    //CheckTreasureBox();

    //    //if (CurrentTurnState == TurnState.Player) {

    //    //    // プレイヤーの移動の監視再開
    //    //    StartCoroutine(ObserveEnemyTurnState());
    //    //} else
    //    if (CurrentTurnState == TurnState.Boss) {

    //        // ボスの出現
    //        Debug.Log("Boss 出現");


    //        // TODO 演出
    //        PreparateBossEffect();
    //    }
    //}

    ///// <summary>
    ///// レベルアップするか確認
    ///// </summary>
    //public void CheckExpNextLevel() {

    //    // 現在の経験値と次のレベルに必要な経験値を比べて、レベルが上がるか確認
    //    if (GameData.instance.totalExp < DataBaseManager.instance.CalcNextLevelExp(GameData.instance.playerLevel - 1)) {
    //        // 達していない場合には経験値とゲージ更新
    //        UpdateDisplayExp(true);

    //        // 処理終了
    //        return;
    //    } else {
    //        // 達している場合にはレベルアップ
    //        GameData.instance.playerLevel++;
    //        levelupCount++;

    //        // アビリティポイント加算
    //        GameData.instance.AddAbilityPoint();

    //        Debug.Log("レベルアップ！ 現在のレベル : " + GameData.instance.playerLevel);

    //        // レベルアップ演出
    //        shinyEffectImgPlayerLevelFrame.Play();

    //        // プレイヤーレベルと経験値の表示更新
    //        UpdateDisplayPlayerLevel();
    //        UpdateDisplayExp(false);

    //        // さらにレベルが上がるか再帰処理を行って確認
    //        CheckExpNextLevel();
    //    }
    //}

    ///// <summary>
    ///// プレイヤーレベルの表示更新
    ///// </summary>
    //private void UpdateDisplayPlayerLevel() {
    //    // プレイヤーレベルの表示更新
    //    txtPlayerLevel.text = GameData.instance.playerLevel.ToString();
    //}

    ///// <summary>
    ///// 経験値の表示更新
    ///// </summary>
    ///// <param name="isSliderOn"></param>
    //private void UpdateDisplayExp(bool isSliderOn) {
    //    // 現在/目標経験値の表示更新
    //    txtExp.text = GameData.instance.totalExp + " / " + DataBaseManager.instance.CalcNextLevelExp(GameData.instance.playerLevel - 1);

    //    if (isSliderOn) {
    //        // ゲージ更新
    //        sliderExp.DOValue((float)GameData.instance.totalExp / DataBaseManager.instance.CalcNextLevelExp(GameData.instance.playerLevel - 1), 1.0f).SetEase(Ease.Linear);
    //    }
    //}

    ///// <summary>
    ///// ターンの確認。プレイヤーのターンに切り替え。コンディションの更新
    ///// </summary>
    //private void CheckTurn() {
    //    if (GameData.instance.staminaPoint.Value <= 0) {
    //        CurrentTurnState = TurnState.Boss;
    //    } else {
    //        CurrentTurnState = TurnState.Player;

    //        // コンディションの残り時間の更新(いまは MapController 内の２箇所でやっているので、ここで一本化する)
    //        mapMoveController.UpdateConditionsDuration();

    //        // 移動ボタンと足踏みボタンを押せる状態にする
    //        ActivateInputButtons();

    //        // コンディションの効果を適用
    //        ApplyEffectConditions();

    //        mapMoveController.IsMoving = false;

    //        // プレイヤーの移動の監視再開
    //        //StartCoroutine(ObserveEnemyTurnState());
    //    }
    //    Debug.Log(CurrentTurnState);
    //}

    ///// <summary>
    ///// オーブのイベントが登録されているか確認して、登録されている場合には実行
    ///// </summary>
    //private void CheckOrb() {
    //    mapMoveController.CallBackOrbSymbolTriggerEvent();
    //}

    ///// <summary>
    ///// トレジャーボックスのイベントが登録されているか確認して、登録されている場合には実行
    ///// </summary>
    //private void CheckTreasureBox() {
    //    mapMoveController.CallBackOrbSymbolTriggerEvent();
    //}

    ///// <summary>
    ///// プレイヤーレベルのボタンを押下した際の処理
    ///// </summary>
    //private void OnClickPlayerLevel() {
    //    //Debug.Log("Show SelectAbilityPopUp");

    //    // フィールドを隠す
    //    SwitchMaskField(false);

    //    selectAbilityPopUp.ShowPopUp();
    //}

    ///// <summary>
    ///// マスクで切り抜いて表示しているフィールドの表示/非表示の切り替え
    ///// </summary>
    ///// <param name="isSwitch"></param>
    //public void SwitchMaskField(bool isSwitch) {
    //    maskFieldObj.SetActive(isSwitch);
    //}

    ///// <summary>
    ///// アビリティ選択用ウインドウの生成と初期設定
    ///// </summary>
    //private void CreateSelectAbilityPopUp() {
    //    selectAbilityPopUp = Instantiate(selectAbilityPopUpPrefab, canvasTran);
    //    selectAbilityPopUp.SetUpSelectAbilityPopUp(this);
    //}

    ///// <summary>
    ///// アビリティ強化時のエフェクトをプレイヤー上で再生
    ///// </summary>
    //public IEnumerator PlayAbilityPowerUpEffect() {
    //    GameObject effect_1 = Instantiate(EffectManager.instance.abilityPowerUpPrefab_1, mapMoveController.transform.position, EffectManager.instance.abilityPowerUpPrefab_1.transform.rotation);
    //    effect_1.transform.position = new Vector3(effect_1.transform.position.x, effect_1.transform.position.y - 0.35f, effect_1.transform.position.z);
    //    Destroy(effect_1, 1.5f);

    //    yield return new WaitForSeconds(1.5f);

    //    GameObject effect_2 = Instantiate(EffectManager.instance.abilityPowerUpPrefab_2, mapMoveController.transform.position, EffectManager.instance.abilityPowerUpPrefab_2.transform.rotation);
    //    //effect_2.transform.position = new Vector3(effect_2.transform.position.x, effect_2.transform.position.y - 0.5f, effect_2.transform.position.z);
    //    Destroy(effect_2, 1.0f);
    //}

    ///// <summary>
    ///// 移動の入力を可能にする
    ///// </summary>
    //public void ActivateInputButtons() {
    //    inputButtonManager.SwitchActivateAllButtons(true);
    //}

    ///// <summary>
    ///// コンディションの効果を適用
    ///// </summary>
    //private void ApplyEffectConditions() {
    //    foreach (PlayerConditionBase condition in mapMoveController.GetConditionsList()) {

    //        condition.ApplyEffect();

    //        //// 睡眠(移動不可)の場合
    //        //if (condition.GetConditionType() == ConditionType.Sleep) {
    //        //    // 足踏みしか出来ないように入力制限する 
    //        //    inputButtonManager.SwitchActivateMoveButtons(false);
    //        //}
    //        //// 混乱(停止不可)の場合
    //        //if (condition.GetConditionType() == ConditionType.Confusion) {
    //        //    // 足踏み不可
    //        //    inputButtonManager.SwitchActivateSteppingButton(false);

    //        //    // ランダムな移動しか出来ないように入力制限する => Map側で制御

    //        //}
    //        //// 毒の場合
    //        //if (condition.GetConditionType() == ConditionType.Poison) {

    //        //    // 体力を減らす
    //        //    condition.ApplyEffect();

    //        //    // 表示更新
    //        //    StartCoroutine(UpdateDisplayHp(1.0f));
    //        //}
    //    }

    //    // 疲労の場合は攻撃力が半減(これはコンディションでの効果)

    //    // 病気の場合は移動速度が半減(これはコンディションでの効果)

    //    // 呪い(アイテム取得不可)の場合はエネミーのシンボルのみのエンカウント(これは MapController )

    //}


    //public InputButtonManager GetInputManager() {
    //    return inputButtonManager;
    //}

    ///// <summary>
    ///// SymbolManager の情報を取得
    ///// </summary>
    ///// <returns></returns>
    //public SymbolManager GetSymbolManager() {
    //    return symbolManager;
    //}


    ///// <summary>
    ///// バトルで付与されたデバフの確認
    ///// </summary>
    //private void CheckDebuffConditions() {

    //    if (GameData.instance.debuffConditionsList.Count == 0) {
    //        return;
    //    }

    //    for (int i = 0; i < GameData.instance.debuffConditionsList.Count; i++) {
    //        // デバフの付与
    //        AddDebuff(GameData.instance.debuffConditionsList[i]);
    //    }

    //    // デバフリストをクリア
    //    GameData.instance.debuffConditionsList.Clear();
    //}

    ///// <summary>
    ///// デバフの付与
    ///// </summary>
    //private void AddDebuff(ConditionType conditionType) {

    //    ConditionData conditionData = DataBaseManager.instance.conditionDataSO.conditionDatasList.Find(x => x.conditionType == conditionType);

    //    // すでに同じコンディションが付与されているか確認
    //    if (mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == conditionType)) {
    //        // すでに付与されている場合は、持続時間を更新し、効果は上書きして処理を終了する
    //        mapMoveController.GetConditionsList().Find(x => x.GetConditionType() == conditionType).ExtentionCondition(conditionData.duration, conditionData.conditionValue);
    //        return;
    //    }

    //    // 付与するコンディションが睡眠かつ、すでに混乱のコンディションが付与されているときには、睡眠のコンディションは無視する(操作不能になるため)
    //    if (conditionType == ConditionType.Sleep && mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == ConditionType.Confusion)) {
    //        return;
    //    }

    //    // 付与されていないコンディションの場合は、付与する準備する
    //    PlayerConditionBase playerCondition;

    //    // Player にコンディションを付与
    //    playerCondition = conditionType switch {

    //        ConditionType.View_Wide => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
    //        ConditionType.View_Narrow => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
    //        ConditionType.Hide_Symbols => mapMoveController.gameObject.AddComponent<PlayerCondition_HideSymbol>(),
    //        ConditionType.Untouchable => mapMoveController.gameObject.AddComponent<PlayerCondition_Untouchable>(),
    //        ConditionType.Walk_through => mapMoveController.gameObject.AddComponent<PlayerCondition_WalkThrough>(),
    //        ConditionType.Sleep => mapMoveController.gameObject.AddComponent<PlayerCondition_Sleep>(),
    //        ConditionType.Confusion => mapMoveController.gameObject.AddComponent<PlayerCondition_Confusion>(),
    //        ConditionType.Curse => mapMoveController.gameObject.AddComponent<PlayerCondition_Curse>(),
    //        ConditionType.Poison => mapMoveController.gameObject.AddComponent<PlayerCondition_Poison>(),
    //        ConditionType.Disease => mapMoveController.gameObject.AddComponent<PlayerCondition_Disease>(),
    //        ConditionType.Fatigue => mapMoveController.gameObject.AddComponent<PlayerCondition_Fatigue>(),
    //        _ => null
    //    };

    //    // 初期設定を実行
    //    playerCondition.AddCondition(conditionType, conditionData.duration, conditionData.conditionValue, mapMoveController);

    //    // コンディション用の List に追加
    //    mapMoveController.AddConditionsList(playerCondition);
    //}

    ///// <summary>
    ///// レベルアップ演出
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator GenerateLebelUpEffect() {

    //    Debug.Log("レベルアップ演出");

    //    yield return new WaitForSeconds(1.0f);

    //    GameObject levelUpLogo = Instantiate(EffectManager.instance.LevelUpLogoPrefab, overlayCanvasTran, false);
    //    GameObject levelUpEffect = Instantiate(EffectManager.instance.levelUpPrefab, transform.position, EffectManager.instance.levelUpPrefab.transform.rotation);
    //    Destroy(levelUpEffect, 2.5f);

    //    Sequence sequence = DOTween.Sequence();
    //    sequence.Append(levelUpLogo.transform.DOLocalMoveY(325.0f, 1.0f).SetEase(Ease.OutQuart));
    //    sequence.Append(levelUpLogo.transform.DOShakeScale(0.15f, 0.5f, 5).SetEase(Ease.InQuart));
    //    sequence.AppendInterval(0.5f).OnComplete(() => { Destroy(levelUpLogo); });
    //}

    ///// <summary>
    ///// ボス出現のエフェクト生成の準備
    ///// </summary>
    ///// <returns></returns>
    //private void PreparateBossEffect() {

    //    StartCoroutine(PlayBossEffect());
    //}

    ///// <summary>
    ///// ボス出現のエフェクト生成
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator PlayBossEffect() {

    //    BossEffect bossEffect = Instantiate(EffectManager.instance.bossEffectPrefab, overlayCanvasTran, false);

    //    yield return StartCoroutine(bossEffect.PlayEffect());

    //    // ボスバトルであることを記録
    //    GameData.instance.isBossBattled = true;

    //    // シーン遷移
    //    SceneStateManager.instance.PreparateBattleScene();
    //}   

    ///// <summary>
    ///// Overlay 設定の Canvas の情報を取得
    ///// </summary>
    ///// <returns></returns>
    //public Transform GetOverlayCanvasTran() {
    //    return overlayCanvasTran;
    //}
}
