using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClassInfoView : InfoViewBase {
    [SerializeField] protected Button btnInfoView;

    public void ShowClassInfoView(IMasterData masterData, UnityAction<IMasterData> btnAction) {
        isReleased = false;


        // 表示内容設定


        // ボタンにコールバック登録
        disposable = btnInfoView?.OnClickExt(() => btnAction.Invoke(masterData), this);
    }
}