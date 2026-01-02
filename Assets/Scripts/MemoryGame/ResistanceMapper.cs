using System;
using System.Collections.Generic;

public static class ResistanceMapper {

    /// <summary>
    /// マッピング
    /// </summary>
    private static readonly (ConditionType type, Func<IResistanceSource, float> getter)[] map = {
        (ConditionType.Hallucination, sourse => sourse.HallucinationResist),
        (ConditionType.Poison, sourse => sourse.PoisonResist),
        (ConditionType.Distraction, sourse => sourse.DistractionResist),
        (ConditionType.Seal, sourse => sourse.SealResist),
        (ConditionType.Curse, sourse => sourse.CurseResist),
    };

    /// <summary>
    /// 適用処理
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    public static void Apply(IResistanceSource source, ResistanceValues values) {
        foreach (var (type, getter) in map) {
            float value = getter(source);
            if (value != 0) {
                values.Add(type, value);
            }
        }
    }

    /// <summary>
    /// 抵抗値の再計算
    /// </summary>
    /// <param name="equippedItems"></param>
    /// <param name="learnedSkills"></param>
    /// <returns></returns>
    public static ResistanceValues RebuildResistanceValues(List<ItemData> equippedItems, List<MemoriaSkillData> learnedSkills = null) {
        ResistanceValues values = new();

        foreach (ItemData item in equippedItems) {
            Apply(item, values);
        }

        //foreach (var skill in learnedSkills)
        //    ResistanceMapper.Apply(skill, values);

        return values;
    }
}