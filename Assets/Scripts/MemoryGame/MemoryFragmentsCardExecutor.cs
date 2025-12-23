using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 思い出の断片カードの実行クラス
/// </summary>
public class MemoryFragmentsCardExecutor : ICardExecutor {
    MemoriaRankUpExecutor memoriaRankUpExecutor;

    public MemoryFragmentsCardExecutor(MemoriaRankUpExecutor memoriaRankUpExecutor) {
        this.memoriaRankUpExecutor = memoriaRankUpExecutor;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        if (card is not MemoryFragmentsCard memoryFragmentsCard) {
            DebugLogger.Log($"MemoryFragmentsCard ではありません : {card.GetType().Name}");
            return;
        }

        await memoriaRankUpExecutor.ExecuteMemoriaRankUpAsync(memoryFragmentsCard.MemoryStoneData, token);
    }
}