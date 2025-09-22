using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using R3;

public class AchievementPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private Button btnAchieve;

    [SerializeField]
    private Button btnReset;


    // 各アチーブメントデータの表示用
    [SerializeField]
    private Text[] txtChallengeCounts;

    [SerializeField]
    private Text[] txtClearCounts;

    [SerializeField]
    private Text[] txtFailureCounts;

    [SerializeField]
    private Text[] txtMaxFeverCounts;

    [SerializeField]
    private Text[] txtNoMissClearCounts;

    [SerializeField]
    private Text[] txtMaxMosaicCounts;

    [SerializeField]
    private Text[] txtMaxLinkCounts;

    [SerializeField]
    private Text[] txtFastestClearTimes;

    // GameInitPopUp
    [SerializeField]
    private GameInitPopUp gameInitPopUpPrefab;

    private GameInitPopUp gameInitPopUp;


    /// <summary>
    /// 初期設定
    /// </summary>
    public void Setup() {
        // ボタンの購読
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        btnAchieve.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        btnReset.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => PrepareGameInitPopUp())
            .AddTo(gameObject);

        // ステージ毎のアチーブメントデータを表示
        SetAchievementStageDatas();

        canvasGroup.alpha = 0;

        // ポップアップ表示
        OpenPopup();
    }

    /// <summary>
    /// ステージ毎のアチーブメントデータを表示
    /// </summary>
    private void SetAchievementStageDatas() {
        //for (int i = 0; i < UserData.instance.achievementStageDataList.Count; i++) {
        //    txtChallengeCounts[i].text = UserData.instance.achievementStageDataList[i].challengeCount.ToString();
        //    txtClearCounts[i].text = UserData.instance.achievementStageDataList[i].clearCount.ToString();
        //    txtFailureCounts[i].text = UserData.instance.achievementStageDataList[i].failureCount.ToString();
        //    txtMaxFeverCounts[i].text = UserData.instance.achievementStageDataList[i].maxFeverCount.ToString();
        //    txtNoMissClearCounts[i].text = UserData.instance.achievementStageDataList[i].noMissClearCount.ToString();
        //    txtMaxMosaicCounts[i].text = UserData.instance.achievementStageDataList[i].maxMosaicCount.ToString();
        //    txtMaxLinkCounts[i].text = UserData.instance.achievementStageDataList[i].maxLinkCount.ToString();
        //    txtFastestClearTimes[i].text = UserData.instance.achievementStageDataList[i].fastestClearTime.ToString("F2");
        //}
    }

    /// <summary>
    /// ポップアップを表示する
    /// </summary>
    public void OpenPopup() {
        gameObject.SetActive(true);
        AnimePopup(1.0f);
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnClose.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnClose.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
            .OnComplete(() => AnimePopup(0f));

        //SoundManager.instance.PlaySE(SE_TYPE.Cancel);        
    }

    /// <summary>
    /// ポップアップをアニメさせる
    /// </summary>
    /// <param name="alpha"></param>
    private void AnimePopup(float alpha) {
        canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;

                if (alpha == 0) {
                    gameObject.SetActive(false);
                }
            }).SetLink(gameObject);
    }

    /// <summary>
    /// GameInitPopUp の生成とオープン
    /// </summary>
    private void PrepareGameInitPopUp() {
        // ポップアップが生成されていなければ
        if (!gameInitPopUp) {
            // 生成して初期設定してから開く
            gameInitPopUp = Instantiate(gameInitPopUpPrefab);
            gameInitPopUp.SetUp();
        } else {
            // ポップアップを開く
            gameInitPopUp.OpenPopup();
        }
        //SoundManager.instance.PlaySE(SE_TYPE.Submit);
    }
}