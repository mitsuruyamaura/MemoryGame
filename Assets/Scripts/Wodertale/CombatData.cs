using R3;

/// <summary>
/// プレイヤー、敵共通
/// </summary>
[System.Serializable]
public class CombatData {
    public SerializableReactiveProperty<int> Hp = new();
    public SerializableReactiveProperty<int> MaxInventorySize = new();

    public CombatData(int hp, int defaultInventorySize) {
        Hp = new(hp);
        MaxInventorySize.Value = defaultInventorySize;
    }
}