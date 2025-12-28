using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// セーブ用データ保持クラス
/// このままだと UserData が保存できないので、SaveDataDto に変換してセーブする
/// 今回はゲーム内では利用せず(復元して継続するデータはない)、リーダーボードでしか使わないのでロード時は使わない
/// </summary>
[System.Serializable]
public class SaveData {
    public string saveId;
    public UserData userData;
    public List<ItemData> itemDataList = new();
    public List<int> enhanceLevelList = new();

    public SaveData() { }

    public SaveData(string saveId, UserData userData, List<ItemData> itemDataList, List<int> enhanceLevelList) {
        this.saveId = saveId;
        this.userData = userData;
        this.itemDataList = new(itemDataList);
        this.enhanceLevelList = new(enhanceLevelList);
    }
}

/// <summary>
/// UserData 内の RectiveProperty などが保存できないので、UserDataSave クラスとしてプリミティブ型でセーブするためのクラス
/// NewtonSoft の JsonConvert クラスを使わないで、JsonUtility クラスで対応する
/// </summary>
[Serializable]
public class SaveDataDto {
    public string saveId;
    public UserDataSave userData;
    public List<ItemData> itemDataList;
    public List<int> enhanceLevelList;

    public SaveDataDto() { }

    public SaveDataDto(string saveId, UserDataSave userData, List<ItemData> itemDataList, List<int> enhanceLevelList) {
        this.saveId = saveId;
        this.userData = userData;
        this.itemDataList = itemDataList;
        this.enhanceLevelList = enhanceLevelList;
    }
}

/// <summary>
/// 指定したクラスをstring型のJson形式でPlayerPrefsクラスにセーブ・ロードするためのHelperクラス
/// </summary>
public static class PlayerPrefsHelper {

    /// <summary>
    /// 指定したキーのデータが存在しているか確認
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistsData(string key) {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// すべてのセーブデータを削除
    /// </summary>
    public static void ClearAllSaveData() {
        PlayerPrefs.DeleteAll();
        DebugLogger.Log("すべてのデータを削除しました");
    }

    /// <summary> /// 指定したデータを削除 /// 複数スロット用のデータ削除メソッド /// </summary>
    public static void DeleteSaveData(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= ConstData.MAX_SAVE_SLOTS) {
            DebugLogger.Log("スロットインデックスが範囲外です");
            return;
        }

        string key = $"{ConstData.GAME_DATA_SAVE_KEY}{slotIndex}";
        PlayerPrefs.DeleteKey(key);
        DebugLogger.Log($"スロット{slotIndex}のデータを削除しました");
    }

    /// <summary>
    /// 条件付きセーブ
    /// </summary>
    public static void ConditionalSave(SaveData gameData) {
        // SaveData を SaveDataDTO に変換する
        SaveDataDto newData = ToDto(gameData);

        // セーブ
        ConditionalSaveInternal(newData);
    }

