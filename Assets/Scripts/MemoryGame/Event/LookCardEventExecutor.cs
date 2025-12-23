using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// カードの中を見るイベント実行クラス
/// </summary>
public class LookCardEventExecutor : IEventExecutor {
    private MemoryGameManager memoryGameManager;
    public LookCardEventExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        await memoryGameManager.FaceUpCardsByTargetTypeAsync(blessingData);
    }
}