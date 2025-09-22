using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MapMoveController : MonoBehaviour
{
    private Vector3 movePos;
    private float moveDuration = 0.35f;

    public float MoveDuration { get => moveDuration; }

    //private float moveTilePanel = 8.0f;
    //private Rigidbody2D rb;
    //Vector2 velocity;

    //[SerializeField]
    //private Tilemap tilemapWalk;

    [SerializeField]
    private Tilemap tilemapCollider;

    [SerializeField]  // Debug用
    private bool isMoving;

    public bool IsMoving { set => isMoving = value;  get => isMoving; }

    [SerializeField]
    private List<PlayerConditionBase> conditionsList = new List<PlayerConditionBase>();

    private Stage stage;

    //private int steppingRecoveryPoint = 3;

    //private UnityAction<MapMoveController> enemySymbolTriggerEvent;
    //private UnityAction<MapMoveController> orbSymbolTriggerEvent;
    private UnityEvent<MapMoveController> enemySymbolTriggerEvent;
    private UnityEvent<MapMoveController> orbSymbolTriggerEvent;

    [SerializeField]
    private Transform conditionEffectTran;

    private CancellationToken token;


    void Start() {
        token = this.GetCancellationTokenOnDestroy();

        //transform.GetChild(0).TryGetComponent(out rb);

        //PlayerConditionBase condition = gameObject.AddComponent<PlayerCondition_Fatigue>();
        //condition.AddCondition(ConditionType.Fatigue, 5, 0.5f, this, stage.GetSymbolManager());
        //conditionsList.Add(condition);
    }

    void FixedUpdate()
    {
        //InputMove();
        //rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }


    /// <summary>
    /// 移動できるタイルか判定
    /// </summary>
    public void CheckMoveTile(Vector2 nextPos) {

        //// プレイヤーの番でなければ処理しない
        //if (stage.CurrentTurnState != Stage.TurnState.Player) {
        //    return;
        //}

        //isMoving = true;

        //// 混乱状態のコンディションの確認
        //if (JudgeConditionType(ConditionType.Confusion)) {
        //    // 混乱状態の場合は移動先を乱数化(斜め移動はさせない)
        //    int x = Random.Range(-1, 2);
        //    int y = Mathf.Abs(x) == 1 ? 0 : Random.Range(-1, 2);
        //    movePos = new Vector2(x, y);
        //} else {
        //    // キー入力をそのまま適用
        //    movePos = nextPos;
        //}

        //// タイルマップの座標に変換
        //Vector3Int tilePos = tilemapCollider.WorldToCell(transform.position + movePos);

        ////Debug.Log(tilePos);
        ////Debug.Log(tilemapCollider.GetColliderType(tilePos));

        //// Grid のコライダーの場合
        //if (tilemapCollider.GetColliderType(tilePos) == Tile.ColliderType.Grid) {

        //    // 移動しないで終了
        //    isMoving = false;

        //    // ボタンを押せるようにする
        //    stage.ActivateInputButtons();
        //    //    //break;

        //    //Debug.Log("Grid");
            
        //    // Grid 以外の場合
        //} else {
        //    //if (tilemapWalk.GetColliderType(tilePos) == Tile.ColliderType.None) {   // tilemapCollider.GetColliderType(tilePos) != Tile.ColliderType.Grid) {

        //    // 移動させる
        //    Move(transform.position + movePos);
        //    //break;
        //}
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="destination"></param>
    private void Move(Vector2 destination) {
        //transform.Translate(move * 0.5f);
        //rb.velocity = move * moveTilePanel;

        //velocity = move * moveTilePanel;

        //rb.MovePosition(rb.position + velocity );

        //GameData.instance.staminaPoint.Value--;

        //// コンディションが付与されている場合、持続時間を更新
        //if (conditionsList.Count > 0) {
        //    // 現在のコンディションの状態の残り時間を更新
        //    UpdateConditionsDuration();
        //}

        // 移動
        //transform.DOMove(destination, moveDuration * GameData.instance.moveTimeScale)
        //    .SetEase(Ease.Linear)
        //    .OnComplete(() => {
        //        //isMoving = false;

        //        // エネミーの番になり、エネミーの移動処理を行う
        //        //stage.CurrentTurnState = Stage.TurnState.Enemy;

        //        // 敵のターン開始
        //        StartCoroutine(stage.ObserveEnemyTurnState());
        //    });
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.TryGetComponent(out SymbolBase symbolBase)) {
            //switch (symbolBase.symbolType) {
            //case SymbolType.Enemy:
            //    Debug.Log("移動先で敵に接触");
            //    StartCoroutine(PreparateBattle(symbolBase));

            //    break;

            //case SymbolType.Stamina:
            //case SymbolType.Life:
            //case SymbolType.Orb:

            // エネミーのシンボルに接触した際、プレイヤーに Walk_Through のコンディションが付与されている場合
            // symbolBase.symbolType == SymbolType.Enemy && conditionsList.Exists(x => x.GetConditionType() == ConditionType.Walk_through)
            if (symbolBase.symbolType == SymbolType.Enemy && TryGetComponent(out PlayerCondition_WalkThrough walkThrough)) {
                // エネミーに接触しても戦闘を開始しない
                return;
            }

            // 同じシンボルに接触した場合は処理しない
            if (symbolBase.isSymbolTriggerd) {
                return;
            }

            Debug.Log("移動先でシンボルに接触 : " + symbolBase.symbolType.ToString());

            if (symbolBase.symbolType != SymbolType.Enemy) {

                // 呪いのコンディションの確認
                if (JudgeConditionType(ConditionType.Curse)) {

                    // 呪い状態である場合は、シンボルのイベントを発生させない
                    return;
                }

                // それ以外のシンボルはすぐに実行
                //symbolBase.TriggerAppearEffect(this);

                // イベント登録
                //SymbolManager.instance.
                //    AddNormalEvent(symbolBase.TriggerEffect, symbolBase);

                //Debug.Log("登録");

                //TriggerEventProc(symbolBase).Forget();

                // バトル以外のイベントを追加
                SymbolManager.instance.AddSymbolEvent(symbolBase);

                // エネミーのシンボルの場合
            } else {

                // エネミーのシンボルのイベントの重複登録はしない
                if (enemySymbolTriggerEvent != null) {
                    return;
                }

                symbolBase.isSymbolTriggerd = true;

                // シンボルのイベントを登録して予約し、すべてのエネミーの移動が終了してから実行
                //enemySymbolTriggerEvent = (x) =>  symbolBase.TriggerAppearEffect(this);
                //enemySymbolTriggerEvent = new UnityEvent<MapMoveController>();
                //enemySymbolTriggerEvent.AddListener(symbolBase.TriggerAppearEffect);

                // イベント登録
                SymbolManager.instance.
                    AddBattleEvent(symbolBase.TriggerEffect, (EnemySymbol)symbolBase);

                Debug.Log("登録");
            }

            //// オーブか、トレジャーボックスの場合
            //if (symbolBase.symbolType == SymbolType.Orb || symbolBase.symbolType == SymbolType.TreasureBox) {
            //    // シンボルのイベントを登録して予約し、バトル後 Stage に戻ってきてから実行
            //    //orbSymbolTriggerEvent = _ => symbolBase.TriggerAppearEffect(this);
            //    //orbSymbolTriggerEvent = new UnityEvent<MapMoveController>();
            //    //orbSymbolTriggerEvent.AddListener(symbolBase.TriggerAppearEffect);

            //    symbolBase.TriggerAppearEffect(this);
            //} else if (symbolBase.symbolType != SymbolType.Enemy) {

            //    // 呪いのコンディションの確認
            //    if (JudgeConditionType(ConditionType.Curse)) {

            //        // 呪い状態である場合は、シンボルのイベントを発生させない
            //        return;
            //    }

            //    // それ以外のシンボルはすぐに実行
            //    symbolBase.TriggerAppearEffect(this);
            //}
            //break;
            //}
        }
    }


    //private async UniTask TriggerEventProc(SymbolBase symbolBase) {

    //    await symbolBase.TriggerEffectAsync();

    //    SymbolManager.instance.AddSymbolEvent(symbolBase.TriggerEffectAsync, symbolBase);
    //}

    /// <summary>
    /// 現在のコンディションの状態の残り時間を更新
    /// </summary>
    public void UpdateConditionsDuration() {
        for (int i = 0; i< conditionsList.Count; i++) {
            conditionsList[i].CalcDuration();
        }
    }

    /// <summary>
    /// コンディションを追加
    /// </summary>
    /// <param name="playerCondition"></param>
    public void AddConditionsList(PlayerConditionBase playerCondition) {
        conditionsList.Add(playerCondition);
    }

    /// <summary>
    /// コンディションを削除
    /// </summary>
    public void RemoveConditionsList(PlayerConditionBase playerCondition) {
        conditionsList.Remove(playerCondition);
        //Destroy(playerCondition);
    }

    /// <summary>
    /// コンディションの List を取得
    /// </summary>
    /// <returns></returns>
    public List<PlayerConditionBase> GetConditionsList() {
        return conditionsList;
    }

    /// <summary>
    /// 足踏み
    /// </summary>
    public void Stepping() {
        // プレイヤーの番でなければ処理しない
        //if (stage.CurrentTurnState != Stage.TurnState.Player) {
        //    return;
        //}

        //GameData.instance.staminaPoint.Value--;

        //// コンディションが付与されている場合、持続時間を更新
        //if (conditionsList.Count > 0) {
        //    // 現在のコンディションの状態の残り時間を更新
        //    UpdateConditionsDuration();
        //}

        // 足踏みしてHP回復
        //GameData.instance.hp = Mathf.Clamp(GameData.instance.hp += steppingRecoveryPoint, 0, GameData.instance.maxHp);

        //StartCoroutine(stage.UpdateDisplayHp(1.0f));

        //// エネミーの番になり、エネミーの移動処理を行う
        //stage.CurrentTurnState = Stage.TurnState.Enemy;

        // 敵のターン開始
        //StartCoroutine(stage.ObserveEnemyTurnState());
    }

    /// <summary>
    /// 登録されているエネミーシンボルのイベント(エネミーとのバトル)を実行
    /// </summary>
    public bool CallBackEnemySymbolTriggerEvent() {
        if (enemySymbolTriggerEvent != null) {
            // イベントがあるときだけ実行する
            enemySymbolTriggerEvent?.Invoke(this);

            // イベントをクリア
            enemySymbolTriggerEvent?.RemoveAllListeners();
            enemySymbolTriggerEvent = null;

            return true;
        }
        return false;
    }

    public async UniTask<bool> ExecuteSymbolEventAsync() {  // Token

        await UniTask.Yield();

        //if (enemySymbolTriggerEvent != null) {
        //    // Channel で待機
        //    bool isBattleResult = await BattleManager.instance.StartBattle();

        //    // イベントをクリア
        //    enemySymbolTriggerEvent?.RemoveAllListeners();
        //    enemySymbolTriggerEvent = null;
        //    return isBattleResult;

        //    // イベントがあるときだけ実行する
        //    enemySymbolTriggerEvent?.Invoke(this);

        //    // イベントをクリア
        //    enemySymbolTriggerEvent?.RemoveAllListeners();
        //    enemySymbolTriggerEvent = null;

        //    return true;
        //}
        return false;
    }


    /// <summary>
    /// 登録されているオーブシンボルのイベントを実行
    /// </summary>
    public void CallBackOrbSymbolTriggerEvent() {

        if (orbSymbolTriggerEvent != null) {
            // イベントがあるときだけ実行する
            orbSymbolTriggerEvent?.Invoke(this);

            // イベントをクリア
            orbSymbolTriggerEvent?.RemoveAllListeners();
            orbSymbolTriggerEvent = null;
        }
    }

    /// <summary>
    /// Stage の情報を取得
    /// </summary>
    /// <returns></returns>
    public Stage GetStage() {
        return stage;
    }

    /// <summary>
    /// 引数に指定されたコンディションが付与されているか確認
    /// </summary>
    /// <param name="conditionType"></param>
    /// <returns></returns>
    public bool JudgeConditionType(ConditionType conditionType) {
        return conditionsList.Exists(x => x.GetConditionType() == conditionType);
    }

    /// <summary>
    /// コンディション用のエフェクト生成位置の取得
    /// </summary>
    /// <returns></returns>
    public Transform GetConditionEffectTran() {
        return conditionEffectTran;
    }
}
