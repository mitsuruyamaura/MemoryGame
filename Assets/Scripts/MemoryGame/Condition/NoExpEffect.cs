/// <summary>
/// 獲得経験値(ソウルポイント)を 0 にする
/// 主に封印のコンディションで利用する
/// </summary>
public class NoExpEffect : IConditionEffect, IExpModifier {
    public int ModifyExp(int baseExp) => 0;

    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {}

    public void OnTurnEnd(ConditionProgressData data) {}
}