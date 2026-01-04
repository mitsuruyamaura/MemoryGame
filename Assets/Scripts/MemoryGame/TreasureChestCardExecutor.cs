using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 宝箱カードの実行クラス
/// </summary>
public class TreasureChestCardExecutor : ICardExecutor {
    private TreasureGetExecutor treasureGetExecutor;
    private ConditionManager conditionManager;
    private StageUIManager stageUIManager;

    public TreasureChestCardExecutor(TreasureGetExecutor treasureGetExecutor, ConditionManager conditionManager, StageUIManager stageUIManager) {
        this.treasureGetExecutor = treasureGetExecutor;
        this.conditionManager = conditionManager;
        this.stageUIManager = stageUIManager;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        //DebugLogger.Log("TreasureChest");

        if (card is not TreasureChestCard treasureChestCard) {
            DebugLogger.Log($"TreasureChestCard ではありません : {card.GetType().Name}");
            return;
        }

        // アイテム獲得可否チェック
        if (!conditionManager.CanObtainItem()) {
            DebugLogger.Log("デバフにより、アイテム獲得できません。");

            stageUIManager.UnobtainableItemInfo();

            // TODO SE など

            return;
        }

        await treasureGetExecutor.ExecuteTreasureGetEventAsync(treasureChestCard.ItemData, treasureChestCard.isEnemyDrop, token);
    }
}