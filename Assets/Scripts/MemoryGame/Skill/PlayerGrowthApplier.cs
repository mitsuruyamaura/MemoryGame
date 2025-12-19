/// <summary>
/// 成長データの適用クラス
/// </summary>
public class PlayerGrowthApplier {
    private readonly PlayerGrowthData growthData;
    private readonly ITarget target;

    public PlayerGrowthApplier(PlayerGrowthData growthData, ITarget target) {
        this.growthData = growthData;
        this.target = target;
    }

    ///// <summary>
    ///// 登録されているすべての成長効果を適用
    ///// </summary>
    //public void ApplyAllGrowthEffects() {
    //    foreach (PlayerGrowthEntry entry in growthData.growthEntryList) {
    //        foreach (var kvp in entry.growthValues) {
    //            SkillEffectType effectType = kvp.Key;
    //            float value = kvp.Value;

    //            ITarget baseTarget = selector.GetTarget(player);

    //            SkillEffectHandlerBase handler = SkillEffectHandlerFactory.GetHandler(effectType);
    //            if (handler != null) {
    //                handler.ApplyEffect(baseTarget, value);
    //                DebugLogger.Log($"[Apply] {effectType} +{value}");
    //            } else {
    //                DebugLogger.Log($"[Warning] No handler for {effectType}");
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// 登録されているすべての成長効果を解除（必要に応じて）
    ///// </summary>
    //public void RemoveAllGrowthEffects() {
    //    foreach (PlayerGrowthEntry entry in growthData.growthEntryList) {
    //        foreach (var kvp in entry.growthValues) {
    //            SkillEffectType effectType = kvp.Key;
    //            float value = kvp.Value;

    //            ITarget baseTarget = selector.GetTarget(player);

    //            SkillEffectHandlerBase handler = SkillEffectHandlerFactory.GetHandler(effectType);
    //            if (handler != null) {
    //                handler.RemoveEffect(baseTarget, value);
    //                DebugLogger.Log($"[Remove] {effectType} -{value}");
    //            }
    //        }
    //    }
    //}
}