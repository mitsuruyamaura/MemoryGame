using R3;
using System.Collections.Generic;

/// <summary>
/// プレイヤー、敵共通
/// </summary>
[System.Serializable]
public class CombatData {
    public SerializableReactiveProperty<int> Hp = new();
    public SerializableReactiveProperty<int> MaxInventorySize = new();


    // インベントリ

    // バフ・デバフ
    public List<PlayerConditionBase> conditionList = new();

    public CombatData(int hp, int defaultInventorySize) {
        Hp = new(hp);
        MaxInventorySize.Value = defaultInventorySize;
    }
}