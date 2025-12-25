using DG.Tweening;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class LeadingBoardPopup : MonoBehaviour {
    [SerializeField] private LoadUserDataManager loadUserDataManager;

    [SerializeField] private Button btnReturnTitle;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private List<LeaderBoard> leaderBoardList = new();
    private float duration = 0.5f;

    public void SetUpPopup () {
        ClosePopup(0);
        leaderBoardList.ForEach(board => board.InactivateLeaderBoard());

        // LoadUserDataManager の SaveData を見て、必要な数だけ LeaderBoard の中身を作る
        for (int i = 0; i < loadUserDataManager.SaveDataList.Count; i++) {
            leaderBoardList[i].SetLeaderBoard(loadUserDataManager.SaveDataList[i]);
        }

        btnReturnTitle.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
            .Subscribe(_ => ClosePopup(duration))
            .AddTo(this);
    }

    public void OpenPopup() {
        // SaveData を削除している場合
        if (loadUserDataManager.SaveDataList.Count == 0) {
            leaderBoardList.ForEach(board => board.InactivateLeaderBoard());
        }
        canvasGroup.DOFade(1.0f, duration).SetEase(Ease.InQuart);
        canvasGroup.blocksRaycasts = true;
    }

    public void ClosePopup(float duration) {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, duration).SetEase(Ease.OutQuart);
    }
}