using UnityEngine;

/// <summary>
/// 割合ダメージ用
/// 今回は使わない
/// </summary>
public class RateDamageCalculator : IDamageCalculator {
    public int CalculateDamage(TrapActionData trapData, BattleManager battleManager) {
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;
        int currentHp = battleManager.PlayerHP.Value;

        int targetHp = Mathf.FloorToInt(maxHp * (1f - trapData.value));
        targetHp = Mathf.Min(targetHp, currentHp);

        return targetHp - currentHp; // delta
    }
}