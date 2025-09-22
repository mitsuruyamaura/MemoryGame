using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン名の管理用(文字列入力による誤字防止)
/// </summary>
public enum SCENE_STATE {
    Title,
    Menu,
    Stage,
    Start
}

/// <summary>
/// 各シーンの遷移、およびシーン開始時と終了時のトランジション用クラス。シングルトン
/// </summary>
public class TransitionManager : MonoBehaviour {

    [Header("フェイドイン／フェイドアウト制御用")]
    public Fade fade;
    [Header("マスク用イメージ制御用")]
    public GameObject maskImage;

    [Header("フェイドインまでの待機時間")]
    public float fadeInTime = 1.0f;
    [Header("現在のシーン名")]
    public SCENE_STATE sceneState;

    //[Header("バトル終了確認ポップアップ")]
    //public ExitPopUp exitPopupPrefab;
    [Header("ポップアップボタン")]
    public Button openBtn;
    [Header("ポップアップボタンイメージ")]
    public Image openBtnImage;

    private bool isSet;

    // シングルトン
    public static TransitionManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        // 現在のシーン名を取得
        sceneState = (SCENE_STATE)Enum.Parse(typeof(SCENE_STATE), SceneManager.GetActiveScene().name);
        // フェイドイン処理
        TransFadeIn(fadeInTime);
        // 終了確認ボタン登録
        //openBtn.onClick.AddListener(OnClickOpenExitPopup);
    }

    /// <summary>
    /// 次のシーンが開始したときの処理(イベント)
    /// </summary>
    /// <param name="nextScene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene nextScene, LoadSceneMode mode) {
        // 現在のシーン名を取得する
        sceneState = (SCENE_STATE)Enum.Parse(typeof(SCENE_STATE), SceneManager.GetActiveScene().name);
        // フェイドイン処理
        TransFadeIn(fadeInTime);
    }

    /// <summary>
    /// 各シーン開始時のフェイドイン処理
    /// </summary>
    public void TransFadeIn(float time) {
        StartCoroutine(ActiveReturnBtn(time));
        fade.FadeIn(time, () => {
            if (!isSet) {
                isSet = true;
                SetUp();
            }
            fade.FadeOut(time);
        });
    }

    /// <summary>
    /// リターンボタンを表示する
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ActiveReturnBtn(float time) {
        yield return new WaitForSeconds(time + 0.2f);
        //openBtnImage.enabled = true;
        //openBtn.interactable = true;
    }
    /// <summary>
    /// マスク画像を非表示にする
    /// </summary>
    private void SetUp() {
        maskImage.SetActive(false);
    }

    /// <summary>
    /// 各シーン終了時のフェイドアウト処理
    /// </summary>
    /// <param name="time"></param>
	public void TransFadeOut(float time) {
        fade.FadeIn(1.0f, () => {
            fade.FadeOut(time);
        });
    }


    public void PrepareNextScene(SCENE_STATE nextSceneName) {
        StartCoroutine(MoveNextScene(nextSceneName));
    }

    /// <summary>
    /// フェイドアウトしながらシーン遷移する
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveNextScene(SCENE_STATE nextSceneName) {
        // フェイドアウト処理
        TransFadeOut(fadeInTime);
        //openBtnImage.enabled = false;
        yield return new WaitForSeconds(fadeInTime);

        // イベントハンドラーに次のシーンを登録し、シーン遷移後にイベント処理を行うようにする
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene(nextSceneName.ToString());
    }

    /// <summary>
    /// ゲーム終了確認用ポップアップを開く
    /// </summary>
    public void OnClickOpenExitPopup() {
        // バトル準備中には開かない
        //if (sceneState == SCENE_STATE.Stage) {
        //    if (!GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>().CheckState()) {
        //        return;
        //    }
        //}
        //openBtn.interactable = false;

        // ExitPopup生成
        //ExitPopUp exitPop = Instantiate(exitPopupPrefab, Camera.main.transform, false);
        //exitPop.Setup();

        //if (sceneState == SCENE_STATE.Stage) {
        //    GameMaster gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        //    exitPop.gameMaster = gameMaster;
        //    // 現在のゲームの状態を保存し、バトルを一時停止
        //    exitPop.currentSceneState = gameMaster.gameState;
        //    exitPop.gameMaster.PauseBattle();
        //}
    }

    //private void Update() {
    //if ((Input.GetKeyDown(KeyCode.Escape)) && (openBtn.interactable)) {
    //    // 端末のリターンボタンでも終了できるようにしておく
    //    OnClickOpenExitPopup();
    //}
    //}
}