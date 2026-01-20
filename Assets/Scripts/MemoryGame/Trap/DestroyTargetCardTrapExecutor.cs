using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 指定したカードタイプのカードペアをすべて破壊するトラップ実行クラス
/// </summary>
public class DestroyTargetCardTrapExecutor : ITrapEffect {
    private MemoryGameManager memoryGameManager;
    public DestroyTargetCardTrapExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }


    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        // 指定されたタイプのカードペアをすべて破壊する(ソウルポイントはもらえない)
        await memoryGameManager.DestroyAllPairsByTypeAsync(trapActionData.toCardEventType, ReleaseType.Destroy);
    }
}