using System;

/// <summary>
/// 各クラスに「自分が何の効果を担当しているか」を付ける
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SkillEffectAttribute : Attribute {
    public SkillEffectType EffectType { get; }
    public SkillEffectAttribute(SkillEffectType effectType) {
        EffectType = effectType;
    }
}