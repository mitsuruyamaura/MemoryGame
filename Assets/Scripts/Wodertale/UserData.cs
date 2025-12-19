using System.Collections.Generic;
using ObservableCollections;
using R3;

[System.Serializable]
public class UserData {
    public SerializableReactiveProperty<bool> CanUseStairs = new(false);      // 階段が使える状態かどうか
    public SerializableReactiveProperty<int> FlipPoint = new(0);              // めくれる回数
    public SerializableReactiveProperty<int> MemoryStoneCount = new(0);       // 思い出の秘石の獲得数
    public SerializableReactiveProperty<int> FloorCount = new(1);             // 現在の階層
    public SerializableReactiveProperty<int> MemoriaRank = new(0);            // メモリアランク
    public SerializableReactiveProperty<int> SoulPoint = new();

    public int expandInventoryCount;                                          // インベントリを拡張した回数
    public int consumeSoulPoint;                                              // 消費したソウルポイントの合計。これと SoulPoint を加算すれば総獲得量になる

    public SerializableReactiveProperty<int> DefeatedEnemyCount = new(0);     // 倒した敵の数
    public SerializableReactiveProperty<int> FindTreasureCount = new(0);      // 見つけた宝物の数
    public SerializableReactiveProperty<int> BlessingCount = new(0);          // 祝福を受けた回数
    public SerializableReactiveProperty<int> MemoriaCount = new(0);           // メモリアを解放した数
    public SerializableReactiveProperty<int> TrapDisarmCount = new(0);        // 解除した罠の数
    public SerializableReactiveProperty<int> TrapFailureCount = new(0);       // 罠にかかった数

    public List<int> equipItemList = new();                    // 装備しているアイテムのリスト(アイテムNo)。初期装備としてランダムに2つ装備する

    public ObservableList<int> MemoryStoneSlotList = new();    // 思い出の秘石を獲得した際にセットするためのリスト。3つでリセット


    /// <summary>
    /// JsonConvert に利用するには、引数なしのコンストラクタが必要 
    /// </summary>
    public UserData() {
        SoulPoint.Value = 0;
        FlipPoint.Value = 0;
        MemoriaRank.Value = 0;
        MemoryStoneCount.Value = 0;

        expandInventoryCount = 0;
        consumeSoulPoint = 0;

        DefeatedEnemyCount.Value = 0;
        FindTreasureCount.Value = 0;
        BlessingCount.Value = 0;
        MemoriaCount.Value = 0;
        TrapDisarmCount.Value = 0;
        TrapFailureCount.Value = 0;

        equipItemList.Clear();
    }

    public UserData(int flipCount) {
        SoulPoint.Value = 0;
        FlipPoint.Value = flipCount;
        MemoriaRank.Value = 0;
        MemoryStoneCount.Value = 0;

        expandInventoryCount = 0;
        consumeSoulPoint = 0;

        DefeatedEnemyCount.Value = 0;
        FindTreasureCount.Value = 0;
        BlessingCount.Value = 0;
        MemoriaCount.Value = 0;
        TrapDisarmCount.Value = 0;
        TrapFailureCount.Value = 0;

        equipItemList.Clear();
    }
}