using UnityEngine;

/// <summary>
/// 固定割合(最大HP基準)ダメージ用
/// 今回は使わない
/// </summary>
public class MaxHpRateDamageCalculator : IDamageCalculator {
    public int CalculateDamage(TrapActionData trapData, BattleManager battleManager) {
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;
        int damage = Mathf.FloorToInt(maxHp * trapData.value);
        return -damage;
    }
}