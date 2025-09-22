using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataBaseManager : AbstractSingleton<DataBaseManager> {

    public EnemyDataSO enemyDataSO;
    public ConditionDataSO conditionDataSO;
    public ExpTableSO expTableSO;
    public StageDataSO stageDataSO;
    public ItemDataSO itemDataSO;
    public NameDataSO nameDataSO;
    public ExpTableDataSO expTableDataSO;
    public WaveDataSO waveDataSO;
    public SymbolRateDataSO symbolRateDataSO;
    public PowerSpotDataSO powerSpotDataSO;
    public TerrainDataSO terrainDataSO;

    // mi
    public EnemyMoveEventDataSO enemyMoveEventDataSO;
    public OrbDataSO orbDataSO;
    public TreasureTableSO treasureTableSO;


    public List<AbilityItemDataSO> abilityItemDataSOList;

    // ドロップするトレジャーをすべて入れる
    public List<AbilityItemDataSO.AbilityItemData> dropItemDatasList = new List<AbilityItemDataSO.AbilityItemData>();


    protected override void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 次のレベルアップに必要な経験値を計算して取得
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int CalcNextLevelExp(int level) {
        return expTableSO.expTablesList[level].maxExp;
    }

    // mi

    /// <summary>
    /// レベルと AbilityType による AbilityPointTable の取得
    /// </summary>
    /// <returns></returns>
    public AbilityItemDataSO.AbilityItemData GetAbilityPointTable(int level, AbilityType abilityType) {
        return abilityItemDataSOList[(int)abilityType].abilityItemDatasList.Find(x => x.abilityLevel == level);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dropTreasureLevel"></param>
    public void CreateDropItemDatasList(int dropTreasureLevel) {

        // TreasureTable 内の DropItemDatas を取得
        DropItemData[] dropItemDatas = treasureTableSO.treasureTablesList.Find(x => x.treasureLevel == dropTreasureLevel).dropItemDatas;

        //Debug.Log(dropItemDatas.Length);

        // すべてのアイテムデータのリストを検索
        for (int i = 0; i < abilityItemDataSOList.Count; i++) {

            // テーブルに含まれるレアリティを取得
            int[] rarityArray = dropItemDatas[i].rarities.Split(',').ToArray().Select(x => int.Parse(x)).ToArray();
            //Debug.Log(rarityArray.Length);

            foreach (AbilityItemDataSO.AbilityItemData itemData in abilityItemDataSOList[i].abilityItemDatasList) {
                for (int x = 0; x < rarityArray.Length; x++) {

                    if (itemData.rarity == rarityArray[x]) {
                        dropItemDatasList.Add(itemData);
                        continue;
                    }
                }
            }
        }
    }


    public ItemData GetItemData(int searchId) {
        return itemDataSO.itemDataList.FirstOrDefault(data => data.id == searchId);
    }


    public List<ItemData> GetItemDataListByRarity(Rarity rarity) {
        return itemDataSO.itemDataList.Where(data => data.rarity == rarity).ToList();
    }


    public Sprite GetItemIcon(int searchItemId) {
        return Resources.Load<Sprite>("Item/" + searchItemId);
    }


    public Sprite GetEnemyIcon(int enemyId) {
        return Resources.Load<Sprite>("Enemy/" + enemyId);
    }


    public EnemyData GetEnemyData(int searchEnemyId) {
        return enemyDataSO.enemyDatasList.FirstOrDefault(data => data.enemyNo == searchEnemyId);
    }

    /// <summary>
    /// Rarity で抽出した敵の List を戻す
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public List<EnemyData> GetEnemyDataListFromRarity(Rarity rarity) {
        return enemyDataSO.enemyDatasList.Where(data => data.rarity == rarity).ToList();
    }


    public List<PowerSpotData> GetPowerSpotDataListFromRarity(Rarity rarity) {
        return powerSpotDataSO.powerSpotDataList.Where(data => data.rarity == rarity).ToList();
    }


    public List<PowerSpotData> GetPowerSpotDataListFromWaveCount(int targetWaveCount) {
        return powerSpotDataSO.powerSpotDataList.Where(data => data.waveCount == targetWaveCount).ToList();
    }

    public PowerSpotData GetPowerSpotData(int searchId) {
        return powerSpotDataSO.powerSpotDataList.FirstOrDefault(data => data.no == searchId);
    }

    public NameData GetNameData(int searchNameId) {
        return nameDataSO.nameDataList.FirstOrDefault(data => data.id == searchNameId);
    }


    public WaveData GetWaveData(int searchWaveId) {
        return waveDataSO.waveDataList.FirstOrDefault(data => data.waveNo == searchWaveId); 
    }


    public int GetWaveNo(int walkCount, int waveNo) {

        // LINQ の場合
        //return waveDataSO.waveDataList
        //.Where(item => item.waveNo > waveNo && item.walkCount >= walkCount)
        //.Select(item => item.waveNo)
        //.DefaultIfEmpty(waveNo)
        //.First();

        foreach (var item in waveDataSO.waveDataList) {
            if (item.waveNo > waveNo && item.walkCount <= walkCount) {
                return item.waveNo;
            }
        }
        return waveNo;
    }

    public TerrainData GetTerrainData(string searchTerrainName) {
        return terrainDataSO.terrainDataList.FirstOrDefault(data => data.name == searchTerrainName);
    }
}