using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.AccuracyUp)]
public class AccuracyUpHandler : ISkillEffectHandler {
    SkillEffectType ISkillEffectHandler.EffectType => SkillEffectType.AccuracyUp;

    void ISkillEffectHandler.ApplyEffect(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.accuracy += value;
            DebugLogger.Log($"命中率を {value} 増加");
        }
    }

    void ISkillEffectHandler.RemoveEffect(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.accuracy -= value;
            DebugLogger.Log($"命中率を {value} 減少");
        }
    }
}