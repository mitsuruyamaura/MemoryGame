using System;
using System.Linq;

[System.Serializable]
public class FloorData : IMasterData {
    public int id;
    public string desc;
    public int minFloorCount;
    public Rarity[] enemyRarities;   // 敵と祝福と宝箱の Rarity と Rate。敵を倒した場合には敵の Rarity を使うので、敵の方が Rarity を狙いやすい
    public int[] enemyRates;
    public Rarity[] trapRarities;
    public int[] trapRates;
    public int row;
    public int pairCount;
    public int treasureChest;
    public int blessing;
    public int enemy;
    public int trap;
    public int memoryStone;
    public int key;
    public int random;
    public int treasureChestWeight;
    public int blessingWeight;
    public int enemyWeight;
    public int trapWeight;
    public int memoryStoneWeight;
    public Rarity[] blessingRarities;
    public int[] blessingRate;

    public int Id => id;

    public FloorData(string[] datas) {
        id = int.Parse(datas[0]);
        desc = datas[1];
        minFloorCount = int.Parse(datas[2]);

        // 半角スラッシュで区切って配列に変換
        enemyRarities = datas[3].Split('/').Select(type => (Rarity)Enum.Parse(typeof(Rarity), type)).ToArray();
        enemyRates = datas[4].Split('/').Select(int.Parse).ToArray();

        trapRarities = datas[5].Split('/').Select(type => (Rarity)Enum.Parse(typeof(Rarity), type)).ToArray();
        trapRates = datas[6].Split('/').Select(int.Parse).ToArray();

        row = int.Parse(datas[7]);
        pairCount = int.Parse(datas[8]);

        treasureChest = int.Parse(datas[9]);
        blessing = int.Parse(datas[10]);
        enemy = int.Parse(datas[11]);
        trap = int.Parse(datas[12]);
        memoryStone = int.Parse(datas[13]);

        key = int.Parse(datas[14]);
        random = int.Parse(datas[15]);

        treasureChestWeight = int.Parse(datas[16]);
        blessingWeight = int.Parse(datas[17]);
        enemyWeight = int.Parse(datas[18]);
        trapWeight = int.Parse(datas[19]);
        memoryStoneWeight = int.Parse(datas[20]);

        blessingRarities = datas[21].Split('/').Select(type => (Rarity)Enum.Parse(typeof(Rarity), type)).ToArray();
        blessingRate = datas[22].Split('/').Select(int.Parse).ToArray();
    }
}
