using Cysharp.Threading.Tasks;
using System.Threading;

public class EnhanceItemEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        PlayerInventoryManager.instance.EnhanceItem(blessingData);
        await UniTask.Yield(token);
    }
}