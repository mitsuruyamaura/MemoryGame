using System;

[System.Serializable]
public class ConditionData {
    public int id;
    public ConditionPolarity conditionPolarity;
    public string name;
    public string desc;
    public float value;
    public ConditionType conditionType;
    public int conditionPower;            // 強度
    public int maxConditionPower;         // スタック時の最大強度
    public int maxStackCount;             // 最大スタック数
    public int implemented;


    public ConditionData(string[] datas) {
        id = int.Parse(datas[0]);
        conditionPolarity = (ConditionPolarity)Enum.Parse(typeof(ConditionPolarity), datas[1]);
        name = datas[2];
        desc = datas[3];
        value = float.Parse(datas[4]);
        conditionType = (ConditionType)Enum.Parse(typeof(ConditionType), datas[5]);
        conditionPower = int.Parse(datas[6]);
        maxConditionPower = int.Parse(datas[7]);
        maxStackCount = int.Parse(datas[8]);
        implemented = int.Parse(datas[9]);
    }

    public static string GetConditionName(ConditionType conditionType) {
        return conditionType switch {
            ConditionType.Hallucination => "幻覚",
            ConditionType.Poison => "猛毒",
            ConditionType.Distraction => "散漫",
            ConditionType.Seal => "封印",
            ConditionType.Curse => "呪詛",
            ConditionType.Fortitude => "不屈",
            _ => ""
        };
    }
}