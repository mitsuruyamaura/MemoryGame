using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommonPopup_ThreeBtn : PopupBase {
    [SerializeField] private Button btnOther_1;
    [SerializeField] private Button btnOther_2;

    [SerializeField] private GameObject ojbTitle;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private TMP_Text txtBtnOther_1;
    [SerializeField] private TMP_Text txtBtnOther_2;


    public void SetCommonPopup(string title, string body, CommonPopupButtonType buttonType, string[] otherButtonStrs, bool[] isCloses, UnityAction[] popupActions = null) {
        if (title != "") {
            ojbTitle.SetActive(true);
            txtTitle.text = title;
        } else {
            ojbTitle.SetActive(false);
        }
        txtMessage.text = body;

        // 閉じるボタンはセットせず、そのまま PopupBase の X ボタンを使う
        // ここでは閉じる以外の2つのボタンをセットする
        btnOther_1.gameObject.SetActive(true);
        btnOther_2.gameObject.SetActive(false);

        // 1つ目のボタン設定(Two の場合は、1つ目と2つ目の両方をセットする)
        if (buttonType == CommonPopupButtonType.One || buttonType == CommonPopupButtonType.Two) {
            txtBtnOther_1.text = otherButtonStrs[0];

            btnOther_1.OnClickExt(() => {
                popupActions[0]?.Invoke();

                if (isCloses[0]) {
                    ClosePopupProc();
                }
            }, this);
        }

        // 2つ目のボタン設定
        if (buttonType == CommonPopupButtonType.Two) {
            btnOther_2.gameObject.SetActive(true);

            txtBtnOther_2.text = otherButtonStrs[1];

            btnOther_2.OnClickExt(() => {
                popupActions[1]?.Invoke();

                if (isCloses[1]) {
                    ClosePopupProc();
                }
            }, this);
        }
        base.SetInitialize();
    }
}