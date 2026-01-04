/// <summary>
/// アイテム操作不可、アイテム獲得不可の両方の効果
/// 主に呪詛で利用する
/// </summary>
public class ItemRestrictionEffect : IConditionEffect, IRestrictItemUse, IRestrictItemObtain, IOnConditionEnter, IOnConditionExit {
    public bool CanUseItem() {
        return false;
    }

    public bool CanObtainItem() {
        return false;
    }

    public void OnEnter(ConditionProgressData data) {
        data.conditionContext.StageUIManager.ShowItemLockShade();
    }

    public void OnExit(ConditionProgressData data) {
        data.conditionContext.StageUIManager.HideItemLockShade();
    }

    public void OnApply(ConditionProgressData data) { }

    public void OnMisstep(ConditionProgressData data) { }

    public void OnTurnEnd(ConditionProgressData data) { }
}