using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// デバフのコンディション付与トラップの実行クラス
/// </summary>
public class PlayerDebuffTrapExecutor : ITrap {
    private ConditionManager conditionManager;
    private MemoryGameManager memoryGameManager;

    public PlayerDebuffTrapExecutor(ConditionManager conditionManager, MemoryGameManager memoryGameManager) {
        this.conditionManager = conditionManager;
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        ConditionData conditionData = DataBaseManager.instance.GetConditionData(trapData.conditionType);
        float conditionPowerMultiplier = memoryGameManager.GetConditionPowerMultiplierByFloor();
        conditionManager.AddConditionList(conditionData, conditionPowerMultiplier, 1);

        await UniTask.Yield(token);
    }
}
