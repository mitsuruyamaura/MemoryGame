using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 各スキル/耐性の成長情報を1件分保持する
/// </summary>
[Serializable]
public class PlayerGrowthEntry {
    public int rank;
    public SkillTypeValue skillTypeValue;

    // 各スキル・耐性の成長値
    public Dictionary<SkillEffectType, float> growthValues = new();

    public PlayerGrowthEntry(SkillTypeValue skillTypeValue, int rank = 0) {
        this.skillTypeValue = skillTypeValue;
        this.rank = rank;
    }
}

/// <summary>
/// プレイヤー全体の成長データを管理する
/// </summary>
[Serializable]
public class PlayerGrowthData {
    public List<PlayerGrowthEntry> growthEntryList = new();

    /// <summary>
    /// 指定の SkillTypeValue に対応する成長エントリを取得/自動生成
    /// </summary>
    private PlayerGrowthEntry GetOrCreateEntry(SkillTypeValue type) {
        var entry = growthEntryList.FirstOrDefault(e => e.skillTypeValue.Equals(type));
        if (entry == null) {
            entry = new PlayerGrowthEntry(type);
            growthEntryList.Add(entry);
        }
        return entry;
    }

    /// <summary>
    /// 指定の効果タイプに対して値を設定(存在すれば上書き)
    /// </summary>
    public void SetGrowth(SkillTypeValue type, SkillEffectType effectType, float value) {
        var entry = GetOrCreateEntry(type);
        entry.growthValues[effectType] = value;
    }

    /// <summary>
    /// 指定の効果タイプに対して値を加算
    /// </summary>
    public void AddGrowth(SkillTypeValue type, SkillEffectType effectType, float delta) {
        var entry = GetOrCreateEntry(type);
        if (entry.growthValues.TryGetValue(effectType, out float current)) {
            entry.growthValues[effectType] = current + delta;
        } else {
            entry.growthValues[effectType] = delta;
        }
    }

    /// <summary>
    /// 指定の効果タイプの値を取得(存在しない場合は0)
    /// </summary>
    public float GetGrowth(SkillTypeValue type, SkillEffectType effectType) {
        var entry = growthEntryList.FirstOrDefault(e => e.skillTypeValue.Equals(type));
        if (entry == null) return 0f;
        return entry.growthValues.TryGetValue(effectType, out float value) ? value : 0f;
    }

    /// <summary>
    /// 指定のスキルの成長ランクを取得
    /// </summary>
    public int GetRank(SkillTypeValue type) {
        var entry = growthEntryList.FirstOrDefault(e => e.skillTypeValue.Equals(type));
        return entry?.rank ?? 0;
    }

    /// <summary>
    /// 成長ランクを加算
    /// </summary>
    public void AddRank(SkillTypeValue type, int add = 1) {
        var entry = GetOrCreateEntry(type);
        entry.rank += add;
    }

    /// <summary>
    /// デバッグ出力
    /// </summary>
    public void PrintAll() {
        foreach (var entry in growthEntryList) {
            DebugLogger.Log($"[Rank {entry.rank}] {entry.skillTypeValue}");
            foreach (var kvp in entry.growthValues) {
                DebugLogger.Log($"  - {kvp.Key}: {kvp.Value}");
            }
        }
    }
}