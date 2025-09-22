using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System.Threading;
using R3;

/// <summary>
/// シンボルの生成・管理・制御を行うクラス
/// </summary>
public class SymbolManager : AbstractSingleton<SymbolManager> {

    [SerializeField]
    private List<SymbolBase> symbolsList = new();  // これを不要として、最初から各シンボルの種類ごとで List 化しておき、全部必要な時は List を結合する

    public List<SymbolBase> SymbolsList {
        set => symbolsList = value;
        get => symbolsList;
    }

    [SerializeField]　　// Debug 用
    private List<EnemySymbol> enemiesList = new();

    public List<OrbSymbol> specialSymbols = new();

    [SerializeField] private CameraSizeChanger cameraSizeChanger;

    public Tilemap tilemapCollider;   // Grid_Collison の Tilemap をアサイン

    private UnityEvent enemySymbolTriggerEvent;
    private EnemySymbol enterSymbol;
    private SymbolBase symbolBase;

    public Subject<Unit> onTurnEnd = new();
    public Subject<Unit> onSuccessSettlement = new();
    public CancellationTokenSource cts; 


    //void Start() {
    //    // Debug
    //    SetUpAllSymbos();
    //}

    /// <summary>
    /// すべてのシンボルの初期設定
    /// </summary>
    public void SetUpAllSymbols() {
        ResetEvent();
        cts = new ();

        //int orbNo = 0;

        // 各シンボルの設定
        for (int i = 0; i < symbolsList.Count; i++) {
            symbolsList[i].transform.SetParent(this.transform);
            symbolsList[i].OnEnterSymbol();

            // 特殊シンボルの場合
            //if (symbolsList[i].symbolType == SymbolType.Orb) {
            //    // 追加設定(画像を変えたり、オーブの種類を設定)
            //    symbolsList[i].GetComponent<OrbSymbol>().SetOrbData(GameData.instance.currentStageData.orbTypes[orbNo], orbNo);
            //    specialSymbols.Add(symbolsList[i].GetComponent<OrbSymbol>());
            //    orbNo++;
            //}
        }

        // Enemy の種類だけを抽出して List に代入
        //enemiesList = GetListSimbolTypeFromSymbolsList(SymbolType.Enemy);

        //// オーブの配置用にエネミーの数分の番号を登録
        //List<int> numbers = new List<int>();

        //for (int i = 0; i < enemiesList.Count; i++) {
        //    numbers.Add(i);
        //}

        ////各オーブをランダムなエネミーの上に配置
        //for (int i = 0; i < specialSymbols.Count; i++) {
        //    int randomIndex = Random.Range(0, numbers.Count);
        //    specialSymbols[i].GetComponent<OrbSymbol>().SetPositionOrbSymbol(enemiesList[randomIndex].transform.position);
        //    //Debug.Log(randomIndex);
        //    numbers.RemoveAt(randomIndex);
        //}

        ////各オーブをエネミーの上に配置(動くけどイマイチ)
        //int randomIndex = Mathf.FloorToInt(enemiesList.Count / specialSymbols.Count);
        //for (int i = 0; i < specialSymbols.Count; i++) {
        //    if (i == 3 && enemiesList.Count % specialSymbols.Count == 0) {
        //        randomIndex = 0;
        //    }
        //    specialSymbols[i].GetComponent<OrbSymbol>().SetPositionOrbSymbol(enemiesList[randomIndex * (i + 1)].transform.position);
        //}
    }

    /// <summary>
    /// Symbol の List を取得
    /// </summary>
    /// <returns></returns>
    public List<SymbolBase> GetSymbolsList() {
        return symbolsList;
    }

    /// <summary>
    /// すべてのシンボルの画像を表示/非表示
    /// </summary>
    public void SwitchDisplayAllSymbols(bool isSwitch) {
        for (int i = 0; i < symbolsList.Count; i++) {
            symbolsList[i].SwitchDisplaySymbol(isSwitch);
        }
    }

