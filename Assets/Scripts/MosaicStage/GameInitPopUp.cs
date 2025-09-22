using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using R3;

public class GameInitPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnCancel;

    [SerializeField]
    private Button btnInit;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Text txtVolumeValue;


    /// <summary>
    /// 初期設定
    /// </summary>
    public void SetUp() {
        btnCancel.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => 
            {
                // 現在の Slider の Value 値を保持
                SoundManager.instance.SetMasterVolume(volumeSlider.value);
                ClosePopup();
            })
            .AddTo(gameObject);

        btnInit.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => InitGame())
            .AddTo(gameObject);

        volumeSlider.OnValueChangedAsObservable()
            .Subscribe(x =>
            {
                SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, x);
                UpdateVolumeValue(x); 
            })
            .AddTo(gameObject);

        // スライダーの Value に現在のボリューム設定
        volumeSlider.value = SoundManager.instance.masterVolume;

        canvasGroup.alpha = 0;

        // ポップアップ表示
        OpenPopup();
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
        sequence.Append(btnCancel.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnCancel.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
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
    /// ゲームの初期化
    /// </summary>
    private void InitGame() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnInit.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnInit.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject);

        //SoundManager.instance.PlaySE(SE_TYPE.Submit);

        // 指定したキーのセーブデータ削除
        //PlayerPrefsHelper.RemoveObjectData(UserData.instance.GetSaveDataKey());

        //// 情報リセット
        //UserData.instance.PrepareReset();

        // シーン再読み込み
        StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }

    /// <summary>
    /// ボリューム ％ 表示の更新
    /// </summary>
    /// <param name="value"></param>
    private void UpdateVolumeValue(float value) {
        txtVolumeValue.text = (value * 100).ToString("F0");
    }
}