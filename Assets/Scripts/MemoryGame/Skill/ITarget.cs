public interface ITarget {
    ITarget GetTarget();

    string Name { get; }
    void ApplyModifier(SkillEffectType effectType, float value);
    void RemoveModifier(SkillEffectType effectType, float value);
}