    /// <summary>
    /// 指定された以外のシンボルのゲームオブジェクトの表示/非表示
    /// </summary>
    public void SwitchActivateExceptSymbols(bool isSwitch, int exceptSymbolTypeNo) {

        //for (int i = 0; i < symbolsList.Count; i++) {
        //    if (symbolsList[i].symbolType != exceptSymbolType) {
        //        symbolsList[i].SwitchActivateSymbol(isSwitch);
        //    }
        //}

        foreach (SymbolBase symbol in symbolsList.Where(x => x.symbolType != (SymbolType)exceptSymbolTypeNo)) {
            symbol.SwitchActivateSymbol(isSwitch);
        }
    }

    /// <summary>
    /// List からシンボルを削除
    /// </summary>
    /// <param name="symbol"></param>
    public void RemoveSymbolsList(SymbolBase symbol) {
        symbolsList.Remove(symbol);
    }

    /// <summary>
    /// List からすべてのシンボルを削除
    /// </summary>
    public void AllClearSymbolsList() {
        symbolsList?.ForEach(symbol => symbol?.Release());
        symbolsList.Clear();
        DebugLogger.Log("Symbol Reset");
    }

    /// <summary>
    /// SymbolList から引数で指定した種類のみを抽出する
    /// </summary>
    private List<EnemySymbol> GetListSimbolTypeFromSymbolsList(SymbolType getSymbolType) {
        return symbolsList.Where(x => x.symbolType == getSymbolType).Select(x => x.GetComponent<EnemySymbol>()).ToList();
    }

    /// <summary>
    /// 全エネミーのシンボルの移動処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnemisMove() {

