using Cysharp.Threading.Tasks;
using System.Threading;

public class EnhanceRandomItemsExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        PlayerInventoryManager.instance.EnhanceRandomItems(blessingData);
        await UniTask.Yield(token);
    }
}