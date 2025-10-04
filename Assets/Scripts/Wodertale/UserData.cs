using System.Collections.Generic;
using ObservableCollections;
using R3;

[System.Serializable]
public class UserData {
    public SerializableReactiveProperty<bool> CanUseStairs = new(false);      // 階段が使える状態かどうか
    public SerializableReactiveProperty<int> FlipPoint = new(0);              // めくれる回数
    public SerializableReactiveProperty<int> MemoryStoneCount = new(0);       // 思い出の秘石の獲得数
    public SerializableReactiveProperty<int> FloorCount = new(1);
    public SerializableReactiveProperty<int> MemoriaRank = new(0);

    public int expandInventoryCount;                                          // インベントリを拡張した回数

    public SerializableReactiveProperty<int> SoulPoint = new();
    public SerializableReactiveProperty<int> WalkCount = new();
    public int stageCount;
    public int waveNo;
    public int challengeCount;
    public List<int> clearedStageNoList = new();
    public int maxWaveNo;
    public int consumeSoulPoint;  // 消費したソウルポイントの合計。これと SoulPoint を加算すれば総獲得量になる

    public SerializableReactiveProperty<int> DefeatedEnemyCount = new(0);
    public SerializableReactiveProperty<int> FindTreasureCount = new(0);

    public SerializableReactiveProperty<int> ExploreCount = new(0);
    public SerializableReactiveProperty<int> UncurseCount = new(0);

    public SerializableReactiveProperty<int> Stamina = new();
    public List<int> equipItemList = new();

    public ObservableList<int> MemoryStoneSlotList = new();


    /// <summary>
    /// JsonConvert に利用するには、引数なしのコンストラクタが必要 
    /// </summary>
    public UserData() {
        SoulPoint.Value = 0;
        WalkCount.Value = 0;
        stageCount = 0;
        waveNo = 1;
        challengeCount = 0;
        clearedStageNoList.Clear();
        maxWaveNo = 10;
        ExploreCount.Value = 0;
        DefeatedEnemyCount.Value = 0;
        FindTreasureCount.Value = 0;
        UncurseCount.Value = 0;

        Stamina.Value = 0;
        equipItemList.Clear();
    }

    public UserData(int stamina) {
        SoulPoint.Value = 0;
        WalkCount.Value = 0;
        stageCount = 0;
        waveNo = 1;
        challengeCount = 0;
        clearedStageNoList.Clear();
        maxWaveNo = 10;
        ExploreCount.Value = 0;
        DefeatedEnemyCount.Value = 0;
        FindTreasureCount.Value = 0;
        UncurseCount.Value = 0;

        Stamina.Value = stamina;
        equipItemList.Clear();
    }
}