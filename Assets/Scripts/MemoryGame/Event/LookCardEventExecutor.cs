using Cysharp.Threading.Tasks;
using System.Threading;

public class LookCardEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        MemoryGameManager memoryGameManager = UnityEngine.GameObject.FindFirstObjectByType<MemoryGameManager>();
        await memoryGameManager.FaceUpCardsByTargetTypeAsync(blessingData);
        await UniTask.Yield(token);
    }
}