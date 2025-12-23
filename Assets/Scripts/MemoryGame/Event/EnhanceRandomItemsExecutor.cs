using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ランダムなアイテムを強化するイベント実行クラス
/// </summary>
public class EnhanceRandomItemsExecutor : IEventExecutor {
    private PlayerInventoryManager playerInventoryManager;
    public EnhanceRandomItemsExecutor(PlayerInventoryManager playerInventoryManager) {
        this.playerInventoryManager = playerInventoryManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        playerInventoryManager.EnhanceRandomItems(blessingData);
        await UniTask.Yield(token);
    }
}