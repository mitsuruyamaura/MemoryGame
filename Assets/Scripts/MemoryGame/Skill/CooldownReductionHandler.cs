using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.CooldownReduction)]
public class CooldownReductionHandler : ISkillEffectHandler {
    SkillEffectType ISkillEffectHandler.EffectType => SkillEffectType.CooldownReduction;

    void ISkillEffectHandler.ApplyEffect(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.coolTime = Mathf.Max(0.05f, backpack.itemData.coolTime - value);
            DebugLogger.Log($"クールタイムを {value} 減少");
        }
    }

    void ISkillEffectHandler.RemoveEffect(ITarget target, float value) {
        if (target is BackPackInItem backpack) {
            backpack.itemData.coolTime = Mathf.Min(10.0f, backpack.itemData.coolTime + value);
            DebugLogger.Log($"クールタイムを {value} 増加");
        }
    }
}