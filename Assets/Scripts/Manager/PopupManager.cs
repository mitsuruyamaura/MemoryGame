using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class PopupManager : AbstractSingleton<PopupManager> {
    [SerializeField] private List<PopupBase> popupList = new();  // PopupBase を継承しているポップアップクラスのリスト
    private PopupBase currentViewPopup;   // 現在開いているポップアップ
    public PopupBase CurrentViewPopup => currentViewPopup;

    private Stack<PopupBase> history = new(); // 以前開いていたポップアップを保持するためのスタック


    void Start() {
        // デバッグ用
        SetUp();
    }

    /// <summary>
    /// 初期設定。外部クラスから1回だけ実行予定
    /// </summary>
    public void SetUp() {
        // Stack をクリア
        history.Clear();

        currentViewPopup = null;

        // TODO 他に必要な処理があれば追加する

    }

    /// <summary>
    /// 指定された型の最初に登録された PopupBase を継承しているクラスを検索して取得
    /// </summary>
    /// <typeparam name="T">検索対象の PopupBase クラス</typeparam>
    /// <returns>検索結果となる、指定された型の PopupBase インスタンス。見つからない場合は null </returns>
    public T GetPopupPrefab<T>() where T : PopupBase {
        return popupList.OfType<T>().SingleOrDefault();
    }

    /// <summary>
    /// 指定された型の PopupBase を継承したクラスを検索して表示する
    /// </summary>
    /// <param name="keepInHistory">現在開いている Popup を履歴スタックに追加するかどうか。true で追加</param>
    /// <typeparam name="T">検索対象の PopupBase クラス</typeparam>
    public PopupBase Show<T>(object param = null, UnityAction popupAction = null, bool keepInHistory = false) where T : PopupBase {
        // 生成したいポップアップのプレハブを探す
        var targetPopup = GetPopupPrefab<T>();

        // 指定された型のポップアップが見つからない場合
        if (targetPopup == null) {
            DebugLogger.Log($"指定された型のポップアップが見つかりません。");
            return null;
        }

        if (currentViewPopup == null) {
            // currentViewPopup が破棄されている場合(Missing 回避用)
            //DebugLogger.Log("ポップアップは破棄されています。");
        } else if (currentViewPopup is T) {
            // 現在のポップアップが既に指定された型 T の場合
            DebugLogger.Log($"{targetPopup} はすでに開いています。");
            return null;
        }

        // ポップアップを生成して開く
        return CreatePopup(param, targetPopup, popupAction, keepInHistory);
    }

    /// <summary>
    /// Popup を表示し、他の Popup を非表示にする
    /// </summary>
    /// <param name="popupPrefab">表示する Popup</param>
    /// <param name="keepInHistory">現在開いている Popup を履歴スタックに追加するかどうか。true で追加</param>
    private PopupBase CreatePopup(object param, PopupBase popupPrefab, UnityAction popupAction, bool keepInHistory) {
        // 現在開いているポップアップが存在している場合
        if (currentViewPopup != null) {

            // 現在開いているポップアップを保持しない場合
            if (!keepInHistory) {                
                // 現在開いているポップアップを閉じる
                currentViewPopup.ClosePopupProc();
            }
        }

        // ポップアップ生成と初期設定
        var popup = Instantiate(popupPrefab, transform, false);
        popup.SetInitialize(param, popupAction).Forget();

        // 新しく開いたポップアップをカレントとして保持しない場合(下にあるポップアップを残しておく場合)
        if (!keepInHistory) {
            // 現在開いているポップアップを更新
            currentViewPopup = popup;
        }

        return popup;
    }

    public void ShowCommonPopup(string title, string message, CommonPopupButtonType commonButtonType, string closeButtonStr, string otherButtonStr, UnityAction popupAction = null, bool isCloseFilter = false){ 
        // 生成したいポップアップのプレハブを探す
        var targetPopup = GetPopupPrefab<CommonPopup>();

        // 指定された型のポップアップが見つからない場合
        if (targetPopup == null) {
            DebugLogger.Log($"指定された型のポップアップが見つかりません。");
            return;
        }

        if (currentViewPopup == null) {
            // currentViewPopup が破棄されている場合(Missing 回避用)
            //DebugLogger.Log("ポップアップは破棄されています。");
        } else if (currentViewPopup is CommonPopup) {
            // 現在のポップアップが既に指定された型 T の場合
            DebugLogger.Log($"{targetPopup} はすでに開いています。");
            return;
        }

        // ポップアップ生成と初期設定
        var popup = Instantiate(targetPopup, transform, false);
        popup.SetCommonPopup(title, message, commonButtonType, closeButtonStr, otherButtonStr, popupAction, isCloseFilter);
    }

    /// <summary>
    /// 右上に X ボタン、画面下部に1つ、あるいは2つのボタンを配置するタイプ
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="commonButtonType"></param>
    /// <param name="otherButtonStrs"></param>
    /// <param name="isCloses"></param>
    /// <param name="popupActions"></param>
    public void ShowCommonPopupThreeBtn(string title, string message, CommonPopupButtonType commonButtonType, string[] otherButtonStrs, bool[] isCloses, UnityAction[] popupActions = null) {
        // 生成したいポップアップのプレハブを探す
        var targetPopup = GetPopupPrefab<CommonPopup_ThreeBtn>();

        // 指定された型のポップアップが見つからない場合
        if (targetPopup == null) {
            DebugLogger.Log($"指定された型のポップアップが見つかりません。");
            return;
        }

        if (currentViewPopup == null) {
            // currentViewPopup が破棄されている場合(Missing 回避用)
            //DebugLogger.Log("ポップアップは破棄されています。");
        } else if (currentViewPopup is CommonPopup) {
            // 現在のポップアップが既に指定された型 T の場合
            DebugLogger.Log($"{targetPopup} はすでに開いています。");
            return;
        }

        // ポップアップ生成と初期設定
        var popup = Instantiate(targetPopup, transform, false);
        popup.SetCommonPopup(title, message, commonButtonType, otherButtonStrs, isCloses, popupActions);
    }

    public void AllClosePopup() {
        // 現在開いているポップアップを閉じる
        currentViewPopup?.ClosePopupProc();
        //currentViewPopup = null;

        // 非アクティブのポップアップも含めて探して閉じる
        PopupBase[] popups = transform.GetComponentsInChildren<PopupBase>(true);
        for (int i = 0; i < popups.Length; i++) {
            popups[i].ClosePopupProc();
        }

        // Stack をクリア
        history?.Clear();
    }


    public void ResetCurrentPopup() {
        currentViewPopup = null;
    }

    public void SetCurrentPopup(PopupBase popupBase) {
        currentViewPopup = popupBase;
    }

    /// <summary>
    /// 子オブジェクトの中から指定された PopupBase を継承したコンポーネントを探して返す(非アクティブも対象)
    /// </summary>
    /// <typeparam name="T">検索対象の PopupBase 派生クラス</typeparam>
    /// <returns>見つかったインスタンス、なければ null</returns>
    public T FindPopupFromChildren<T>() where T : PopupBase {
        return transform.GetComponentsInChildren<T>(includeInactive: true).FirstOrDefault();
    }
}