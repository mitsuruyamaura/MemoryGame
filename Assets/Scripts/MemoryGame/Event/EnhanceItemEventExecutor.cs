using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// アイテム強化イベント実行クラス
/// </summary>
public class EnhanceItemEventExecutor : IEventExecutor {
    private PlayerInventoryManager playerInventoryManager;
    private ConditionManager conditionManager;

    public EnhanceItemEventExecutor(PlayerInventoryManager playerInventoryManager, ConditionManager conditionManager) {
        this.playerInventoryManager = playerInventoryManager;
        this.conditionManager = conditionManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        // 呪詛解除
        conditionManager.RemoveCondition(ConditionType.Curse);

        // アイテム強化
        playerInventoryManager.EnhanceItem(blessingData);
        await UniTask.Yield(token);
    }
}