using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ランダムなアイテムを強化するイベント実行クラス
/// </summary>
public class EnhanceRandomItemsExecutor : IEventExecutor {
    private PlayerInventoryManager playerInventoryManager;
    private ConditionManager conditionManager;

    public EnhanceRandomItemsExecutor(PlayerInventoryManager playerInventoryManager, ConditionManager conditionManager) {
        this.playerInventoryManager = playerInventoryManager;
        this.conditionManager = conditionManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        // 呪詛解除
        conditionManager.RemoveCondition(ConditionType.Curse);

        playerInventoryManager.EnhanceRandomItems(blessingData);
        await UniTask.Yield(token);
    }
}