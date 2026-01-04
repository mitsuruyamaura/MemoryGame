/// <summary>
/// アイテムが獲得できない
/// 単体利用用。今回は使わない
/// </summary>
public class UnobtainableItemEffect : IConditionEffect, IRestrictItemObtain {
    public bool CanObtainItem() {
        return false;
    }

    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {}

    public void OnTurnEnd(ConditionProgressData data) {}
}