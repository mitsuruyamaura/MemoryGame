using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum CommonPopupButtonType {
    One,
    Two
}

public class CommonPopup : PopupBase {
    [SerializeField] private Button btnOther;

    [SerializeField] private GameObject ojbTitle;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private TMP_Text txtBtnClose;
    [SerializeField] private TMP_Text txtBtnOther;

    public void SetCommonPopup(string title, string body, CommonPopupButtonType buttonType, string txtBtnCloseName, string txtBtnOtherName, UnityAction buttonCallback, bool isCloseFilter) {
        if (title != "") {
            ojbTitle.SetActive(true);
            txtTitle.text = title;
        } else {
            ojbTitle.SetActive(false);
        }
        txtMessage.text = body;

        if (buttonType == CommonPopupButtonType.One) {
            btnOther.gameObject.SetActive(false);
            txtBtnClose.text = txtBtnCloseName;
        } else {
            btnOther.gameObject.SetActive(true);

            txtBtnClose.text = txtBtnCloseName;
            txtBtnOther.text = txtBtnOtherName;

            btnOther.OnClickExt(() => {
                buttonCallback?.Invoke();
                ClosePopupProc();
            }, this);
        }

        this.isCloseFilter = isCloseFilter;

        base.SetInitialize();
    }

    /// <summary>
    /// 左上に×ボタンがあり、下部中央にボタンが1つあるタイプ
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="txtBtnOtherName"></param>
    /// <param name="buttonCallback"></param>
    public void SetCommonPopup(string title, string body, string txtBtnOtherName, UnityAction buttonCallback = null) {
        if (title != "") {
            ojbTitle.SetActive(true);
            txtTitle.text = title;
        } else {
            ojbTitle.SetActive(false);
        }
        txtMessage.text = body;
        btnClose.gameObject.SetActive(true);

        txtBtnOther.text = txtBtnOtherName;

        btnOther.OnClickExt(() => {
            buttonCallback?.Invoke();
            ClosePopupProc();
        }, this);

        base.SetInitialize();
    }
}