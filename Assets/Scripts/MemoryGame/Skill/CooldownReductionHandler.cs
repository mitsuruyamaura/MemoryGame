using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.CooldownReduction)]
public class CooldownReductionHandler : SkillEffectHandlerBase {

    public override SkillEffectType EffectType => SkillEffectType.CooldownReduction;

    protected override void ApplyToTarget(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.coolTime = Mathf.Max(0.05f, backpack.itemData.coolTime - value);
            DebugLogger.Log($"クールタイムを {value} 減少");
        }
    }

    protected override void RemoveFromTarget(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.coolTime = Mathf.Min(10.0f, backpack.itemData.coolTime + value);
            DebugLogger.Log($"クールタイムを {value} 増加");
        }
    }
}