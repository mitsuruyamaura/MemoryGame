using UnityEngine;

/// <summary>
/// ダメージを与える効果
/// 主に猛毒などのコンディションで利用する
/// </summary>
public class DamageEffect : IConditionEffect {
    private BattleManager battleManager;

    public DamageEffect(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {
        float damageRate = data.ConditionData.value * data.StackCount.Value;
        int damage = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * damageRate);

        battleManager.UpdatePlayerHp(-damage, EffectType.Magic, false);
    }

    public void OnTurnEnd(ConditionProgressData data) {}
}