using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// トラップカードの実行クラス
/// </summary>
public class TrapCardExecutor : ICardExecutor {
    private TrapExecutor trapExecutor;

    public TrapCardExecutor(TrapExecutor trapExecutor) {
        this.trapExecutor = trapExecutor;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        if (card is not TrapCard trapCard) {
            DebugLogger.Log($"TrapCard ではありません : {card.GetType().Name}");
            return;
        }

        // 罠の発動処理
        await trapExecutor.ExecuteTrapAsync(trapCard.TrapData, token);
    }
}