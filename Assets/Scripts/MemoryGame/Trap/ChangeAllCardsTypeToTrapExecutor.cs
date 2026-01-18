using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// すべてのカードタイプを指定されたトラップカードタイプに変更するトラップ効果の実行クラス
/// </summary>
public class ChangeAllCardsTypeToTrapExecutor : ITrapEffect {
    private MemoryGameManager memoryGameManager;
    
    public ChangeAllCardsTypeToTrapExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        memoryGameManager.ReplaceAllCardsToType(trapActionData.toCardEventType);
        await UniTask.Yield(token);
    }
}