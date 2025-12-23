using System.Collections.Generic;
using UnityEngine;
using R3;
using DG.Tweening;
using System.Linq;

/// <summary>
/// インベントリ用のアイテムデータ
/// </summary>
[System.Serializable]
public class InventryAbilityItemData {
    public AbilityType abilityType;
    public int abilityNo;
}

/// <summary>
/// ゲーム進行状態の種類
/// </summary>
public enum GameState {
    Prepare,
    Wait,
    Play,
    GameUp,
    Battle,
    TrapDisarm
}

/// <summary>
/// PlayFabのTitleId種類
/// </summary>
public enum PlayFabServer {
    Test,
    Dev,
}


public class GameData : AbstractSingleton<GameData> {
    public float moveTimeScale;

    public CharaStatus charaStatus;
    public UserData userData;

    public int limitInventorySize = 32;                // 上限値
    public int expandRequiredXP = 200;                 // インベントリの拡張に必要な基礎値
    public int flipGainRequiredXP = 100;               // めくれる回数の回復に必要な基礎値
    public int lifeGainRequiredXP = 100;               // ライフの回復に必要な基礎値

    public int lastFloorCount = 50;                    // 最終階層

    public CombatData playerCombatData;           // プレイヤーの戦闘データ。この中に Hp などが含まれる
    public CombatData enemyCombatData;

    public int initFlipCount;                     // 初期のめくれる回数
    public int initMaxHp;                         // 初期の最大HP

    public List<int> defeatEnemyNoList = new();   // 倒したことのある敵のリスト
    public List<int> getItemNoList = new();       // 獲得したことのあるアイテムのリスト

    public List<PlayerConditionBase> conditionList = new();

    [SerializeField] private Transform conditionEffectTran;

    public SerializableReactiveProperty<int> EnchantPoint = new(0);
    public int consumeEnchantPoint;

    public SerializableReactiveProperty<int> ComboPairCount = new(0);     // コンボで繋げたペア数
    public SerializableReactiveProperty<int> MatchedPairCount = new(0);   // ペアを揃えた数

    private int currentMemoryStoneIndex = 0;
    private readonly int maxMemoryStoneCount = 3;

    public SerializableReactiveProperty<GameState> CurrentGameState = new(GameState.Prepare);

    public PlayFabServer playFabServer;       // ログインするゲームサーバー

    private PlayerInventoryManager playerInventoryManager;


    protected override void Awake() {
        base.Awake();

        DOTween.Init().SetCapacity(1000, 500);

        // ゲームの初期化
        InitialzeGameData();
        InitUserData();
        InitCharaStatus();
    }

    public void Setup(PlayerInventoryManager playerInventoryManager) {
        this.playerInventoryManager = playerInventoryManager;
    }

    /// <summary>
    /// ゲームの初期化
    /// </summary>
    private void InitialzeGameData() {
        moveTimeScale = 1.0f;
    }

    /// <summary>
    /// 思い出の断片をリストに追加
    /// </summary>
    /// <param name="memoryStoneData"></param>
    /// <returns></returns>
    public void AddMemoryStoneList(MemoryStoneData memoryStoneData) {
        // 獲得数を加算
        userData.MemoryStoneCount.Value++;

        userData.MemoriaCount.Value++;

        // 思い出の断片をスロットにセット
        userData.MemoryStoneSlotList.Add(currentMemoryStoneIndex);

        // UI 用のスロットインデックスを更新
        currentMemoryStoneIndex++;
        currentMemoryStoneIndex = currentMemoryStoneIndex % maxMemoryStoneCount;

        // めくれる回数を加算
        userData.FlipPoint.Value += memoryStoneData.addFlipCount;

        // ランクアップの確認
        bool isRankUp = CheckMemoriaRankUp();
        if (isRankUp) {
            // ランクアップ
            userData.MemoriaRank.Value++;
            ClearMemoryStoneList();
        }
    }

    public void ClearMemoryStoneList() {
        userData.MemoryStoneSlotList.Clear();
    }

