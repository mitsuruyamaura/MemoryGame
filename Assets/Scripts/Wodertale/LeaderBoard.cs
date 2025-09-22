using R3;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour { 
    [SerializeField] private CanvasGroup canvasGroup; 
    
    [SerializeField] private Text txtClearWaveNo;
    [SerializeField] private Text txtWalkCount;
    [SerializeField] private Text txtSoulPoint;

    [SerializeField] private Text txtDefeatedEnemies;
    [SerializeField] private Text txtFindTreasures;
    [SerializeField] private Text txtReleasePowerSpot;

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
    public void SetLeaderBoard(SaveData saveData) {
        canvasGroup.alpha = 1.0f;

        // 各種数値の設定
        txtClearWaveNo.text = saveData.userData.waveNo.ToString();
        txtWalkCount.text = saveData.userData.WalkCount.Value.ToString();
        txtSoulPoint.text = (saveData.userData.SoulPoint.Value + saveData.userData.consumeSoulPoint).ToString();

        txtDefeatedEnemies.text = saveData.userData.DefeatedEnemyCount.Value.ToString();
        txtFindTreasures.text = saveData.userData.FindTreasureCount.Value.ToString();
        txtReleasePowerSpot.text = saveData.userData.ExploreCount.Value.ToString();

        // アイテム一覧作成
        for (int i = 0; i < saveData.itemDataList.Count; i++) {
            BackPackInItem backPackInItem = Instantiate(backPackInItemPrefab, contentTran, false);
            backPackInItem.SetUpResult(saveData.itemDataList[i], saveData.enhanceLevelList[i]);
        }        
    }
}