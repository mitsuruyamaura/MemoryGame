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
    private int rankUpRequiredMemoryStoneCount = 3;   // ランクアップに必要な思い出の断片の数

    public SerializableReactiveProperty<GameState> CurrentGameState = new(GameState.Prepare);

    public PlayFabServer playFabServer;       // ログインするゲームサーバー

    private PlayerInventoryManager playerInventoryManager;


    protected override void Awake() {
        base.Awake();

        DOTween.Init().SetCapacity(1000, 500);
    }

    public void Setup(PlayerInventoryManager playerInventoryManager) {
        this.playerInventoryManager = playerInventoryManager;

        // インベントリの上限サイズを設定
        limitInventorySize = int.Parse(DataBaseManager.instance.GetConstantDataValue("LIMIT_INVENTORY_SIZE"));
        DebugLogger.Log($"インベントリ上限サイズ: {limitInventorySize}");

        // めくれる回数の初期値を設定
        initFlipCount = int.Parse(DataBaseManager.instance.GetConstantDataValue("DEFAULT_FLIP_COUNT"));
        DebugLogger.Log($"初期めくれる回数: {initFlipCount}");

        // 初期の最大HPを設定
        initMaxHp = int.Parse(DataBaseManager.instance.GetConstantDataValue("DEFAULT_MAX_HP"));
        DebugLogger.Log($"初期最大HP: {initMaxHp}");

        // Title シーンで選択した最終階層を設定
        lastFloorCount = GetLastFloorCountFromDifficulty(DataBaseManager.instance.entryData.selectLevel);
        DebugLogger.Log($"最終階層: {lastFloorCount}");

        expandRequiredXP = int.Parse(DataBaseManager.instance.GetConstantDataValue("EXPAND_REQUIRED_XP"));
        DebugLogger.Log($"インベントリ拡張に必要な基礎XP: {expandRequiredXP}");

        flipGainRequiredXP = int.Parse(DataBaseManager.instance.GetConstantDataValue("FLIP_GAIN_REQUIRED_XP"));
        DebugLogger.Log($"めくれる回数回復に必要な基礎XP: {flipGainRequiredXP}");

        lifeGainRequiredXP = int.Parse(DataBaseManager.instance.GetConstantDataValue("LIFE_GAIN_REQUIRED_XP"));
        DebugLogger.Log($"ライフ回復に必要な基礎XP: {lifeGainRequiredXP}");

        rankUpRequiredMemoryStoneCount = int.Parse(DataBaseManager.instance.GetConstantDataValue("RANK_UP_REQUIRED_MEMORY_STONE_COUNT"));
        DebugLogger.Log($"ランクアップに必要な思い出の断片の数: {rankUpRequiredMemoryStoneCount}");

        // ゲームの初期化
        InitialzeGameData();
        InitUserData();
        InitCharaStatus();
        InitPlayerCombatData();
    }

    /// <summary>
    /// ゲームの初期化
    /// </summary>
    private void InitialzeGameData() {
        moveTimeScale = 1.0f;
    }

    public void InitUserData() {
        userData = new(initFlipCount);

        // 物理か魔法の効果タイプのコモンアイテムをランダムで2つ取得して装備させる
        var initItemDataList = DataBaseManager.instance.GetItemDataListByRarity(Rarity.Common).Where(data => data.effectType == EffectType.Physical || data.effectType == EffectType.Magic);
        getItemNoList = initItemDataList.OrderBy(_ => UnityEngine.Random.value).Take(2).Select(data => data.id).ToList();

        // debug(new で新規インスタンスにしないと参照してしまって重複リストになる)
        userData.equipItemList = new(getItemNoList);
    }

    public void InitCharaStatus() {
        charaStatus = new(initMaxHp);
        EnchantPoint.Value = GetTotalStatusValues();
    }

    public void InitPlayerCombatData() {
        // インベントリサイズを初期化
        int defaultInventorySize = int.Parse(DataBaseManager.instance.GetConstantDataValue("DEFAULT_INVENTORY_SIZE"));
        DebugLogger.Log($"初期インベントリサイズ: {defaultInventorySize}");
        DebugLogger.Log($"初期HP: {initMaxHp}");

        playerCombatData = new(initMaxHp, defaultInventorySize);
    }

    /// <summary>
    /// 現在未使用
    /// </summary>
    /// <param name="selectLevel"></param>
    public void SetLastFloorCountFromDifficulty(int selectLevel) {
        lastFloorCount = GetLastFloorCountFromDifficulty(selectLevel);
        DebugLogger.Log($"最終階層設定: {lastFloorCount}");
    }

    /// <summary>
    /// 選択されている難易度から最終フロアの回数を取得
    /// </summary>
    /// <param name="selectLevel"></param>
    /// <returns></returns>
    private int GetLastFloorCountFromDifficulty(int selectLevel) {
        return selectLevel switch {
            0 => int.Parse(DataBaseManager.instance.GetConstantDataValue("TUTORIAL_LAST_FLOOR_COUNT")),
            1 => int.Parse(DataBaseManager.instance.GetConstantDataValue("NORMAL_LAST_FLOOR_COUNT")),
            2 => int.Parse(DataBaseManager.instance.GetConstantDataValue("HARD_LAST_FLOOR_COUNT")),
            _ => int.Parse(DataBaseManager.instance.GetConstantDataValue("TUTORIAL_LAST_FLOOR_COUNT")),
        };
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
        currentMemoryStoneIndex = currentMemoryStoneIndex % rankUpRequiredMemoryStoneCount;

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
        return userData.MemoryStoneCount.Value % rankUpRequiredMemoryStoneCount == 0;
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