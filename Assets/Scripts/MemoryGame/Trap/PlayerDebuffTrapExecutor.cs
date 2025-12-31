using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// デバフのコンディション付与トラップの実行クラス
/// </summary>
public class PlayerDebuffTrapExecutor : ITrapEffect {
    private ConditionManager conditionManager;
    private MemoryGameManager memoryGameManager;

    public PlayerDebuffTrapExecutor(ConditionManager conditionManager, MemoryGameManager memoryGameManager) {
        this.conditionManager = conditionManager;
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        ConditionData conditionData = DataBaseManager.instance.GetConditionData(trapActionData.conditionType);
        float conditionPowerMultiplier = memoryGameManager.GetConditionPowerMultiplierByFloor();
        conditionManager.AddConditionList(conditionData, conditionPowerMultiplier, 1);

        await UniTask.Yield(token);
    }
}
