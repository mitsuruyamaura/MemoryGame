using System.Collections.Generic;

/// <summary>
/// UserData のセーブ用クラス
/// RectiveProperty などの形式ではセーブできないので、これを使う
/// </summary>
[System.Serializable]
public class UserDataSave {
    public int flipPoint;
    public int memoryStoneCount;
    public int floorCount;
    public int memoriaRank;
    public int soulPoint;

    public int expandInventoryCount;
    public int consumeSoulPoint;

    public int defeatedEnemyCount;
    public int findTreasureCount;
    public int blessingCount;
    public int memoriaCount;
    public int trapDisarmCount;
    public int trapFailureCount;

    public List<int> equipItemList;
    public List<int> memoryStoneSlotList;

    public int selectLevel;
}