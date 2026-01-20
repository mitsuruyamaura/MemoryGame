using UnityEngine;
using R3;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class Title : MonoBehaviour {
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnLeaderBoard;
    [SerializeField] private Button btnResetDatas;
    [SerializeField] private Image imgShadeReset;

    [SerializeField] private CanvasGroup cgVolume;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text txtVolumeValue;

    [SerializeField] private CanvasGroup cgLoading;
    [SerializeField] private Text txtLoading;

    [SerializeField] private GSSReceiver GSSReceiver;
    [SerializeField] private LoadUserDataManager loadUserDataManager;
    [SerializeField] private LeadingBoardPopup leadingBoardPopup;

    [SerializeField] private Button btnLevelSelect;
    [SerializeField] private DifficultySelectToggle[] difficultySelectToggles;
    [SerializeField] private CanvasGroup cgDifficultySelect;
    [SerializeField] private ToggleGroup difficultyToggleGroup;

    private CompositeDisposable disposables;

    private bool hasSelectedOnce = false;


    void Start() {
        InitGameData().Forget();

        InitLeaderBoard();

        HideLevelSelectToggles();
    }

    private async UniTaskVoid InitGameData() {
        cgLoading.alpha = 1.0f;
        cgVolume.alpha = 0;

        btnLeaderBoard.interactable = false;
        btnResetDatas.interactable = false;
        btnStart.interactable = false;

        // 先にスライダーの購読を設定しておく(スライダーの Value を設定してしまうとボリュームが初期値とロード値の2回分動いてしまうため)
        volumeSlider.OnValueChangedAsObservable()
            .Subscribe(x => {
                SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, x);
                UpdateVolumeValue(x);
            }).AddTo(gameObject);

        // セーブされている BGMボリューム値があるか確認
        float bgmVolume = PlayerPrefs.GetFloat(ConstData.BGM_VOLUME, SoundManager.instance.masterVolume);

        // スライダーの Value に現在のボリューム設定してBGM再生
        volumeSlider.value = bgmVolume;
        SoundManager.instance.PlayBGM(BGM_TYPE.Title);

        Tweener tweener = txtLoading.DOFade(0, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);

        var token = this.GetCancellationTokenOnDestroy();

        // SO のデータを取得するまで待機
        //await UniTask.WaitUntil(() => GSSReceiver.IsLoading, cancellationToken: token);

        await GSSReceiver.PrepareGSSLoadStartAsync();

        //// シーンの非同期読み込み
        //var sceneLoadTask = UniTask.Create(async () => {
        //    var asyncOperation = SceneManager.LoadSceneAsync("Stage");
        //    asyncOperation.allowSceneActivation = false;

        //    // シーンの読み込みが完了するまで待機
        //    while (!asyncOperation.isDone) {
        //        if (asyncOperation.progress >= 0.9f && !asyncOperation.allowSceneActivation) {
        //            asyncOperation.allowSceneActivation = true;
        //        }
        //        await UniTask.Yield(); // フレーム待機
        //    }
        //    Debug.Log("Stage ロード完了");
        //}).AttachExternalCancellation(token);

        btnStart.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
            .Subscribe(_ => {
                // 現在の BGM のボリューム値をセーブしてからシーン遷移
                PlayerPrefs.SetFloat(ConstData.BGM_VOLUME, volumeSlider.value);
                SceneStateManager.instance.PrepareteNextScene(SceneName.Stage);
            }).AddTo(this);

        tweener.Kill();
        tweener = null;

        cgLoading.DOFade(0, 0.5f).SetEase(Ease.Linear).SetLink(gameObject)
            .OnComplete(() => {
                cgLoading.blocksRaycasts = false;
                cgVolume.alpha = 1.0F;
            });

        // 難易度選択関連
        disposables = new();

        // ゲームスタートの前に押す難易度選択ボタンの購読
        btnLevelSelect.OnClickExt(() => ShowLevelSelectToggles(), this).AddTo(this);

        // 難易度選択用のトグル設定
        for (int i = 0; i < difficultySelectToggles.Length; i++) {
            int index = i;

            difficultySelectToggles[index].Setup(index);
            Toggle toggle = difficultySelectToggles[index].Toggle;

            toggle.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe(isOn => {
                    if (isOn) {
                        // 選択したゲームレベル取得して最終フロア設定
                        int selectedLevel = difficultySelectToggles[index].Level;
                        DataBaseManager.instance.SetSelectLevel(selectedLevel);

                        // 文字色の演出
                        difficultySelectToggles[index].Choose();
                        Debug.Log($"選択レベル: {selectedLevel}");

                        // ゲームスタートボタンの活性化(目線誘導)
                        ActivateGameStartBtn();

                        // 初回選択時のみトグル未選択状態になっているので、2回目以降は、未選択状態を許可しないようにする
                        if (!hasSelectedOnce) {
                            hasSelectedOnce = true;
                            difficultyToggleGroup.allowSwitchOff = false;
                        }
                    } else {
                        // 文字色の演出を戻す
                        difficultySelectToggles[index].Unchoose();
                    }
                })
                .AddTo(disposables);
        }
    }

    /// <summary>
    /// ゲームスタートボタンの活性化
    /// </summary>
    private void ActivateGameStartBtn() {
        btnStart.interactable = true;
        btnStart.transform.DOShakeScale(0.25f, 0.2f).SetEase(Ease.InQuart).SetLink(gameObject);
    }

    /// <summary>
    /// 難易度選択用のトグル群表示
    /// </summary>
    private void ShowLevelSelectToggles() {
        btnLevelSelect.gameObject.SetActive(false);

        cgDifficultySelect.alpha = 1.0f;
        cgDifficultySelect.blocksRaycasts = true;
    }

    /// <summary>
    /// 難易度選択用のトグル群非表示
    /// </summary>
    private void HideLevelSelectToggles() {
        cgDifficultySelect.alpha = 0f;
        cgDifficultySelect.blocksRaycasts = false;
    }

    /// <summary>
    /// ボリューム ％ 表示の更新
    /// </summary>
    /// <param name="value"></param>
    private void UpdateVolumeValue(float value) {
        txtVolumeValue.text = (value * 100).ToString("F0");
    }

    /// <summary>
    /// LeaderBoard の設定
    /// </summary>
    private void InitLeaderBoard() {
        // セーブデータのロード
        loadUserDataManager.LoadSaveData();

        // LeaderBoard の設定
        leadingBoardPopup.SetUpPopup();

        btnLeaderBoard.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
            .Subscribe(_ => leadingBoardPopup.OpenPopup()).AddTo(this);

        // LeaderBoard の準備が終ってから活性化
        btnLeaderBoard.interactable = true;

        // セーブデータがある場合のみ、リセットボタンを有効化
        if (loadUserDataManager.SaveDataList.Count > 0) {
            btnResetDatas.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
                .Subscribe(_ => {
                    btnResetDatas.interactable = false;
                    imgShadeReset.enabled = true;
                    loadUserDataManager.ResetSaveDatas();
                }).AddTo(this);

            btnResetDatas.interactable = true;
            imgShadeReset.enabled = false;
        }
    }
}