using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 宝箱カードの実行クラス
/// </summary>
public class TreasureChestCardExecutor : ICardExecutor {
    private TreasureGetExecutor treasureGetExecutor;

    public TreasureChestCardExecutor(TreasureGetExecutor treasureGetExecutor) {
        this.treasureGetExecutor = treasureGetExecutor;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        //DebugLogger.Log("TreasureChest");

        if (card is not TreasureChestCard treasureChestCard) {
            DebugLogger.Log($"TreasureChestCard ではありません : {card.GetType().Name}");
            return;
        }

        await treasureGetExecutor.ExecuteTreasureGetEventAsync(treasureChestCard.ItemData, treasureChestCard.isEnemyDrop, token);
    }
}