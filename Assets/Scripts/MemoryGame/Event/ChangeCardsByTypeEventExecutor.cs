using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 指定した種類のカードを別の種類のカードに変更するイベント実行クラス
/// 現在は CardEvetType を TreasureChest に変更するのみ対応(汎用的にする場合には、BlessingData に新しいカラムを追加する)
/// </summary>
public class ChangeCardsByTypeEventExecutor : IEventExecutor {
    private MemoryGameManager memoryGameManager;

    public ChangeCardsByTypeEventExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        CardEventType fromCardEventType = MemoryGameManager.ConvertCardEventTypeByBlessingValueType(blessingData.valueType);
        CardEventType toCardEventType = CardEventType.TreasureChest;

        memoryGameManager.ReplaceCardsByType(fromCardEventType, toCardEventType);
        await UniTask.Yield(token);
    }
}