using System;

[System.Serializable]
public class TrapActionData : IMasterData {
    public int id;
    public int trapId;
    public int order;
    public TrapActionType trapActionType;
    public float value;
    public ConditionType conditionType;
    public TrapDamageType trapDamageType;
    public CardEventType fromCardEventType;
    public CardEventType toCardEventType;
    public int implemented;

    public int Id => id;

    public TrapActionData(string[] datas) {
        id = int.Parse(datas[0]);
        trapId = int.Parse(datas[1]);
        order = int.Parse(datas[2]);
        trapActionType = ParseTrapActionType(datas[3]);
        value = float.Parse(datas[4]);
        conditionType = ParseConditionType(datas[5]);
        trapDamageType = ParseTrapDamageType(datas[6]);
        fromCardEventType = ParseCardEventType(datas[7]);
        toCardEventType = ParseCardEventType(datas[8]);
        implemented = int.Parse(datas[9]);
    }

    public static TrapActionType ParseTrapActionType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out TrapActionType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown TrapActionType: {typeStr}");
        return TrapActionType.None;
    }

    public static ConditionType ParseConditionType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out ConditionType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown TrapVlaueType: {typeStr}");
        return ConditionType.None;
    }

    public static TrapDamageType ParseTrapDamageType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out TrapDamageType result)) {
            return result;
        }

        DebugLogger.Log($"Unknown TrapDamageType: {typeStr}");
        return TrapDamageType.None;
    }

    public static CardEventType ParseCardEventType(string typeStr) {
        if (Enum.TryParse(typeStr, ignoreCase: true, out CardEventType result)) {
            return result;
        }
        DebugLogger.Log($"Unknown CardEventType: {typeStr}");
        return CardEventType.Stairs;
    }
}