    /// <summary>
    /// セーブ実処理
    /// </summary>
    /// <param name="newData"></param>
    public static void ConditionalSaveInternal(SaveDataDto newData) {
        List<SaveDataDto> allDataList = new();

        // すべてのセーブデータをロードして List に追加
        for (int i = 0; i < ConstData.MAX_SAVE_SLOTS; i++) {
            SaveDataDto data = LoadGameData(i);
            if (data != null) {
                allDataList.Add(data);
            }
        }

        // 新しいセーブデータと同じデータがセーブされている場合には、除外してから追加
        allDataList.RemoveAll(loadDate => loadDate.saveId == newData.saveId);
        allDataList.Add(newData);

        // 降順(ソウルポイントの高いデータ順、メモリアランクの高い順)に並び替え
        allDataList = allDataList.OrderByDescending(d => d.userData.soulPoint)
                    .ThenByDescending(d => d.userData.memoriaRank)
                    .ToList();

        // 降順に並んでいる List 内のデータを3つまで取得
        if (allDataList.Count > ConstData.MAX_SAVE_SLOTS) {
            allDataList = allDataList.Take(ConstData.MAX_SAVE_SLOTS).ToList();
        }

        for (int i = 0; i < ConstData.MAX_SAVE_SLOTS; i++) {
            if (i < allDataList.Count) {
                SaveGameData(allDataList[i], i);
            } else {
                DeleteSaveData(i); // 空スロット化
            }
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 複数スロット用のセーブメソッド
    /// </summary>
    public static void SaveGameData(SaveDataDto saveData, int slotIndex) {
        if (slotIndex < 0 || slotIndex >= ConstData.MAX_SAVE_SLOTS) {
            Debug.LogError("スロットインデックスが範囲外です");
            return;
        }

        string key = $"{ConstData.GAME_DATA_SAVE_KEY}{slotIndex}";
        string jsonString = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(key, jsonString);
        DebugLogger.Log($"スロット{slotIndex}にデータをセーブしました");
    }

    /// <summary>
    /// 複数スロット用のロードメソッド
    /// </summary>
    public static SaveDataDto LoadGameData(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= ConstData.MAX_SAVE_SLOTS) {
            DebugLogger.Log("スロットインデックスが範囲外です");
            return null;
        }

        string key = $"{ConstData.GAME_DATA_SAVE_KEY}{slotIndex}";
        if (!ExistsData(key)) {
            DebugLogger.Log($"スロット{slotIndex}にセーブデータがありません");
            return null;
        }

        string jsonString = PlayerPrefs.GetString(key);
        SaveDataDto loadData = JsonUtility.FromJson<SaveDataDto>(jsonString);
        DebugLogger.Log($"スロット{slotIndex}からデータをロードしました");

        return loadData;
    }

    /// <summary>
    /// セーブデータをすべてロードし、指定条件で並び替えて返す
    /// </summary>
    public static List<SaveDataDto> LoadAllSortedSaveDataList() {
        List<SaveDataDto> saveDataList = new();

        // 各スロットからデータをロード
        for (int i = 0; i < ConstData.MAX_SAVE_SLOTS; i++) {
            SaveDataDto data = LoadGameData(i);
            if (data != null) {
                saveDataList.Add(data);
            }
        }

        // データが存在しない場合はそのままリストを返す
        if (saveDataList.Count == 0) {
            return saveDataList;
        }

        return saveDataList;
    }

    /// <summary>
    /// SaveData → SaveDataDto 変換
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static SaveDataDto ToDto(SaveData data) {
        // 新規作成時のみ生成
        if (string.IsNullOrEmpty(data.saveId)) {
            data.saveId = Guid.NewGuid().ToString();
        }

        return new SaveDataDto {
            saveId = data.saveId,
            userData = ToSave(data.userData),
            itemDataList = new List<ItemData>(data.itemDataList),
            enhanceLevelList = new List<int>(data.enhanceLevelList)
        };
    }

    /// <summary>
    /// UserData → UserDataSave 変換
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static UserDataSave ToSave(UserData data) {
        return new UserDataSave {
            flipPoint = data.FlipPoint.Value,
            memoryStoneCount = data.MemoryStoneCount.Value,
            floorCount = data.FloorCount.Value,
            memoriaRank = data.MemoriaRank.Value,
            soulPoint = data.SoulPoint.Value,

            expandInventoryCount = data.expandInventoryCount,
            consumeSoulPoint = data.consumeSoulPoint,

            defeatedEnemyCount = data.DefeatedEnemyCount.Value,
            findTreasureCount = data.FindTreasureCount.Value,
            blessingCount = data.BlessingCount.Value,
            memoriaCount = data.MemoriaCount.Value,
            trapDisarmCount = data.TrapDisarmCount.Value,
            trapFailureCount = data.TrapFailureCount.Value,

            equipItemList = new List<int>(data.equipItemList),
            memoryStoneSlotList = new List<int>(data.MemoryStoneSlotList),

            selectLevel = data.selectLevel
        };
    }

    /// <summary>
    /// SaveDataDto → SaveData 変換にして戻す
    /// 今回は使わない(リーダーボードに表示するだけなので、SaveData に戻す必要がない)
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public static SaveData LoadGameDataAsGameData(int slotIndex) {
        SaveDataDto dto = LoadGameData(slotIndex);
        if (dto == null) return null;

        return FromDto(dto);
    }

    /// <summary>
    /// SaveDataDto → SaveData 変換
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static SaveData FromDto(SaveDataDto dto) {
        return new SaveData {
            saveId = dto.saveId,
            userData = FromSave(dto.userData),
            itemDataList = new List<ItemData>(dto.itemDataList),
            enhanceLevelList = new List<int>(dto.enhanceLevelList)
        };
    }

    /// <summary>
    /// UserDataSave → UserData 変換
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    public static UserData FromSave(UserDataSave save) {
        var data = new UserData();
        data.FlipPoint.Value = save.flipPoint;
        data.MemoryStoneCount.Value = save.memoryStoneCount;
        data.FloorCount.Value = save.floorCount;
        data.MemoriaRank.Value = save.memoriaRank;
        data.SoulPoint.Value = save.soulPoint;

        data.expandInventoryCount = save.expandInventoryCount;
        data.consumeSoulPoint = save.consumeSoulPoint;

        data.DefeatedEnemyCount.Value = save.defeatedEnemyCount;
        data.FindTreasureCount.Value = save.findTreasureCount;
        data.BlessingCount.Value = save.blessingCount;
        data.MemoriaCount.Value = save.memoriaCount;
        data.TrapDisarmCount.Value = save.trapDisarmCount;
        data.TrapFailureCount.Value = save.trapFailureCount;

        data.equipItemList = new List<int>(save.equipItemList);
        data.MemoryStoneSlotList = new ObservableList<int>(save.memoryStoneSlotList);

        data.selectLevel = save.selectLevel;

        return data;
    }
}