    /// <summary>
    /// ランクアップするか確認
    /// ランクアップする場合には true を返す
    /// </summary>
    /// <returns></returns>
    public bool CheckMemoriaRankUp() {
        return userData.MemoryStoneCount.Value % maxMemoryStoneCount == 0;
    }

    public void InitUserData() {
        userData = new(initFlipCount);

        // 物理か魔法の効果タイプのコモンアイテムをランダムで2つ取得して装備させる
        var initItemDataList = DataBaseManager.instance.GetItemDataListByRarity(Rarity.Common).Where(data => data.effectType == EffectType.Physical || data.effectType == EffectType.Magic);
        getItemNoList = initItemDataList.OrderBy(_ => UnityEngine.Random.value).Take(2).Select(data => data.id).ToList();

        // debug(new で新規インスタンスにしないと参照してしまって重複リストになる)
        userData.equipItemList = new(getItemNoList);
    }

    public void InitPlayerCombatData() {
        playerCombatData = new(ConstData.DEFAULT_PLAYER_HP, ConstData.DEFAULT__INVENTORY_SIZE);
    }

    public void InitCharaStatus() {
        charaStatus = new(initMaxHp);
        EnchantPoint.Value = GetTotalStatusValues();
    }

    /// <summary>
    /// GameState の切り替え
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeGameState(GameState nextState) {
        CurrentGameState.Value = nextState;
    }

    /// <summary>
    /// ログインするサーバーの取得
    /// </summary>
    /// <returns></returns>
    public string GetGameServer() {
        return playFabServer switch {
            PlayFabServer.Test => ConstData.PLAYFAB_TEST_TITLEID,
            PlayFabServer.Dev => ConstData.PLAYFAB_DEV_TITLEID,
            _ => ConstData.PLAYFAB_TEST_TITLEID
        };
    }


    public bool CheckDefeatEnemyNo(int enemyNo) {
        return defeatEnemyNoList.Any(data => data == enemyNo);
    }


    public bool CheckGetItem(int itemNo) {
        return getItemNoList.Any(data => data == itemNo);
    }


    public void AddDefeatEnemyList(int enemyNo) {
        defeatEnemyNoList.Add(enemyNo);
    }

    public int GetTotalStatusValues() {
        return charaStatus.statusValueList.Select(data => data.statusValue.Value).Sum();
    }

    /// <summary>
    /// 現在のインベントリサイズが最大サイズを超えていないか確認し、最大サイズ以下の場合に true を返す
    /// </summary>
    /// <returns></returns>
    public bool IsInventoryUnderMaxSize() {
        return playerInventoryManager.PlayerBackPackItemList.Count < playerCombatData.MaxInventorySize.Value;

    }

    /// <summary>
    /// 現在のコンディションの状態の残り時間を更新
    /// </summary>
    public void UpdateConditionDuration(ConditionData conditionData) {
        //for (int i = 0; i < conditionList.Count; i++) {
        //    conditionList[i].CalcDuration();
        //}

        conditionList.FirstOrDefault(data => data.GetConditionType() == conditionData.conditionType)
            .ExtentionCondition(conditionData.duration, conditionData.conditionValue);
    }

    /// <summary>
    /// コンディションを追加
    /// </summary>
    /// <param name="playerCondition"></param>
    public void AddConditionList(PlayerConditionBase playerCondition) {
        conditionList.Add(playerCondition);
    }

    /// <summary>
    /// コンディションを削除
    /// </summary>
    public void RemoveConditionList(PlayerConditionBase playerCondition) {
        conditionList.Remove(playerCondition);

        // エフェクト削除
        //Destroy(playerCondition);
    }

    /// <summary>
    /// 引数に指定されたコンディションが付与されているか確認
    /// </summary>
    /// <param name="checkConditionType"></param>
    /// <returns></returns>
    public bool JudgeConditionType(ConditionType checkConditionType) {
        return conditionList.Exists(condition => condition.GetConditionType() == checkConditionType);
    }
}