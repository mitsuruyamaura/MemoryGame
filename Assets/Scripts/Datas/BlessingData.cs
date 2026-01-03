using System;
using UnityEngine;

[System.Serializable]
public class BlessingData : IMasterData, IInfoView, IHasIcon {
    public int id;
    public string name;
    public BlessingType type;
    public string desc;
    public float value;
    public BlessingValueType valueType;
    public Rarity rarity;
    public int weight;
    public int exp;
    public int stack;
    public int implemented;   // 1 が実装済でランダム選択の対象

    public int Id => id;

    public Rarity Rarity => rarity;

    public string Name => name;

    public string Description => desc;
    public Sprite GetIcon() {
        return Resources.Load<Sprite>("Blessing/" + Id);
    }

    public BlessingData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        type = ParseBlessingType(datas[2]);
        desc = datas[3];
        value = float.Parse(datas[4]);
        valueType = ParseBlessingValueType(datas[5]);
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[6]);
        weight = int.Parse(datas[7]);
        exp = int.Parse(datas[8]);
        stack = int.Parse(datas[9]);
        implemented = int.Parse(datas[10]);
    }

    public static BlessingType ParseBlessingType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out BlessingType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown BlessingType: {typeStr}");
        return BlessingType.None;
    }

    public static BlessingValueType ParseBlessingValueType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out BlessingValueType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown BlessingVlaueType: {typeStr}");
        return BlessingValueType.None;
    }

    public bool TryGetConditionType(out ConditionType conditionType) {
        return BlessingValueTypeMapper.TryToCondition(valueType, out conditionType);
    }

    public bool TryGetCardType(out CardEventType cardType) {
        return BlessingValueTypeMapper.TryToCard(valueType, out cardType);
    }
}
