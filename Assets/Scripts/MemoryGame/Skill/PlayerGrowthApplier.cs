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

    public void ApplyAllGrowthEffects() {
        foreach (PlayerGrowthEntry entry in growthData.growthEntryList) {
            foreach (var kvp in entry.growthValues) {
                ISkillEffectHandler handler = SkillEffectHandlerFactory.GetHandler(kvp.Key);
                handler?.ApplyEffect(target, kvp.Value);
            }
        }
    }
}