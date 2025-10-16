using System;

[System.Serializable]
public struct SkillTypeValue {
    public SkillTypeCategory category;
    public int value; // Enum の int 値を格納

    public SkillTypeValue(SkillTypeCategory category, int value) {
        this.category = category;
        this.value = value;
    }

    public override string ToString() {
        return category switch {
            SkillTypeCategory.Mastery => ((MasteryType)value).ToString(),
            SkillTypeCategory.Condition => ((ConditionType)value).ToString(),
            SkillTypeCategory.Reaction => ((ReactionType)value).ToString(),
            _ => "Unknown"
        };
    }

    // ← これが重要。文字列から自動判定して変換
    public static bool TryParse(string str, out SkillTypeValue result) {
        // 大文字・小文字無視
        str = str.Trim();

        // まず MasteryType として判定
        if (Enum.TryParse(str, true, out MasteryType mastery)) {
            result = new SkillTypeValue(SkillTypeCategory.Mastery, (int)mastery);
            return true;
        }

        // 次に ConditionType
        if (Enum.TryParse(str, true, out ConditionType condition)) {
            result = new SkillTypeValue(SkillTypeCategory.Condition, (int)condition);
            return true;
        }

        // 最後に ReactionType
        if (Enum.TryParse(str, true, out ReactionType reaction)) {
            result = new SkillTypeValue(SkillTypeCategory.Reaction, (int)reaction);
            return true;
        }

        // どれにも該当しない場合
        result = default;
        return false;
    }

    // 比較のためのオーバーライド
    public override bool Equals(object obj) {
        return obj is SkillTypeValue other &&
               category == other.category &&
               value == other.value;
    }

    public override int GetHashCode() {
        return HashCode.Combine(category, value);
    }
}