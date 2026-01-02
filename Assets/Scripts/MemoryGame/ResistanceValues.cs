using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全抵抗値の管理クラス
/// </summary>
[System.Serializable]
public class ResistanceValues {
    private readonly Dictionary<ConditionType, float> values = new();

    public void Add(ConditionType type, float value) {
        // まだ存在していない場合には初期化
        if (!values.ContainsKey(type)) {
            values[type] = 0;
        }

        values[type] += value;
    }

    public float Get(ConditionType type) {
        return values.TryGetValue(type, out var v) ? Mathf.Clamp(v, 0, 100) : 0;
    }
}