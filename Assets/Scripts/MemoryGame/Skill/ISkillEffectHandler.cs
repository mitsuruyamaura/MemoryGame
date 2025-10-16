public interface ISkillEffectHandler {
    SkillEffectType EffectType { get; }
    void ApplyEffect(ITarget target, float value);   // 効果を適用
    void RemoveEffect(ITarget target, float value);  // 効果を解除（任意）
}