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

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text txtVolumeValue;
    [SerializeField] private Text txtLoading;

    [SerializeField] private GSSReceiver GSSReceiver;
    [SerializeField] private LoadUserDataManager loadUserDataManager;
    [SerializeField] private LeadingBoardPopup leadingBoardPopup;


    void Start() {
        btnLeaderBoard.interactable = false;
        btnResetDatas.interactable = false;

        InitGameData().Forget();

        // TODO 一旦なし
        InitLeaderBoard();
    }

    private async UniTaskVoid InitGameData() {
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

        btnStart.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);

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
        txtLoading.gameObject.SetActive(false);

        btnStart.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(true);
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