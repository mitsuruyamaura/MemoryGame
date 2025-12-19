using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using R3;

[System.Serializable]
public class SaveData {
    public UserData userData;
    public List<ItemData> itemDataList = new();
    public List<int> enhanceLevelList = new();

    public SaveData() { }

    public SaveData(UserData userData, List<ItemData> itemDataList, List<int> enhanceLevelList) { 
        this.userData = userData;
        this.itemDataList = new(itemDataList);
        this.enhanceLevelList = new(enhanceLevelList);
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
    /// 指定されたオブジェクトのデータをセーブ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public static void Save<T>(string key, T obj) {
        // オブジェクトのデータをJson形式に変換
        string json = JsonUtility.ToJson(obj);

        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();

        DebugLogger.Log($"{key}をセーブしました");
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをロード
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Load<T>(string key) {
        DebugLogger.Log($"{key}をロードします");

        string json = PlayerPrefs.GetString(key);

        // 読み込む型を指定し、変換して取得
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// すべてのセーブデータを削除
    /// </summary>
    public static void ClearAllSaveData() {
        PlayerPrefs.DeleteAll();
        DebugLogger.Log("すべてのデータを削除しました");
    }

    /// <summary>
    /// クラスをセーブ
    /// </summary>
    public static void SaveGameData(SaveData saveData) {
        SaveData saveUserData = saveData;

        //クラスをJSONにシリアライズ
        string jsonString = JsonConvert.SerializeObject(saveUserData);

        // JSON文字列を保存
        PlayerPrefs.SetString(ConstData.GAME_DATA_SAVE_KEY, jsonString);
        PlayerPrefs.Save();

        DebugLogger.Log($"{saveUserData}をセーブしました");
    }

    /// <summary>
    /// クラスをロード
    /// </summary>
    public static SaveData LoadGameData() {
        // JSON文字列をロード
        string jsonString = PlayerPrefs.GetString(ConstData.GAME_DATA_SAVE_KEY);

        // JSON文字列をGameDataクラスにデシリアライズ
        SaveData loadUserData = new();
        loadUserData = JsonConvert.DeserializeObject<SaveData>(jsonString);
        DebugLogger.Log($"{loadUserData}をロードしました");

        return loadUserData;       
    }

    /// <summary>
    /// 条件付きセーブ
    /// </summary>
    public static void ConditionalSave(SaveData newData) {
        // 1. 空きスロットを探す
        int emptySlot = -1;
        for (int i = 0; i < ConstData.MAX_SAVE_SLOTS; i++) {
            SaveData existingData = LoadGameData(i);

            if (existingData == null) {
                // 最初に見つけた空きスロットを記録して終了
                emptySlot = i;
                break;
            }
        }

        // 空きスロットがあれば、そこに保存して終了
        if (emptySlot != -1) {
            SaveGameData(newData, emptySlot);
            DebugLogger.Log($"空きスロット{emptySlot}に新規データを保存しました");
            return;
        }

        // 2. 上書き対象のスロットを探す(順位が低いものを下のスロットから順に比較)
        int overwriteIndex = -1;
        for (int i = ConstData.MAX_SAVE_SLOTS - 1; i >= 0; i--) {
            SaveData existingData = LoadGameData(i);

            // 条件比較(上書き条件)：SoulPoint が低い、または SoulPoint が同じで MemoriaRank が低い
            if (newData.userData.SoulPoint.Value < existingData.userData.SoulPoint.Value ||
                (newData.userData.SoulPoint.Value == existingData.userData.SoulPoint.Value &&
                 newData.userData.MemoriaRank.Value < existingData.userData.MemoriaRank.Value)) {

                // 上書き対象を記録
                overwriteIndex = i;
            }
        }

        // 上書き対象が見つからなければ終了
        if (overwriteIndex == -1) {
            DebugLogger.Log("条件を満たさず、セーブを更新しませんでした");
            return;
        }

        // 3. データのシフト処理(下にずらす。常に3番目のスロットが削除対象になる)
        // 上書き対象のスロットより後ろのデータを1つ下にずらす
        for (int i = ConstData.MAX_SAVE_SLOTS - 1; i > overwriteIndex; i--) {
            SaveData tempData = LoadGameData(i - 1); // 1つ前のデータを取得
            SaveGameData(tempData, i); // 1つ後ろにずらして保存
        }

        // 4. 上書き対象スロットに新しいデータを保存
        SaveGameData(newData, overwriteIndex);
        DebugLogger.Log($"スロット{overwriteIndex}のデータを上書きし、他のデータをシフトしました");
    }

    /// <summary>
    /// 複数スロット用のセーブメソッド
    /// </summary>
    public static void SaveGameData(SaveData saveData, int slotIndex) {
        if (slotIndex < 0 || slotIndex >= ConstData.MAX_SAVE_SLOTS) {
            Debug.LogError("スロットインデックスが範囲外です");
            return;
        }

        string key = $"{ConstData.GAME_DATA_SAVE_KEY}{slotIndex}";
        string jsonString = JsonConvert.SerializeObject(saveData);

        PlayerPrefs.SetString(key, jsonString);
        PlayerPrefs.Save();

        DebugLogger.Log($"スロット{slotIndex}にデータをセーブしました");
    }

    /// <summary>
    /// 複数スロット用のロードメソッド
    /// </summary>
    public static SaveData LoadGameData(int slotIndex) {
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
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(jsonString);
        DebugLogger.Log($"スロット{slotIndex}からデータをロードしました");

        return loadData;
    }

    /// <summary>
    /// セーブデータをすべてロードし、指定条件で並び替えて返す
    /// </summary>
    public static List<SaveData> LoadAllSortedSaveDataList() {
        List<SaveData> saveDataList = new();

        // 各スロットからデータをロード
        for (int i = 0; i < ConstData.MAX_SAVE_SLOTS; i++) {
            SaveData data = LoadGameData(i);
            if (data != null) {
                saveDataList.Add(data);
            }
        }

        // データが存在しない場合はそのままリストを返す
        if (saveDataList.Count == 0) {
            return saveDataList;
        }

        // 並び替え：SoulPoint → MemoriaRank
        saveDataList.Sort((a, b) => {
            // まず SoulPoint で比較
            int waveComparison = a.userData.SoulPoint.Value.CompareTo(b.userData.SoulPoint.Value);
            if (waveComparison != 0) {
                return waveComparison; // SoulPoint が低い方が上位
            }

            // SoulPoint が同じ場合は MemoriaRank で比較
            return a.userData.MemoriaRank.Value.CompareTo(b.userData.MemoriaRank.Value); // MemoriaRank が低い方が上位
        });

        return saveDataList;
    }

    /// <summary>
    /// 指定したデータを削除
    /// 複数スロット用のデータ削除メソッド
    /// </summary>
    public static void ClearSaveData(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= ConstData.MAX_SAVE_SLOTS) {
            DebugLogger.Log("スロットインデックスが範囲外です");
            return;
        }

        string key = $"{ConstData.GAME_DATA_SAVE_KEY}{slotIndex}";
        PlayerPrefs.DeleteKey(key);
        DebugLogger.Log($"スロット{slotIndex}のデータを削除しました");
    }
}