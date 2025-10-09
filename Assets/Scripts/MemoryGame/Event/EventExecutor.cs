using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class EventExecutor {
    private readonly Dictionary<BlessingType, IEventExecutor> executorMap;

    public EventExecutor() {
        executorMap = new() {
            { BlessingType.Heal, new HealEventExecutor() },
            { BlessingType.HealAll, new HealAllEventExecutor() },
            { BlessingType.EnhanceResist, new EnhanceResistEventExecutor() },
            { BlessingType.EnhanceItem, new EnhanceItemEventExecutor() },
            { BlessingType.DestroyCard, new DestroyCardEventExecutor() },
            { BlessingType.Look, new LookCardEventExecutor() },
            { BlessingType.Reload, new ReloadEventExecutor() },
            { BlessingType.GainXp, new GainXpEventExecutor() },
            { BlessingType.ClassRankUp, new ClassRankUpEventExecutor() },
            { BlessingType.InventorySizeUp, new InventorySizeUpEventExecutor() },

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
        } else {
            DebugLogger.Log($"未登録のBlessingType: {blessingData.type}");
        }
    }
}