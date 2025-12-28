using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 難易度の種類
/// </summary>
public enum DifficultyType {
    Tutorial,
    Normal,
    Hard
}

public class LeaderBoard : MonoBehaviour { 
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Text txtTotalSoulPoint;
    [SerializeField] private Text txtMemoriaRank;
    [SerializeField] private Text txtInventorySize;

    [SerializeField] private Text txtDefeatedEnemies;
    [SerializeField] private Text txtFindTreasures;
    [SerializeField] private Text txtBlessingCount;

    [SerializeField] private Text txtMemoriaCount;
    [SerializeField] private Text txtTrapDisarmCount;
    [SerializeField] private Text txtTrapFailureCount;

    [SerializeField] private Text txtSelectLevel;

    [SerializeField] private Transform contentTran;
    [SerializeField] private BackPackInItem backPackInItemPrefab;


    /// <summary>
    /// リーダーボードの非表示
    /// </summary>
    public void InactivateLeaderBoard() {
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// リーダーボードの設定
    /// </summary>
    /// <param name="saveData"></param>
    public void SetLeaderBoard(SaveDataDto saveData) {
        canvasGroup.alpha = 1.0f;

        // 各種数値の設定 
        txtTotalSoulPoint.text = (saveData.userData.soulPoint + saveData.userData.consumeSoulPoint).ToString();
        txtMemoriaRank.text = saveData.userData.memoriaRank.ToString();
        txtInventorySize.text = (saveData.userData.expandInventoryCount + ConstData.DEFAULT__INVENTORY_SIZE).ToString();

        txtDefeatedEnemies.text = saveData.userData.defeatedEnemyCount.ToString();
        txtFindTreasures.text = saveData.userData.findTreasureCount.ToString();
        txtBlessingCount.text = saveData.userData.blessingCount.ToString();
        txtMemoriaCount.text = saveData.userData.memoriaCount.ToString();
        txtTrapDisarmCount.text = saveData.userData.trapDisarmCount.ToString();
        txtTrapFailureCount.text = saveData.userData.trapFailureCount.ToString();

        txtSelectLevel.text = ((DifficultyType)saveData.userData.selectLevel).ToString();

        // アイテム一覧作成
        for (int i = 0; i < saveData.itemDataList.Count; i++) {
            BackPackInItem backPackInItem = Instantiate(backPackInItemPrefab, contentTran, false);
            backPackInItem.SetUpResult(saveData.itemDataList[i], saveData.enhanceLevelList[i]);
        }        
    }
}