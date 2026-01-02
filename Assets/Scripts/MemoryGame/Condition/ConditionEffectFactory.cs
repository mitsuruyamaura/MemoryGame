/// <summary>
/// コンディションの効果(実処理)のファクトリークラス
/// </summary>
public class ConditionEffectFactory {
    private BattleManager battleManager;

    public ConditionEffectFactory(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public IConditionEffect Create(ConditionType conditionType) {
        return conditionType switch {
            ConditionType.Poison => new DamageEffect(battleManager),
            ConditionType.Distraction => new SubtractFlipCountEffect(),
            ConditionType.Hallucination => new NoFlipCountEffect(),
            ConditionType.Seal => new NoExpEffect(),
            _ => null
        };
    }
}