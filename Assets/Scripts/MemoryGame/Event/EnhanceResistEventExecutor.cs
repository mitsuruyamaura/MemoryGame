using Cysharp.Threading.Tasks;
using System.Threading;

public class EnhanceResistEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        DebugLogger.Log($"Enhance Resist : {blessingData.value}");

        // TODO SE

        await UniTask.Yield(token);
    }
}