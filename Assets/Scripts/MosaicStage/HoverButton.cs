using UnityEngine;
using UnityEngine.EventSystems;  // IPointerEnterHandler に必要
using UnityEngine.UI;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button btnHover;
    private bool isSelected;
    private float defaultScale;

    [SerializeField] private float sizeUpScale = 1.0f;
    [SerializeField] private float duration = 0.1f;


    void Start() {
        TryGetComponent(out btnHover);
        defaultScale = transform.localScale.x;
        // UI は反応しないので、OnPointerEnter を使う
        //this.OnMouseEnterAsObservable()
        //    .Subscribe(_ => ResponseButton())
        //    .AddTo(gameObject);
    }

    // UI ではコライダーをアタッチしても動作しない
    //private void OnMouseEnter() {
    //    Debug.Log("Enter");
    //}

    // 代わりにこちらを使う
    public void OnPointerEnter(PointerEventData eventData) {
        //if (btnHover == null) {
        //    return;
        //}
        //ResponseHoverButton();

        if (!btnHover.interactable) {
            return;
        }

        HoverCursor();
    }

    /// <summary>
    /// マウスをホバーしたときの処理
    /// </summary>
    //private void ResponseHoverButton() {
    //    if (!btnHover.enabled || isSelected) {
    //        return;
    //    }
    //    isSelected = true;
        
    //    transform.DOShakeScale(0.25f, 0.5f, 2)
    //        .SetEase(Ease.InQuart)
    //        .SetLink(gameObject)
    //        .OnComplete(() =>
    //        {
    //            transform.localScale = Vector3.one * defaultScale;
    //            isSelected = false;            
    //        });
    //}

    public void HoverCursor() {
        transform.DOScale(Vector3.one * sizeUpScale, duration)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
        //Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData) {
        ExitCursor();
    }

    public void ExitCursor() {
        transform.DOScale(Vector3.one * defaultScale, duration)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
        //Debug.Log("Exit");
    }
}