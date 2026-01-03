using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// HP 全回復イベント実行クラス
/// </summary>
public class HealAllEventExecutor : IEventExecutor {
    private BattleManager battleManager;
    private ConditionManager conditionManager;
    private MemoryGameManager memoryGameManager;

    public HealAllEventExecutor(BattleManager battleManager, ConditionManager conditionManager, MemoryGameManager memoryGameManager) {
        this.battleManager = battleManager;
        this.conditionManager = conditionManager;
        this.memoryGameManager = memoryGameManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        int healPower = GameData.instance.charaStatus.MaxHp.Value;
        DebugLogger.Log($"healPower : {healPower}");

        int currentHp = battleManager.PlayerHP.Value;
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;

        if (currentHp < maxHp) {
            battleManager.UpdatePlayerHp(healPower, EffectType.Heal, false);
        } else {
            // Hp 最大値以上なら、シールドに加算
            battleManager.UpdatePlayerShieldHp(healPower, false);
        }

        // 全デバフ解除
        conditionManager.RemoveAllDebuffs();

        // TODO ステータスバフ
        if (blessingData.TryGetConditionType(out var conditionType)) {

            ConditionData conditionData = DataBaseManager.instance.GetConditionData(conditionType);
            float conditionPowerMultiplier = memoryGameManager.GetConditionPowerMultiplierByFloor();

            conditionManager.AddConditionList(conditionData, conditionPowerMultiplier, 1);
        }
        

        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}