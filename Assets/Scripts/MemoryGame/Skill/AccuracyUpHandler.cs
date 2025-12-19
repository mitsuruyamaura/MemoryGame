using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.AccuracyUp)]
public class AccuracyUpHandler : SkillEffectHandlerBase {

    public override SkillEffectType EffectType => SkillEffectType.AccuracyUp;

    protected override void ApplyToTarget(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.accuracy += value;
            DebugLogger.Log($"命中率を {value} 増加");
        }

        target.ApplyModifier(EffectType, value);
        DebugLogger.Log($"命中率を {value} 増加");
    }

    protected override void RemoveFromTarget(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.accuracy -= value;
            DebugLogger.Log($"命中率を {value} 減少");
        }

        target.RemoveModifier(EffectType, value);
        DebugLogger.Log($"命中率を {value} 減少");
    }
}