using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 祝福カードの実行クラス
/// </summary>
public class BlessingCardExecutor : ICardExecutor {
    private ItemInfoDisplayManager itemInfoDisplayManager;
    private EventExecutor eventExecutor;
    

    public BlessingCardExecutor(ItemInfoDisplayManager itemInfoDisplayManager, EventExecutor eventExecutor) {
        this.itemInfoDisplayManager = itemInfoDisplayManager;
        this.eventExecutor = eventExecutor;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        //DebugLogger.Log("blessing");

        if (card.cardData.masterData is BlessingData blessingData) {
            // 画面表示
            await itemInfoDisplayManager.ShowBlessingInfoAsync(blessingData, token);

            // 効果適用
            await eventExecutor.ExecuteEventAsync(blessingData, token);
        }
    }
}