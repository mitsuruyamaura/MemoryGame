using System.Collections.Generic;

/// <summary>
/// コンディションの効果(実処理)のファクトリークラス
/// </summary>
public class ConditionEffectFactory {
    private BattleManager battleManager;

    public ConditionEffectFactory(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    /// <summary>
    /// コンディションの効果を生成
    /// </summary>
    /// <param name="conditionType"></param>
    /// <returns></returns>
    public IConditionEffect CreateEffect(ConditionType conditionType) {
        return conditionType switch {
            ConditionType.Poison => new DamageEffect(battleManager),
            ConditionType.Distraction => new SubtractFlipCountEffect(),
            ConditionType.Hallucination => new NoFlipCountEffect(),
            ConditionType.Seal => new NoExpEffect(),
            ConditionType.Curse => new ItemRestrictionEffect(),
            _ => null
        };
    }

    /// <summary>
    /// 複数の効果を生成したい場合
    /// 現在未使用
    /// </summary>
    /// <param name="conditionType"></param>
    /// <returns></returns>
    public List<IConditionEffect> CreateEffects(ConditionType conditionType) {
        return conditionType switch {
            ConditionType.Poison => new() { new DamageEffect(battleManager) },
            ConditionType.Distraction => new() { new SubtractFlipCountEffect() },
            ConditionType.Hallucination => new() { new NoFlipCountEffect() },
            ConditionType.Seal => new() { new NoExpEffect() },
            ConditionType.Curse => new() { new ItemRestrictionEffect() },
            _ => new()
        };
    }
}