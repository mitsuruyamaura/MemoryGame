using System.Collections.Generic;
using UnityEngine;

public static class BlessingValueTypeMapper {

    private static readonly Dictionary<BlessingValueType, ConditionType> conditionMap = new() {
        { BlessingValueType.Fortitude, ConditionType.Fortitude },

    };

    private static readonly Dictionary<BlessingValueType, CardEventType> cardMap = new() {
        { BlessingValueType.Key, CardEventType.Stairs},

    };


    public static bool TryToCondition(
        BlessingValueType valueType,
        out ConditionType conditionType) {
        return conditionMap.TryGetValue(valueType, out conditionType);
    }

    public static bool TryToCard(
        BlessingValueType valueType,
        out CardEventType cardType) {
        return cardMap.TryGetValue(valueType, out cardType);
    }
}