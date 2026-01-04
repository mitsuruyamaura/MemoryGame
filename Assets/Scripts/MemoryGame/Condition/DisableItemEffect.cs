/// <summary>
/// アイテムが操作できない
/// 単体利用用。今回は使わない
/// </summary>
public class DisableItemEffect : IConditionEffect, IRestrictItemUse {
    public bool CanUseItem() {
        return false;
    }

    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {}

    public void OnTurnEnd(ConditionProgressData data) {}
}
