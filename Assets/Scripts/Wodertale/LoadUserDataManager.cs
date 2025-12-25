using System.Collections.Generic;
using UnityEngine;

public class LoadUserDataManager : MonoBehaviour {
    [SerializeField] private SaveDataDto saveData;
    [SerializeField] private List<SaveDataDto> saveDataList = new();
    public List<SaveDataDto> SaveDataList => saveDataList;

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