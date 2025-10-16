public class SkillEffectExecutor {
    public void Execute(SkillEffectType type, ITarget target, float value) {
        var handler = SkillEffectHandlerFactory.GetHandler(type);
        handler?.ApplyEffect(target, value);
    }
}