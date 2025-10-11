using System;
using UnityEngine;

[System.Serializable]
public class TrapData : IMasterData, IInfoView, IHasIcon {
    public int id;
    public string name;
    public TrapType type;
    public string desc;
    public float value;
    public TrapValueType valueType;
    public Rarity rarity;
    public int weight;
    public int exp;
    public int implemented;

    public int Id => id;
    public Rarity Rarity => rarity;
    public string Name => name;

    public string Description => desc;
    public Sprite GetIcon() {
        return Resources.Load<Sprite>("Trap/" + Id);
    }

    public TrapData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        type = ParseTrapType(datas[2]);
        desc = datas[3];
        value = float.Parse(datas[4]);
        valueType = ParseTrapValueType(datas[5]);
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[6]);
        weight = int.Parse(datas[7]);
        exp = int.Parse(datas[8]);
        implemented = int.Parse(datas[9]);
    }

    public static TrapType ParseTrapType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out TrapType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown TrapType: {typeStr}");
        return TrapType.None;
    }

    public static TrapValueType ParseTrapValueType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out TrapValueType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown TrapVlaueType: {typeStr}");
        return TrapValueType.None;
    }
}