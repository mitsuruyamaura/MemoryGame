using Cysharp.Threading.Tasks;
using System.Threading;

public class EnhanceResistEventExecutor : IEventExecutor {
    private BattleManager battleManager;
    public EnhanceResistEventExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        DebugLogger.Log($"Enhance Resist : {blessingData.value}");

        // TODO SE

        await UniTask.Yield(token);
    }
}