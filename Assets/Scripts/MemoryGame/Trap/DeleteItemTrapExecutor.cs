using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// アイテム破壊トラップ実行クラス
/// </summary>
public class DeleteItemTrapExecutor : ITrapEffect {
    private PlayerInventoryManager playerInventoryManager;

    public DeleteItemTrapExecutor(PlayerInventoryManager playerInventoryManager) {
        this.playerInventoryManager = playerInventoryManager;
    }

    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        playerInventoryManager.DeleteRandomItemFromInventory((int)trapActionData.value);
        await UniTask.Yield(token);
    }
}