        for (int i = 0; i < enemiesList.Count; i++) {

            // プレイヤーと接触しているエネミーは移動させない
            if (enemiesList[i].isSymbolTriggerd) {
                continue;
            }

            // 移動待機カウントのカウントダウン

            enemiesList[i].EnemyMove();
            yield return new WaitForSeconds(0.05f);
            //Debug.Log("敵の移動 :" + i + " 体目");
        }
    }

    /// <summary>
    /// エネミーの List から情報の削除
    /// </summary>
    /// <param name="enemySymbol"></param>
    public void RemoveEnemySymbol(EnemySymbol enemySymbol) {
        enemiesList.Remove(enemySymbol);
    }

    /// <summary>
    /// すべてのエネミーのコライダーを制御
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchEnemyCollider(bool isSwitch) {
        for (int i = 0; i < enemiesList.Count; i++) {
            enemiesList[i].SwtichCollider(isSwitch);
        }
    }

    public async UniTask<BattleResultType> ExecuteEnemySymbolEventAsync(TurnState turnState, CancellationToken token) {  // Token
        if (enemySymbolTriggerEvent != null) {
            // 呪いの判定
            await enterSymbol.TriggerEffectAsync();

            BattleResultType battleResultType = BattleResultType.Runaway;
            DebugLogger.Log(turnState);

            // ボス以外は交渉の判定
            if (turnState != TurnState.Boss) {
                float settlementRate = PlayerInventoryManager.instance.GetSettlementRate();
                DebugLogger.Log($"settlementRate : {settlementRate}");

                float settlementBonus = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Charm);
                DebugLogger.Log($"settlementBonus : {settlementBonus}");

                float randomSettlementValue = UnityEngine.Random.Range(0, 100.00f);
                DebugLogger.Log($"settlement randomValue : {randomSettlementValue}");

                // 合計
                settlementRate += settlementBonus;

                // 交渉成功時にはバトルなし
                if (randomSettlementValue <= settlementRate) {
                    battleResultType = BattleResultType.Win;

                    // 耐久力減少(Backpack 側で購読)
                    onSuccessSettlement.OnNext(Unit.Default);

                    // 交渉成功のメッセージ表示
                    PowerSpotInfoDisplayManager.instance.SuccessSettlementInfo();
                }
            }

            // ボス、あるいは交渉失敗時には Win になっていないので、バトル開始
            if (battleResultType != BattleResultType.Win) {
                // Channel で待機
                battleResultType = await BattleManager.instance.StartBattle(enterSymbol, turnState);
            }

            // Channel で待機
            //BattleResultType battleResultType = await BattleManager.instance.StartBattle(enterSymbol, turnState);

            // イベントをクリア
            enemySymbolTriggerEvent?.RemoveAllListeners();
            enemySymbolTriggerEvent = null;

            if (battleResultType == BattleResultType.Win) {
                GameData.instance.AddDefeatEnemyList(enterSymbol.enemyData.enemyNo);

                //GameData.instance.charaStatus.exp += enterSymbol.enemyData.exp;
                GameData.instance.userData.SoulPoint.Value += enterSymbol.enemyData.exp;

                // 倒した時のエフェクトとSE再生
                GameObject effect = Instantiate(EffectManager.instance.destroyEffectPrefab, BattleManager.instance.VirtualCamera.transform, false);
                Destroy(effect, 1.5f);

                SoundManager.instance.PlaySE(SE_TYPE.Hit_1);
            } else if (battleResultType == BattleResultType.Timeout) {
                SoundManager.instance.PlayVoice(VOICE_TYPE.Timeout);
            }

            enterSymbol.gameObject.SetActive(false);

            // 演出が終わるまで待機
            await UniTask.Delay(1500, cancellationToken: token);

            // バトルに勝利している場合
            if (battleResultType == BattleResultType.Win) {
                DebugLogger.Log("リザルトポップアップを開く");
                SoundManager.instance.PlaySE(SE_TYPE.Drop);

                // エネミーの持つインベントリから１つをランダムで選択
                int randomIndex = Random.Range(0, enterSymbol.equipItemNoList.Count);
                //Debug.Log(randomIndex);

                ItemData resultItemData = DataBaseManager.instance.GetItemData(enterSymbol.equipItemNoList[randomIndex]);

                // アイテムを所持しているか判定
                bool haveItem = PlayerInventoryManager.instance.HaveTargetItem(resultItemData.id);

                // インベントリの最大サイズ以下か判定
                bool isInventoryUnderMaxSive = GameData.instance.IsInventoryUnderMaxSize();

                // 獲得アイテムを所持しておらず、かつ、インベントリが最大の場合
                if (!haveItem && !isInventoryUnderMaxSive) {
                    BattleManager.instance.stageUIManager.InventoryMaxInfo();
                    DebugLogger.Log("バッグがいっぱい");

                    // 獲得できずに終了
                    return battleResultType;
                }

                // プレイヤーの位置に袋アイコン表示
                ItemInfoDisplayManager.instance.ShowBagIcon();

                // 獲得表示
                await ItemInfoDisplayManager.instance.ShowTreasureItemInfoAsync(resultItemData, token);

                // アイテムをバックパックに追加
                PlayerInventoryManager.instance.AddItemDataConvertBackPackInItem(resultItemData, false);

                GameData.instance.userData.DefeatedEnemyCount.Value++;
            }
            return battleResultType;
        }

        return BattleResultType.Ready;
    }

    /// <summary>
    /// バトルイベントを追加
    /// </summary>
    /// <param name="newEvent"></param>
    /// <param name="enemySymbol"></param>
    public void AddBattleEvent(UnityAction newEvent, EnemySymbol enemySymbol) {
        enemySymbolTriggerEvent = new UnityEvent();
        enemySymbolTriggerEvent.AddListener(newEvent);
        enterSymbol = enemySymbol;
    }

    /// <summary>
    /// すべてのイベントを破棄
    /// </summary>
    public void ResetEvent() {
        enemySymbolTriggerEvent = null;
        enterSymbol = null;

        symbolBase = null;
    }

    /// <summary>
    /// バトル以外のイベントを追加
    /// </summary>
    /// <param name="symbolBase"></param>
    public void AddSymbolEvent(SymbolBase symbolBase) {
        this.symbolBase = symbolBase;
    }

    /// <summary>
    /// バトル以外のイベントを実行
    /// </summary>
    /// <returns></returns>
    public async UniTask TriggerEventProc() {
        if (symbolBase == null) {
            return;
        }
        await symbolBase.TriggerEffectAsync();
    }

    /// <summary>
    /// カメラのサイズを広くする
    /// </summary>
    /// <returns></returns>
    public async UniTask ExpandViewSizeProc() {
        await cameraSizeChanger.ExpandViewSize();
    }

    /// <summary>
    /// カメラのサイズを元に戻す
    /// </summary>
    /// <returns></returns>

    public async UniTask DefaultViewSizeProc() {
        await cameraSizeChanger.DefaultViewSize();
    }
}