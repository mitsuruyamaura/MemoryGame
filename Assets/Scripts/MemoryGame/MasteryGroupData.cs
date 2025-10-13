using System;
using System.Linq;

public enum MasteryType {
    hit,
    cut,
    thrust,
    shoot,
    magic,
    defense,
    life,
    protection,
    reaction
}

[Serializable]
public class MasteryGroupData : IMasterData {
    public int id;
    public MasteryType masteryType;
    public int group_type;
    public int[] targetItemTypes;
    public ConditionType conditionType;
    public string desc;

    public int Id => id;


    public MasteryGroupData(string[] datas) {
        id = int.Parse(datas[0]);
        masteryType = (MasteryType)Enum.Parse(typeof(MasteryType), datas[1]);
        group_type = int.Parse(datas[2]);
        targetItemTypes = datas[3].Split('/').Select(id => int.Parse(id)).ToArray();
        conditionType = (ConditionType)Enum.Parse(typeof(ConditionType), datas[4]);
        desc = datas[5];
    }
}