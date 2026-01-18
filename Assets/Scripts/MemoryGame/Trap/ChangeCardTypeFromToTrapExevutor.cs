using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 指定した種類のカードを、別の種類のカードに変更するトラップ効果実行クラス
/// </summary>
public class ChangeCardTypeFromToTrapExevutor : ITrapEffect {
    private MemoryGameManager memoryGameManager;

    public ChangeCardTypeFromToTrapExevutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        memoryGameManager.ReplaceCardsByType(trapActionData.fromCardEventType, trapActionData.toCardEventType);
        await UniTask.Yield(token);
    }
}