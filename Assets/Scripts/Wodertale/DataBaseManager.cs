using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBaseManager : AbstractSingleton<DataBaseManager> {
    public CardTypeSO cardTypeSO;
    public EnemyDataSO enemyDataSO;
    public ItemDataSO itemDataSO;
    //public NameDataSO nameDataSO;
    public MemoryStoneDataSO memoryStoneDataSO;
    public FloorDataSO floorDataSO;
    public TrapDataSO trapDataSO;
    public BlessingDataSO blessingDataSO;
    public ConstantDataSO constantDataSO;
    public ConditionDataSO conditionDataSO;
    public TrapActionDataSO trapActionDataSO;


    public EntryData entryData;           // 選択した難易度の保持用。Title シーン再読み込み時にリセットされる

    //public List<AbilityItemDataSO> abilityItemDataSOList;

    //// ドロップするトレジャーをすべて入れる
    //public List<AbilityItemDataSO.AbilityItemData> dropItemDatasList = new List<AbilityItemDataSO.AbilityItemData>();


    /// <summary>
    /// CardTypeMaster を ID や Enum から取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public CardTypeMaster GetCardType(CardEventType type) => cardTypeSO.cardTypeList.FirstOrDefault(c => c.cardEventType == type);


    /// <summary>
    /// 現在のフロア数から FloorData を取得
    /// </summary>
    /// <param name="currentFloorCount"></param>
    /// <returns></returns>
    public FloorData GetFloorDataByFloor(int currentFloorCount) {
        return floorDataSO.floorDataList.LastOrDefault(data => data.minFloorCount <= currentFloorCount);
    }

    public EnemyData GetRandomEnemyByRarity(Rarity[] enemyRarities, int[] enemyRates) {
        if (enemyRarities == null || enemyRates == null ||
            enemyRarities.Length != enemyRates.Length || enemyRarities.Length == 0) {
            DebugLogger.Log("敵レアリティ抽選の入力が不正です。");
            return null;
        }

        // 累積和によるレアリティ抽選
        int totalWeight = enemyRates.Sum();
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        Rarity chosenRarity = enemyRarities[0]; // デフォルト
        for (int i = 0; i < enemyRates.Length; i++) {
            cumulative += enemyRates[i];
            if (roll < cumulative) {
                chosenRarity = enemyRarities[i];
                break;
            }
        }

        // 2. 選ばれたレアリティから敵を取得
        List<EnemyData> rarityEnemyList = GetEnemyDataListFromRarity(chosenRarity);

        if (rarityEnemyList == null || rarityEnemyList.Count == 0) {
            DebugLogger.Log($"指定レアリティ {chosenRarity} の敵が存在しません。");
            return null;
        }

        // ランダムで1つ選択
        int index = UnityEngine.Random.Range(0, rarityEnemyList.Count);
        return rarityEnemyList[index];
    }

    public ItemData GetRandomItemByEnemyDrop(Rarity enemyRarity) {
        List<ItemData> rarityItemDataList = GetItemDataListByRarity(enemyRarity);

        if (rarityItemDataList == null || rarityItemDataList.Count == 0) {
            DebugLogger.Log($"指定レアリティ {enemyRarity} のアイテムが存在しません。");
            return null;
        }

        // ランダムで1つ選択
        int index = UnityEngine.Random.Range(0, rarityItemDataList.Count);
        return rarityItemDataList[index];
    }

    public ItemData GetRandomItemByChest(Rarity[] rarities, int[] rates) {
        if (rarities == null || rates == null ||
            rarities.Length != rates.Length || rarities.Length == 0) {
            DebugLogger.Log("アイテムレアリティ抽選の入力が不正です。");
            return null;
        }

        // 累積和によるレアリティ抽選
        int totalWeight = rates.Sum();
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        Rarity chosenRarity = rarities[0]; // デフォルト
        for (int i = 0; i < rates.Length; i++) {
            cumulative += rates[i];
            if (roll < cumulative) {
                chosenRarity = rarities[i];
                break;
            }
        }

        // 2. 選ばれたレアリティからアイテムを取得
        List<ItemData> rarityItemDataList = GetItemDataListByRarity(chosenRarity);

        if (rarityItemDataList == null || rarityItemDataList.Count == 0) {
            DebugLogger.Log($"指定レアリティ {chosenRarity} のアイテムが存在しません。");
            return null;
        }

        // ランダムで1つ選択
        int index = UnityEngine.Random.Range(0, rarityItemDataList.Count);
        return rarityItemDataList[index];
    }

    /// <summary>
    /// 抽選されたレアリティ内からランダムなトラップを取得
    /// </summary>
    /// <param name="trapRarities"></param>
    /// <param name="trapRates"></param>
    /// <returns></returns>
    public TrapData GetRandomTrapByRarity(Rarity[] trapRarities, int[] trapRates) {
        if (trapRarities == null || trapRates == null ||
            trapRarities.Length != trapRates.Length || trapRarities.Length == 0) {
            DebugLogger.Log("アイテムレアリティ抽選の入力が不正です。");
            return null;
        }

        // 累積和によるレアリティ抽選
        int totalWeight = trapRates.Sum();
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        Rarity chosenRarity = trapRarities[0]; // デフォルト
        for (int i = 0; i < trapRates.Length; i++) {
            cumulative += trapRates[i];
            if (roll < cumulative) {
                chosenRarity = trapRarities[i];
                break;
            }
        }

        // 2. 選ばれたレアリティからトラップを取得
        List<TrapData> raritytrapDataList = GetTrapDataListByRarity(chosenRarity);

        if (raritytrapDataList == null || raritytrapDataList.Count == 0) {
            DebugLogger.Log($"指定レアリティ {chosenRarity} のトラップが存在しません。");
            return null;
        }

        // ランダムで1つ選択
        int index = UnityEngine.Random.Range(0, raritytrapDataList.Count);
        return raritytrapDataList[index];
    }

    /// <summary>
    /// Tier を利用し、フロアごとのランダムなトラップを取得
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public TrapData GetRandomTrapByTier(FloorData floorData) {
        // フロアの Tier からトラップの候補 List を取得
        List<TrapData> candidateList = GetTrapDataListByTier(floorData);

        if (candidateList == null || candidateList.Count == 0) {
            DebugLogger.Log($"指定 Tier のトラップが存在しません。");
            return null;
        }

        float totalWeight = 0f;
        Dictionary<TrapData, float> weightTable = new ();

        // Tier に補正を反映してトラップの重み付け計算
        foreach (TrapData trap in candidateList) {
            float tierRatio = 0.0f;

            // Tier Ratio の計算
            if (floorData.maxTrapTier == floorData.minTrapTier) {
                tierRatio = 1f;
            } else {
                // 0.0 〜 1.0 の範囲で正規化
                tierRatio = (trap.tier - floorData.minTrapTier) / (float)(floorData.maxTrapTier - floorData.minTrapTier);
            }

            // 念のため、Clamp して範囲内に収める
            tierRatio = Mathf.Clamp01(tierRatio);

            // Tier Range に応じた AnimataionCurve 補正の強さを計算(tierRange が高くなるほど、カーブをそのまま使う)
            int tierRange = floorData.maxTrapTier - floorData.minTrapTier;
            float curveStrength = Mathf.Clamp01((tierRange - 1) / 3.0f);
            //DebugLogger.Log($"TrapID:{trap.id} TierRange:{tierRange} CurveStrength:{curveStrength}");

            // Tier Ratio を元にした補正値を計算
            float tierBonus = Mathf.Lerp(1f, GameSettings.instance.trapBalanceSettings.tierWeightCurve.Evaluate(tierRatio), curveStrength);
            //DebugLogger.Log($"TrapID:{trap.id} Tier:{trap.tier} TierRatio:{tierRatio} TierBonus:{tierBonus}");

            // 最終的な重みの値を算出
            float effectiveWeight = trap.weight * tierBonus;

            // 重みが 0 より大きいものだけを合計する
            if (effectiveWeight > 0f) {
                weightTable.Add(trap, effectiveWeight);
                totalWeight += effectiveWeight;
            }
        }

        if (totalWeight <= 0f) {
            return null;
        }

        // 重み付きランダム抽選
        float rand = Random.Range(0f, totalWeight);

        foreach (var pair in weightTable) {
            rand -= pair.Value;
            if (rand <= 0f) {
                return pair.Key;
            }
        }

        // 保険(通常はここに来ない)
        return null;
    }

    /// <summary>
    /// レアリティの重み付けを利用したイベントの抽選
    /// </summary>
    /// <param name="blessingRarities"></param>
    /// <param name="blessingRates"></param>
    /// <returns></returns>
    public BlessingData GetRandomBlessingByRarity(Rarity[] blessingRarities, int[] blessingRates) {
        if (blessingRarities == null || blessingRates == null ||
            blessingRarities.Length != blessingRates.Length || blessingRarities.Length == 0) {
            DebugLogger.Log("イベントレアリティ抽選の入力が不正です。");
            return null;
        }

        // 累積和によるレアリティ抽選
        int totalWeight = blessingRates.Sum();
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        Rarity chosenRarity = blessingRarities[0]; // デフォルト
        for (int i = 0; i < blessingRates.Length; i++) {
            cumulative += blessingRates[i];
            if (roll < cumulative) {
                chosenRarity = blessingRarities[i];
                break;
            }
        }

        // 2. 選ばれたレアリティからイベントを取得
        List<BlessingData> rarityBlessingDataList = GetBlessingDataListByRarity(chosenRarity);

        if (rarityBlessingDataList == null || rarityBlessingDataList.Count == 0) {
            DebugLogger.Log($"指定レアリティ {chosenRarity} のイベントが存在しません。");
            return null;
        }

        // ランダムで1つ選択
        int index = UnityEngine.Random.Range(0, rarityBlessingDataList.Count);
        return rarityBlessingDataList[index];
    }

    /// <summary>
    /// 思い出の断片の抽選
    /// </summary>
    /// <returns></returns>
    public MemoryStoneData GetRandomMemoryStoneByWeight() {
        int totalWeight = memoryStoneDataSO.memoryStoneList.Sum(data => data.weight);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        for (int i = 0; i < memoryStoneDataSO.memoryStoneList.Count; i++) {
            cumulative += memoryStoneDataSO.memoryStoneList[i].weight;
            if (roll < cumulative) {
                return memoryStoneDataSO.memoryStoneList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 次のレベルアップに必要な経験値を計算して取得
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    //public int CalcNextLevelExp(int level) {
    //    return expTableSO.expTablesList[level].maxExp;
    //}

    // mi

    ///// <summary>
    ///// レベルと AbilityType による AbilityPointTable の取得
    ///// </summary>
    ///// <returns></returns>
    //public AbilityItemDataSO.AbilityItemData GetAbilityPointTable(int level, AbilityType abilityType) {
    //    return abilityItemDataSOList[(int)abilityType].abilityItemDatasList.Find(x => x.abilityLevel == level);
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dropTreasureLevel"></param>
    //public void CreateDropItemDatasList(int dropTreasureLevel) {

    //    // TreasureTable 内の DropItemDatas を取得
    //    DropItemData[] dropItemDatas = treasureTableSO.treasureTablesList.Find(x => x.treasureLevel == dropTreasureLevel).dropItemDatas;

    //    //Debug.Log(dropItemDatas.Length);

    //    // すべてのアイテムデータのリストを検索
    //    for (int i = 0; i < abilityItemDataSOList.Count; i++) {

    //        // テーブルに含まれるレアリティを取得
    //        int[] rarityArray = dropItemDatas[i].rarities.Split(',').ToArray().Select(x => int.Parse(x)).ToArray();
    //        //Debug.Log(rarityArray.Length);

    //        foreach (AbilityItemDataSO.AbilityItemData itemData in abilityItemDataSOList[i].abilityItemDatasList) {
    //            for (int x = 0; x < rarityArray.Length; x++) {

    //                if (itemData.rarity == rarityArray[x]) {
    //                    dropItemDatasList.Add(itemData);
    //                    continue;
    //                }
    //            }
    //        }
    //    }
    //}


    public List<BlessingData> GetBlessingDataListByRarity(Rarity rarity) {
        // 実装済のイベントのみ対象
        return blessingDataSO.blessingDataList.Where(data => data.implemented == 1 && data.rarity == rarity).ToList();
    }

    /// <summary>
    /// 未使用
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public List<TrapData> GetTrapDataListByRarity(Rarity rarity) {
        // 実装済のトラップのみ対象
        return trapDataSO.trapDataList.Where(data => data.implemented == 1 && data.rarity == rarity).ToList();
    }

    /// <summary>
    /// 現在のフロアデータから出現可能なトラップの List を取得
    /// </summary>
    /// <param name="floorData"></param>
    /// <returns></returns>
    public List<TrapData> GetTrapDataListByTier(FloorData floorData) {
        var candidateList = trapDataSO.trapDataList.Where(trap => trap.implemented == 1 && trap.tier >= floorData.minTrapTier && trap.tier <= floorData.maxTrapTier);
        return candidateList.ToList();
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

    public Sprite GetBlessingImage(int blessingId) {
        return Resources.Load<Sprite>("Blessing/" + blessingId);
    }


    public Sprite GetEnemyIcon(int enemyId) {
        return Resources.Load<Sprite>("Enemy/" + enemyId);
    }

    public Sprite GetConditionIcon(int conditionId) {
        return Resources.Load<Sprite>("Condition/" + conditionId);
    }

    public Sprite GetTrapIcon(int trapId) {
        return Resources.Load<Sprite>("Trap/" + trapId);
    }


    public EnemyData GetEnemyData(int searchEnemyId) {
        return enemyDataSO.enemyDatasList.FirstOrDefault(data => data.enemyNo == searchEnemyId);
    }

    public ConditionData GetConditionData(ConditionType searchConditionType) {
        return conditionDataSO.conditionDatasList.FirstOrDefault(data => data.conditionType == searchConditionType);
    }

    public List<TrapActionData> GetTrapActionDataList(int searchTrapId) {
        return trapActionDataSO.trapActionDataList
                   .Where(data => data.trapId == searchTrapId)
                   .OrderBy(data => data.order)
                   .ToList();
    }

    /// <summary>
    /// Rarity で抽出した敵の List を戻す
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public List<EnemyData> GetEnemyDataListFromRarity(Rarity rarity) {
        return enemyDataSO.enemyDatasList.Where(data => data.rarity == rarity).ToList();
    }


    //public List<PowerSpotData> GetPowerSpotDataListFromRarity(Rarity rarity) {
    //    return powerSpotDataSO.powerSpotDataList.Where(data => data.rarity == rarity).ToList();
    //}


    //public List<PowerSpotData> GetPowerSpotDataListFromWaveCount(int targetWaveCount) {
    //    return powerSpotDataSO.powerSpotDataList.Where(data => data.waveCount == targetWaveCount).ToList();
    //}

    //public PowerSpotData GetPowerSpotData(int searchId) {
    //    return powerSpotDataSO.powerSpotDataList.FirstOrDefault(data => data.no == searchId);
    //}

    //public NameData GetRandomNameData() {
    //    int index = UnityEngine.Random.Range(0, nameDataSO.nameDataList.Count);
    //    return nameDataSO.nameDataList[index];
    //}

    /// <summary>
    /// ConstantData の key から value を取得
    /// キャストは呼び出し元で行う
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetConstantDataValue(string key) {
        return constantDataSO.constantDataList.FirstOrDefault(data => data.key == key)?.value;
    }

    /// <summary>
    /// 選択した難易度のレベルを設定
    /// </summary>
    /// <param name="selectLevel"></param>
    public void SetSelectLevel(int selectLevel) {
        entryData.selectLevel = selectLevel;
    }

    //public WaveData GetWaveData(int searchWaveId) {
    //    return waveDataSO.waveDataList.FirstOrDefault(data => data.waveNo == searchWaveId); 
    //}


    //public int GetWaveNo(int walkCount, int waveNo) {

    //    // LINQ の場合
    //    //return waveDataSO.waveDataList
    //    //.Where(item => item.waveNo > waveNo && item.walkCount >= walkCount)
    //    //.Select(item => item.waveNo)
    //    //.DefaultIfEmpty(waveNo)
    //    //.First();

    //    foreach (var item in waveDataSO.waveDataList) {
    //        if (item.waveNo > waveNo && item.walkCount <= walkCount) {
    //            return item.waveNo;
    //        }
    //    }
    //    return waveNo;
    //}
}