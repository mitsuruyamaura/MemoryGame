using UnityEngine.Scripting;

[Preserve]
[SkillEffect(SkillEffectType.MaxHpUp)]
public class MaxHpUpHandler : SkillEffectHandlerBase {

    public override SkillEffectType EffectType => SkillEffectType.MaxHpUp;


    protected override void ApplyToTarget(ITarget target, float value) {
        //if (target is PlayerUnit player) {
        //    player.MaxHp += (int)value;
        //} else if (target is Equipment equip) {
        //    equip.Durability += value;
        //}
    }

    protected override void RemoveFromTarget(ITarget target, float value) {
        //if (target is PlayerUnit player) {
        //    player.MaxHp -= (int)value;
        //} else if (target is Equipment equip) {
        //    equip.Durability -= value;
        //}
    }
}