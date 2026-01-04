using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
/// Text 表示の基底クラス
/// </summary>
public class TextViewBase : PoolBase {

#pragma warning disable 0649
    [SerializeField] protected TextMeshProUGUI txtView;
    [SerializeField] protected CanvasGroup canvasGroupTextView;
    //[SerializeField] protected Canvas canvasView;
#pragma warning restore 0649

    /// <summary>
    /// Text 表示更新
    /// </summary>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    public virtual async UniTask UpdateTextAsync(string newMessage) {
        ShowText();
        txtView.text = newMessage;

        await UniTask.Yield();
    }

    // public virtual void UpdateCounterText(float oldValue, float newValue) {
    //     txtView.DOCounter(oldValue, newValue, duration).SetEase(Ease.InQuart);
    // }

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="newMessage"></param>
    public virtual void SetUpView(string newMessage) {
        UpdateTextAsync(newMessage).Forget();

        //if (canvasView.worldCamera == null) {
        //    canvasView.worldCamera = Camera.main;
        //}
    }

    /// <summary>
    /// Text 表示
    /// </summary>
    public virtual void ShowText() {
        canvasGroupTextView.alpha = 1.0f;
        //canvasView.enabled = true;
    }

    /// <summary>
    /// Text 非表示
    /// </summary>
    public virtual void HideText() {
        //canvasGroupTextView.alpha = 0f;
        //canvasView.enabled = false;
    }
}