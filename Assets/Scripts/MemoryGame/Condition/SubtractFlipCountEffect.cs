/// <summary>
/// 継続的に集中力を減算する効果
/// 主に散漫のコンディションで利用する
/// </summary>
public class SubtractFlipCountEffect : IConditionEffect {
    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {
        int subtractFlipPoint = (int)data.ConditionData.value * data.StackCount.Value;
        GameData.instance.CalcFlipPoint(-subtractFlipPoint);
    }

    public void OnTurnEnd(ConditionProgressData data) {}
}