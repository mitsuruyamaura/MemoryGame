using Cysharp.Threading.Tasks;
using System.Threading;

public class DestroyCardEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        MemoryGameManager memoryGameManager = UnityEngine.GameObject.FindFirstObjectByType<MemoryGameManager>();

        // 指定回数だけ、敵かトラップカードを破棄
        for (int i = 0; i < blessingData.value; i++) {
            await memoryGameManager.ChooseDestroyEnemyOrTrapCardAsync(blessingData);
            await UniTask.Delay(1000);
        }
    }
}