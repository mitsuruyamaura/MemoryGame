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

public class GameData : AbstractSingleton<GameData>
{

    public void AddMemoryStoneList(int addStoneType, int addFlipCount) {
        // 獲得数を加算
        userData.MemoryStoneCount.Value++;

        // 思い出の秘石をスロットにセット
        userData.MemoryStoneSlotList.Add(addStoneType);

        userData.FlipPoint.Value += addFlipCount;
    }

    public void ClearMemoryStoneList() {
        userData.MemoryStoneSlotList.Clear();
    }


    //public ReactiveProperty<int> staminaPoint = new ReactiveProperty<int>();

    //public ReactiveDictionary<int, bool> orbs = new ReactiveDictionary<int, bool>();

    //public int hp;

    //public int maxHp;

    //public bool isDebugOn;

    //public int playerLevel;

    //public int totalExp;

    //public CharacterData currentCharaData;

    //public int abilityPoint;

    //// アビリティアイテムのリスト
    //public List<InventryAbilityItemData> abilityItemDatasList = new List<InventryAbilityItemData>();

    //// バトルで付与されたデバフのリスト
    //public List<ConditionType> debuffConditionsList = new List<ConditionType>();

    //// World シーンで一時的に選択しているステージの番号
    public int chooseStageNo;

    //// クリア済のステージの番号
    //public List<int> clearedStageNos;


    //public bool isBossBattled;

    public float moveTimeScale;

    public CharaStatus charaStatus;
    public UserData userData;

    public CombatData playerCombatData;
    public CombatData enemyCombatData;

    public int debugStamina;
    public int debugMaxHp;

    public List<int> defeatEnemyNoList = new();   // 倒したことのある敵のリスト
    public List<int> getItemNoList = new();       // 獲得したことのあるアイテムのリスト

    public List<PlayerConditionBase> conditionList = new();

    [SerializeField]
    private Transform conditionEffectTran;

    public ReactiveProperty<int> EnchantPoint = new(0);
    public int consumeEnchantPoint;


    //protected override void Awake() {
    //    base.Awake();

    //    DOTween.Init().SetCapacity(1000, 500);

    //    // ゲームの初期化
    //    InitialzeGameData();
    //    InitUserData();
    //    InitCharaStatus();

    //    // Unityroom は PlayFab 対応していないので止めておく
    //    //LoginManager.InitializeAsync().Forget();

    //    //LoginManagerMono.instance.InitializeAsync().Forget();

    //    // ゲームの初期化
    //    void InitialzeGameData() {
    //        //maxHp = currentCharaData.maxHp;
    //        //hp = maxHp;

    //        //playerLevel = 1;

    //        //totalExp = 0;

    //        //abilityPoint += playerLevel;

    //        moveTimeScale = 1.0f;
    //    }
    //}


    protected override void Awake() {
        base.Awake();

        DOTween.Init().SetCapacity(1000, 500);

        // ゲームの初期化
        InitialzeGameData();
        InitUserData();
        InitCharaStatus();
    }

    private // ゲームの初期化
        void InitialzeGameData() {
        //maxHp = currentCharaData.maxHp;
        //hp = maxHp;

        //playerLevel = 1;

        //totalExp = 0;

        //abilityPoint += playerLevel;

        moveTimeScale = 1.0f;
    }

    /// <summary>
    /// アビリティポイントの加算
    /// </summary>
    //public void AddAbilityPoint() {
    //    abilityPoint += playerLevel;
    //}

    /// <summary>
    /// 獲得したトレジャーの情報を追加
    /// </summary>
    //public void AddaAbilityItemDatasList(AbilityType abilityType, int abilityNo) {
    //    abilityItemDatasList.Add(new InventryAbilityItemData { abilityType = abilityType, abilityNo = abilityNo});
    //}


    public void InitUserData() {
        userData = new(debugStamina);

        // debug(new で新規インスタンスにしないと参照してしまって重複リストになる)
        userData.equipItemList = new(getItemNoList);
    }

    public void InitPlayerCombatData() {
        playerCombatData = new(ConstData.DEFAULT_PLAYER_HP, ConstData.DEFAULT__INVENTORY_SIZE);
    }

    public void InitCharaStatus() {
        charaStatus = new(debugMaxHp);
        EnchantPoint.Value = GetTotalStatusValues();
    }

    /// <summary>
    /// レベルアップするか確認
    /// </summary>
    public void CheckExpNextLevel() {

        //// 現在の経験値と次のレベルに必要な経験値を比べて、レベルが上がるか確認
        //if (currentCharaStatus.exp < DataBaseManager.instance.CalcNextLevelExp(currentCharaStatus.level - 1)) {
        //    // 達していない場合には経験値とゲージ更新
        //    UpdateDisplayExp(true);

        //    // 処理終了
        //    return;
        //} else {
        //    // 達している場合にはレベルアップ
        //    GameData.instance.playerLevel++;
        //    levelupCount++;

        //    // アビリティポイント加算
        //    GameData.instance.AddAbilityPoint();

        //    Debug.Log("レベルアップ！ 現在のレベル : " + GameData.instance.playerLevel);

        //    // レベルアップ演出
        //    shinyEffectImgPlayerLevelFrame.Play();

        //    // プレイヤーレベルと経験値の表示更新
        //    UpdateDisplayPlayerLevel();
        //    UpdateDisplayExp(false);

        //    // さらにレベルが上がるか再帰処理を行って確認
        //    CheckExpNextLevel();
        //}
    }


    /// <summary>
    /// ゲーム進行状態の種類
    /// </summary>
    public enum GameState {
        Prepare,
        Wait,
        Play,
        GameUp,
        Battle
    }

    /// <summary>
    /// PlayFabのTitleId種類
    /// </summary>
    public enum PlayFabServer {
        Test,
        Dev,
    }


    public SerializableReactiveProperty<GameState> CurrentGameState = new(GameState.Prepare);

    // ログインするゲームサーバー
    public PlayFabServer playFabServer;

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
        return PlayerInventoryManager.instance.PlayerBackPackItemList.Count < playerCombatData.MaxInventorySize.Value;

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