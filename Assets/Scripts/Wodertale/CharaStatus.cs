using System;
using System.Collections.Generic;
using System.Linq;
using R3;

[System.Serializable]
public class StatusValue {
    public StatusType statusType;
    public SerializableReactiveProperty<int> statusValue = new();

    public StatusValue(StatusType statusType, int value) {
        this.statusType = statusType;
        statusValue.Value = value;
    }

    public override string ToString() {
        return $"{statusType}: {statusValue}";
    }
}

[System.Serializable]
public class CharaStatus {
    public int level;
    public long exp;
    public SerializableReactiveProperty<int> MaxHp = new();
    public List<StatusValue> statusValueList = new();
    public ResistanceValues resistanceValues = new();

    public CharaStatus(int maxHp) {
        level = 1;
        exp = 0;
        MaxHp.Value = maxHp;
        //DebugLogger.Log($"MaxHp : {MaxHp.Value}");

        statusValueList = new() {
            new(StatusType.Strength, 0),
            new(StatusType.Intelligence, 0),
            new(StatusType.Dexterity, 0),
            new(StatusType.Charm, 0),
            new(StatusType.Luck, 0)
        };

        resistanceValues = new();
    }

    public CharaStatus(List<int> values) {
        level = 1;
        exp = 0;

        for (int i = 0; i < values.Count; i++) {
            // i が有効な StatusType であることを確認
            if (Enum.IsDefined(typeof(StatusType), i)) {
                statusValueList.Add(new((StatusType)i, values[i]));
            } else {
                throw new ArgumentOutOfRangeException($"Invalid StatusType cardIndex: {i}");
            }
        }
    }

    public StatusValue GetStatusValue(StatusType searchStatusType) {
        return statusValueList.FirstOrDefault(data => data.statusType == searchStatusType);
    }


    public void SetStatusValue(StatusType statusType, int value) {
        StatusValue statusValue = GetStatusValue(statusType);
        if (statusValue != null) {
            statusValue.statusValue.Value = value;
        } else {
            statusValueList.Add(new(statusType, value));
        }
    }

    /// <summary>
    /// PlayFab から新規作成用
    /// </summary>
    /// <returns></returns>
    public CharaStatus Create() {
        return new(50);
    }

    /// <summary>
    /// アイテムによる強化分をステータスに反映
    /// </summary>
    /// <param name="itemData"></param>
    public void CalculateCharaStatus(ItemData itemData) {
        for (int i = 0; i < itemData.statusTypes.Length; i++) {
            for (int j = 0; j < statusValueList.Count; j++) {
                if (itemData.statusTypes[i] == statusValueList[j].statusType) {
                    statusValueList[j].statusValue.Value += itemData.requiredValues[i];
                }
            }
        }
    }

    /// <summary>
    /// 最大 Hp の算出
    /// </summary>
    /// <param name="hpBonus"></param>
    public void CalculateMaxHp(int hpBonus) {
        MaxHp.Value += hpBonus;
    }

    /// <summary>
    /// リアクション用のボーナスポイントの算出
    /// </summary>
    /// <param name="searchStatusType"></param>
    /// <returns></returns>
    public float GetReactionBonusRate(StatusType searchStatusType) {
        return searchStatusType switch {
            StatusType.Strength => statusValueList[0].statusValue.Value / ConstData.REACTION_BASE,      // 受け流し
            StatusType.Intelligence => statusValueList[1].statusValue.Value / ConstData.REACTION_BASE,  // 吸収
            StatusType.Dexterity => statusValueList[2].statusValue.Value / ConstData.REACTION_BASE,     // 反射
            StatusType.Charm => statusValueList[3].statusValue.Value / ConstData.REACTION_BASE,         // 交渉
            _ => 0
        };
    }

    /// <summary>
    /// 全抵抗値の更新
    /// </summary>
    /// <param name="itemDataList"></param>
    /// <returns></returns>
    public ResistanceValues UpdateResistanceValues(List<ItemData> itemDataList) {
        resistanceValues = ResistanceMapper.RebuildResistanceValues(itemDataList);
        return resistanceValues;
    }
}