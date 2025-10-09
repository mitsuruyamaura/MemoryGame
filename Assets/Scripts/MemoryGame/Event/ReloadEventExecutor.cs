using Cysharp.Threading.Tasks;
using System.Threading;

public class ReloadEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        await UniTask.Yield(token);
    }
}