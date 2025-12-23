using Cysharp.Threading.Tasks;
using System.Threading;

public class ClassRankUpEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        await UniTask.Yield(token);
    }
}