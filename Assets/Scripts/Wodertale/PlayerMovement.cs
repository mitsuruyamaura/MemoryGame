using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private Tilemap walkableTilemap;  // 移動可能なタイルマップ
    [SerializeField] private Tilemap colliderTilemap;  // コライダー用タイルマップ
    // [SerializeField] private Stage stage;
    // [SerializeField] private List<PlayerConditionBase> conditionsList = new List<PlayerConditionBase>();
    [SerializeField] private bool isOverlapCircleColliderGizmosOn;

    private Vector3Int playerPosition;                 // プレイヤーのタイル位置
    private Vector3Int prevPosition;                   // プレイヤーの1つ前のタイル位置
    private float offsetPos = 0.5f;
    private Tweener tweener;
    private float moveDuration = 0.5f;

    // 各タイルの座標に地形情報を保持する Dictionary
    private Dictionary<Vector3Int, TerrainType> terrainInfoDic = new();

    //// デバッグ用 GameData に持たせる。Stage から変える
    //public enum TurnState {
    //    None,
    //    Player_Wait,
    //    Player_Move,
    //    Enemy,
    //    Boss
    //}

    //// デバッグ用
    //private TurnState currentTurnState = TurnState.None;

    private IDisposable disposable;
    private Subject<Unit> onPlayerMoveComplete = new();
    public Observable<Unit> OnPlayerMooveComplete => onPlayerMoveComplete;
    


    void Start() {
        // デバッグ用
        //SetUp();
    }

    public void SetUp() {
        // プレイヤーの初期位置をタイルマップのグリッド座標に変換
        playerPosition = walkableTilemap.WorldToCell(transform.position);

        // マウスクリックイベントを監視
        //this.UpdateAsObservable()
        //    .ThrottleFirst(TimeSpan.FromSeconds(moveDuration))  // この順番のオペレータだと動かないので、先に Where を書くこと
        //    .Where(_ => Input.GetMouseButtonDown(0))  //  && currentTurnState == TurnState.Player_Wait
        //    .Subscribe(_ => OnClickMovePlayer());

        //currentTurnState = TurnState.Player_Wait;           
    }

    public void SetTerrainInfo(Dictionary<Vector3Int, TerrainType> terrainInfoDic) {
        this.terrainInfoDic = new();
        this.terrainInfoDic = terrainInfoDic;
    }

    private void OnDrawGizmos() {
        if (!isOverlapCircleColliderGizmosOn) {
            return;
        }
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }


    public void MovePlayerProc() {
        SymbolManager.instance.ResetEvent();

        // マウスクリックイベントを購読
        disposable = this.UpdateAsObservable()
            .Where(_ => GameData.instance.gameState.Value == GameData.GameState.Play)
            .Where(_ => Input.GetMouseButtonDown(0))  //  && currentTurnState == TurnState.Player_Wait
            .ThrottleFirst(TimeSpan.FromSeconds(moveDuration))
            .Subscribe(_ => OnClickMovePlayer());
    }

    private async void OnClickMovePlayer() {
        // UI上でクリックされているかチェック
        if (EventSystem.current.IsPointerOverGameObject()) {
            // UIをクリックしている場合、処理を中断
            return;
        }

        // マウスのワールド座標を取得
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Z軸は無視

        // マウスのワールド座標をタイルマップのグリッド座標に変換
        Vector3Int mouseCellPos = walkableTilemap.WorldToCell(mouseWorldPos);

        // クリック地点が Grid のコライダーの場合、移動しないで終了
        // (これだけだと、すぐ横のタイルはいいが、先の方をクリックした場合にスタミナが減るので、移動先決定後も再チェックする)
        // TODO 侵入できる効果がある場合には無視させる
        if (colliderTilemap.GetColliderType(mouseCellPos) == Tile.ColliderType.Grid) {
            DebugLogger.Log("移動不可");

            //CompleteMove();
            return;
        }

        //Debug.Log("Mouse Cell Position: " + mouseCellPos);
        //Debug.Log("Player Position: " + playerPosition);

        // プレイヤーのタイル位置と同じなら移動しない
        if (mouseCellPos == playerPosition) {
            DebugLogger.Log("足踏み");

            // TODO 足踏みの機能を入れる場合にはここに書く
            // 一旦、ランダムなアイテム１つの耐久力を回復
            PlayerInventoryManager.instance.RandomRecoveryDurability();

            GameData.instance.userData.Stamina.Value--;

            // 敵のターン開始
            CompleteMove();
            return;
        }

        //currentTurnState = TurnState.Player_Move;

        // 方向ベクトルを計算
        Vector3Int direction = mouseCellPos - playerPosition;

        // 角度を計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //// 角度に基づいて移動方向を決定(上下左右４方向)
        //if (angle >= -45 && angle <= 45) {
        //    direction = Vector3Int.right;    // 右
        //} else if (angle > 45 && angle <= 135) {
        //    direction = Vector3Int.up;       // 上
        //} else if (angle > 135 || angle <= -135) {
        //    direction = Vector3Int.left;     // 左
        //} else if (angle > -135 && angle < -45) {
        //    direction = Vector3Int.down;     // 下
        //}

        // 角度に基づいて移動方向を決定(上下左右斜め８方向)
        if (angle > -22.5f && angle <= 22.5f) {
            direction = Vector3Int.right;          // 右
        } else if (angle > 22.5f && angle <= 67.5f) {
            direction = new Vector3Int(1, 1, 0);   // 右上
        } else if (angle > 67.5f && angle <= 112.5f) {
            direction = Vector3Int.up;             // 上
        } else if (angle > 112.5f && angle <= 157.5f) {
            direction = new Vector3Int(-1, 1, 0);  // 左上
        } else if (angle > 157.5f || angle <= -157.5f) {
            direction = Vector3Int.left;           // 左
        } else if (angle > -157.5f && angle <= -112.5f) {
            direction = new Vector3Int(-1, -1, 0); // 左下
        } else if (angle > -112.5f && angle <= -67.5f) {
            direction = Vector3Int.down;           // 下
        } else if (angle > -67.5f && angle <= -22.5f) {
            direction = new Vector3Int(1, -1, 0);  // 右下
        }

        // 移動先のタイル位置を計算
        Vector3Int targetPosition = playerPosition + direction;
        //Debug.Log(targetPosition);

        // 水辺の場合、移動力が 2 以上残っていないと移動不可
        TerrainType targetTerrainType = GetTerrainAtPosition(targetPosition);
        //Debug.Log(targetTerrainType);
        if (targetTerrainType == TerrainType.Waterside && GameData.instance.userData.Stamina.Value <= 1) {
            return;
        }

        // 移動先の地点が移動不可タイルの場合も移動不可(マップ外クリックもこちらに該当)
        if (targetTerrainType == TerrainType.Mountain) {
            return;
        }

        // 移動するのでスタミナを減少させる(finally の中でもいい)
        if (targetTerrainType == TerrainType.Waterside) {
            GameData.instance.userData.Stamina.Value -= 2;
        } else {
            GameData.instance.userData.Stamina.Value--;
        }

        try {
            // ターゲット位置が移動可能であるか最終チェック
            if (colliderTilemap.GetColliderType(targetPosition) == Tile.ColliderType.None && walkableTilemap.HasTile(targetPosition)) {

                // プレイヤーの位置を更新
                prevPosition = playerPosition;
                playerPosition = targetPosition;

                // 新しいタイル位置をワールド座標に変換
                Vector3 newWorldPosition = walkableTilemap.CellToWorld(playerPosition);

                // DOTween を使わない場合の移動
                //transform.position = new Vector3(newWorldPosition.x + offsetPos, newWorldPosition.y + offsetPos, transform.position.z);

                Vector3 destination = new(newWorldPosition.x + offsetPos, newWorldPosition.y + offsetPos, transform.position.z);

                // 移動
                tweener = transform.DOMove(destination, moveDuration)
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject);

                // 移動が完了するまで待機
                await tweener.AsyncWaitForCompletion();

                // シンボルの処理やターンの完了処理
                HandleSymbolInteraction();
            }
        } finally {
            // 移動後の処理が完了後、あるいは処理の途中で return された場合であっても必ずここを通ってターンを終了させる
            CompleteMove();
        }
    }

    /// <summary>
    /// 移動後に実行する処理
    /// </summary>
    public void HandleSymbolInteraction() {
        // 移動先にプレイヤーのサイズのコライダーを展開し、Symbol があるか判定する
        SymbolBase symbolBase = Physics2D.OverlapCircle(transform.position, 0.4f)?.GetComponent<SymbolBase>();

        if (symbolBase == null) {
            DebugLogger.Log(symbolBase);
            return;
        }

        // 同じシンボルに接触した場合は処理しない
        if (symbolBase.isSymbolTriggerd) {
            DebugLogger.Log(symbolBase.isSymbolTriggerd);
            return;
        }

        DebugLogger.Log("移動先でシンボルに接触 : " + symbolBase.symbolType.ToString());

        // エネミー以外のシンボルの場合
        if (symbolBase.symbolType != SymbolType.Enemy) {

            // 呪いのコンディションの確認
            if (GameData.instance.JudgeConditionType(ConditionType.Curse)) {

                // 呪い状態である場合は、シンボルのイベントを発生させない
                return;
            }

            // バトル以外のイベントを追加
            SymbolManager.instance.AddSymbolEvent(symbolBase);
            DebugLogger.Log($"{symbolBase.name} 登録");
            // エネミーのシンボルの場合
        } else {
            // エネミーのシンボルに接触した際、プレイヤーに Walk_Through のコンディションが付与されている場合
            // symbolBase.symbolType == SymbolType.Enemy && conditionsList.Exists(x => x.GetConditionType() == ConditionType.Walk_through)
            if (symbolBase.symbolType == SymbolType.Enemy && GameData.instance.JudgeConditionType(ConditionType.Walk_through)) {
                // エネミーに接触しても戦闘を開始しない
                return;
            }

            symbolBase.isSymbolTriggerd = true;

            // イベント登録(非同期メソッド登録できないため、代わりに登録)
            SymbolManager.instance.AddBattleEvent(symbolBase.TriggerEffect, (EnemySymbol)symbolBase);

            DebugLogger.Log($"{symbolBase.name} 登録");
        }
    }

    /// <summary>
    /// 移動終了後の処理
    /// </summary>
    private void CompleteMove() {
        disposable?.Dispose();

        onPlayerMoveComplete.OnNext(Unit.Default);
        tweener?.Kill();
    }

    /// <summary>
    /// 引数に指定されたコンディションが付与されているか確認
    /// </summary>
    /// <param name="conditionType"></param>
    /// <returns></returns>
    //public bool JudgeConditionType(ConditionType conditionType) {
    //    return conditionsList.Find(x => x.GetConditionType() == conditionType);
    //}

    public void EnterdBoss() {
        HandleSymbolInteraction();
        CompleteMove();
        //onPlayerMoveComplete.OnNext(Unit.Default);
    }

    /// <summary>
    /// 座標から地形情報を取得する
    /// 未使用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public TerrainType GetTerrainAtPosition(Vector3Int pos) {
        if (terrainInfoDic.ContainsKey(pos)) {
            return terrainInfoDic[pos];
        }
        return TerrainType.Mountain; // デフォルトの地形
    }

    /// <summary>
    /// 未使用
    /// </summary>
    //private void RestorePlayerWaitState() {
    //    // 状態をPlayer_Waitに戻す共通処理
    //    currentTurnState = TurnState.Player_Wait;
    //}
}