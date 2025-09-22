using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class World : MonoBehaviour
{
    [SerializeField]
    private Transform playerTran;

    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private StageSelectDetail stageSelectDetailPrefab;

    [SerializeField]
    private Transform stageSelectDetailTran;

    [SerializeField]
    private List<StageSelectDetail> stageSelectDetailsList = new List<StageSelectDetail>();

    [SerializeField]
    private RectTransform scrollViewRect;


    IEnumerator Start() {
        yield return null;

        float height = scrollViewRect.sizeDelta.y;

        scrollViewRect.sizeDelta = new Vector3(scrollViewRect.sizeDelta.x, 0);

        scrollViewRect.DOSizeDelta(new Vector3(scrollViewRect.sizeDelta.x, height), 1.5f).SetEase(Ease.OutBack);

        // スタート用ボタンの設定
        btnSubmit.onClick.AddListener(OnClickSubmit);
        btnSubmit.interactable = false;

        // ステージ選択用ボタンの生成と設定
        for (int i = 0; i < DataBaseManager.instance.stageDataSO.stageDatasList.Count; i++) {
            StageSelectDetail stageSelectDetail = Instantiate(stageSelectDetailPrefab, stageSelectDetailTran, false);
            stageSelectDetail.SetUpStageSelectDetail(DataBaseManager.instance.stageDataSO.stageDatasList[i], this);
            stageSelectDetailsList.Add(stageSelectDetail);

            // TODO 最初のステージ以外は、クリアしているステージのみ表示する
            if (i > 0 && !GameData.instance.userData.clearedStageNoList.Contains(i)) {
                stageSelectDetail.SwitchActivateButton(false);
            }
        }
    }

    /// <summary>
    /// プレイヤーのアイコンを配置
    /// </summary>
    /// <param name="newTran"></param>
    public void SetPlayerTran(Transform newTran) {
        playerTran.localPosition = newTran.position;

        btnSubmit.interactable = true;
    }

    /// <summary>
    /// ゲーム開始用のボタンの処理
    /// </summary>
    public void OnClickSubmit() {
        
        // 選択しているステージの番号から StageData を取得
        GameData.instance.currentStageData = DataBaseManager.instance.stageDataSO.stageDatasList.Find(x => x.stageNo == GameData.instance.chooseStageNo);

        // ボタンアニメ演出
        btnSubmit.transform.DOShakeScale(0.35f, 0.5f, 5)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => 
            {
                // シーン遷移の準備
                SceneStateManager.instance.PrepareteNextScene(SceneName.Stage);
            } );
    }
}
