using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 継続的にダメージを与える効果
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

        // Hp が 0 になったらゲームオーバー
        if (battleManager.PlayerHP.Value <= 0) {
            battleManager.ForceGameEndAsync().Forget();
        }
    }

    public void OnTurnEnd(ConditionProgressData data) {}
}