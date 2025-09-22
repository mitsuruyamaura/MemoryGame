using System.Collections.Generic;
using UnityEngine;

public class LoadUserDataManager : MonoBehaviour {
    [SerializeField] private SaveData saveData;
    [SerializeField] private List<SaveData> saveDataList = new();
    public List<SaveData> SaveDataList => saveDataList;

    /// <summary>
    /// セーブデータのロード
    /// </summary>
    public void LoadSaveData() {
        //saveData = PlayerPrefsHelper.LoadGameData();
        saveDataList = PlayerPrefsHelper.LoadAllSortedSaveDataList();
    }

    /// <summary>
    /// クリアデータの全削除
    /// </summary>
    public void ResetSaveDatas() {
        saveDataList.Clear();
        PlayerPrefsHelper.ClearAllSaveData();
    }
}