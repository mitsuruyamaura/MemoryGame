using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 指定したカードタイプのカードペアをすべて破壊するイベント実行クラス
/// </summary>

public class DestroyCardsByTypeEventExecutor : IEventExecutor {
    private MemoryGameManager memoryGameManager;

    public DestroyCardsByTypeEventExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        // 指定したタイプのカードペアをすべて破壊する
        CardEventType targetCardEventType = MemoryGameManager.ConvertCardEventTypeByBlessingValueType(blessingData.valueType);
        await memoryGameManager.DestroyAllPairsByTypeAsync(targetCardEventType, ReleaseType.Distribute);
    }
}