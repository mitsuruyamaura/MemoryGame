/// <summary>
/// ダメージ処理の抽象化用
/// 今回は使わない
/// </summary>
public interface IDamageCalculator {
    int CalculateDamage(TrapActionData trapData, BattleManager battleManager);
}