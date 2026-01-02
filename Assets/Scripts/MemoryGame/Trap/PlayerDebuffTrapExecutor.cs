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

        // 対象となる抵抗率と乱数を取得
        float resistance = GameData.instance.charaStatus.resistanceValues.Get(conditionData.conditionType);
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        DebugLogger.Log($"randomValue {randomValue} : resistance {resistance}");

        // 成否判定
        bool resistSucceeded = randomValue < resistance;

        // 抵抗成功時は何もしない
        if (resistSucceeded) {
            return;
        }

        // 抵抗に失敗したのでコンディション付与する
        float conditionPowerMultiplier = memoryGameManager.GetConditionPowerMultiplierByFloor();
        conditionManager.AddConditionList(conditionData, conditionPowerMultiplier, 1);

        await UniTask.Yield(token);
    }
}