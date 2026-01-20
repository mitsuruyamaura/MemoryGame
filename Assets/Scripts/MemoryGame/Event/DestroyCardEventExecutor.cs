using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 敵かトラップカードの破棄イベント実行クラス
/// </summary>
public class DestroyCardEventExecutor : IEventExecutor {
    private MemoryGameManager memoryGameManager;
    public DestroyCardEventExecutor(MemoryGameManager memoryGameManager) {
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        // 指定回数だけ、敵かトラップカードを破棄
        for (int i = 0; i < blessingData.value; i++) {
            bool destroyed = await memoryGameManager.ChooseDestroyEnemyOrTrapCardAsync(blessingData, ReleaseType.Distribute);

            // 破棄されなかった(破棄可能なカードがなかった)場合にはループを抜ける
            if (!destroyed) {
                break;
            }
        }
    }
}