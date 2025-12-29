/// <summary>
/// コンディションの効果適用用
/// 条件に合わせてメソッドを追加する
/// </summary>
public interface IConditionEffect {
    void OnApply(ConditionProgressData data);
    void OnMisstep(ConditionProgressData data);
    void OnTurnEnd(ConditionProgressData data);
}