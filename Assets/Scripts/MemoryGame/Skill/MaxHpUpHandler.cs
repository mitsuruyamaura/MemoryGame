using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.MaxHpUp)]
public class MaxHpUpHandler : ISkillEffectHandler {
    SkillEffectType ISkillEffectHandler.EffectType => SkillEffectType.MaxHpUp;

    void ISkillEffectHandler.ApplyEffect(ITarget target, float value) {
        //if (target is PlayerUnit player) {
        //    player.MaxHp += (int)value;
        //} else if (target is Equipment equip) {
        //    equip.Durability += value;
        //}
    }

    void ISkillEffectHandler.RemoveEffect(ITarget target, float value) {
        //if (target is PlayerUnit player) {
        //    player.MaxHp -= (int)value;
        //} else if (target is Equipment equip) {
        //    equip.Durability -= value;
        //}
    }
}