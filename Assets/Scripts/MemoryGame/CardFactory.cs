/// <summary>
/// カードモデルファクトリークラス
/// </summary>
public class CardFactory {
    private EnemyCardExecutor enemyCardExecutor;                     // 各カードの実行処理クラス
    private TrapCardExecutor trapCardExecutor;
    private TreasureChestCardExecutor treasureChestCardExecutor;
    private BlessingCardExecutor blessingCardExecutor;
    private MemoryFragmentsCardExecutor memoryFragmentsCardExecutor;
    private StairsCardExecutor stairsCardExecutor;

    private BattleExecutor battleExecutor;                   // バトル実行処理クラス
    private TrapExecutor trapExecutor;                       // 罠実行(QTEイベント)処理クラス
    private MemoriaRankUpExecutor memoriaRankUpExecutor;     // メモリアランクアップ実行処理クラス
    private TreasureGetExecutor treasureGetExecutor;         // 宝箱獲得実行処理クラス
    private EventExecutor eventExecutor;

    public CardFactory(MemoryGameManager memoryGameManager, BattleManager battleManager, TrapDisarmQTEManager trapDisarmQTEManager, MemoryLinkManager memoryLinkManager, PlayerInventoryManager playerInventoryManager, ItemInfoDisplayManager itemInfoDisplayManager, ConditionManager conditionManager) {
        InitExecutors(memoryGameManager, battleManager, trapDisarmQTEManager, memoryLinkManager, playerInventoryManager, itemInfoDisplayManager, conditionManager);
    }

    /// <summary>
    /// 各カードの実行処理クラスの初期化
    /// コンストラクタで呼び出して依存性を注入
    /// </summary>
    public void InitExecutors(MemoryGameManager memoryGameManager, BattleManager battleManager, TrapDisarmQTEManager trapDisarmQTEManager, MemoryLinkManager memoryLinkManager, PlayerInventoryManager playerInventoryManager, ItemInfoDisplayManager itemInfoDisplayManager, ConditionManager conditionManager) {
        // 各 Executor 生成
        battleExecutor = new BattleExecutor(battleManager);
        trapExecutor = new TrapExecutor(battleManager, trapDisarmQTEManager, conditionManager, memoryGameManager, itemInfoDisplayManager);
        memoriaRankUpExecutor = new MemoriaRankUpExecutor(memoryLinkManager);
        treasureGetExecutor = new TreasureGetExecutor(itemInfoDisplayManager, playerInventoryManager);
        eventExecutor = new EventExecutor(battleManager, memoryGameManager, playerInventoryManager);

        // 各カード用の Executor 生成と、利用する Executor の依存性注入
        enemyCardExecutor = new EnemyCardExecutor(battleExecutor);
        trapCardExecutor = new TrapCardExecutor(trapExecutor);
        treasureChestCardExecutor = new TreasureChestCardExecutor(treasureGetExecutor);
        blessingCardExecutor = new BlessingCardExecutor(itemInfoDisplayManager, eventExecutor);
        memoryFragmentsCardExecutor = new MemoryFragmentsCardExecutor(memoriaRankUpExecutor);
        stairsCardExecutor = new StairsCardExecutor();
    }

    /// <summary>
    /// カードモデル作成
    /// 各 Executor の依存性を注入
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public CardModelBase CreateCardModel(CardData cardData, int cardIndex) {
        return cardData.cardTypeMaster.cardEventType switch {
            CardEventType.MemoryFragments => new MemoryFragmentsCard(cardData, cardIndex, memoryFragmentsCardExecutor),
            CardEventType.TreasureChest => new TreasureChestCard(cardData, cardIndex, treasureChestCardExecutor, false),
            CardEventType.Blessing => new BlessingCard(cardData, cardIndex, blessingCardExecutor),
            CardEventType.Enemy => new EnemyCard(cardData, cardIndex, enemyCardExecutor),
            CardEventType.Stairs => new StairsCard(cardData, cardIndex, stairsCardExecutor),
            CardEventType.Trap => new TrapCard(cardData, cardIndex, trapCardExecutor),
            _ => new StairsCard(cardData, cardIndex, stairsCardExecutor)
        };
    }

    /// <summary>
    /// 敵のドロップ用宝箱カードモデル作成
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public TreasureChestCard CreateEnemyDropTreasure(CardData cardData) {
        return new(cardData, -1, treasureChestCardExecutor, true);
    }
}