using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// 祝福カードのイベント実行クラス
/// </summary>
public class EventExecutor {
    private readonly Dictionary<BlessingType, IEventExecutor> executorMap;

    public EventExecutor(BattleManager battleManager, MemoryGameManager memoryGameManager, PlayerInventoryManager playerInventoryManager, ConditionManager conditionManager) {
        // 各 Executor 生成と、利用する Executor の依存性注入
        executorMap = new() {
            { BlessingType.Heal, new HealEventExecutor(battleManager, conditionManager) },
            { BlessingType.HealAll, new HealAllEventExecutor(battleManager, conditionManager, memoryGameManager) },
            { BlessingType.EnhanceResist, new EnhanceResistEventExecutor(battleManager) },
            { BlessingType.EnhanceItem, new EnhanceItemEventExecutor(playerInventoryManager, conditionManager) },
            { BlessingType.DestroyCard, new DestroyCardEventExecutor(memoryGameManager) },
            { BlessingType.Look, new LookCardEventExecutor(memoryGameManager) },
            { BlessingType.Reload, new ReloadEventExecutor() },
            { BlessingType.GainXp, new GainXpEventExecutor() },
            { BlessingType.ClassRankUp, new ClassRankUpEventExecutor() },
            { BlessingType.InventorySizeUp, new InventorySizeUpEventExecutor() },
            { BlessingType.EnhanceRandomItems, new EnhanceRandomItemsExecutor(playerInventoryManager, conditionManager) },

        };
    }

    /// <summary>
    /// イベント実行
    /// </summary>
    /// <param name="blessingData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ExecuteEventAsync(BlessingData blessingData, CancellationToken token) {
        // マッピングされている BlessingType から、イベント用のインスタンスを見つけて生成
        if (executorMap.TryGetValue(blessingData.type, out var executor)) {
            DebugLogger.Log($"BlessingType: {blessingData.type}");
            await executor.ExecuteAsync(blessingData, token);

            GameData.instance.userData.BlessingCount.Value++;
        } else {
            DebugLogger.Log($"未登録のBlessingType: {blessingData.type}");
        }
    }
}