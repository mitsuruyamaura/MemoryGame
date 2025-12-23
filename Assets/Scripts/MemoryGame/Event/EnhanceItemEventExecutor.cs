using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// アイテム強化イベント実行クラス
/// </summary>
public class EnhanceItemEventExecutor : IEventExecutor {
    private PlayerInventoryManager playerInventoryManager;
    public EnhanceItemEventExecutor(PlayerInventoryManager playerInventoryManager) {
        this.playerInventoryManager = playerInventoryManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        playerInventoryManager.EnhanceItem(blessingData);
        await UniTask.Yield(token);
    }
}