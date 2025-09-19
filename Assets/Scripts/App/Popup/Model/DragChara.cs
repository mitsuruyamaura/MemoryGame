using R3;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragChara : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Transform originalParent;            // 初期配置時、およびドラッグ開始時の親を記憶
    private Canvas canvas;
    private RectTransform rectTransform;
    private RectTransform rectTransformCanvas;
    private CanvasGroup canvasGroup;

    private bool isDropped = false;              // ドロップが成功したかどうかのフラグ

    private DropCharaArea currentDropCharaArea;
    public DropCharaArea CurrentDropCharaArea {  // プロパティ。currentDropCharaArea が null のときには再取得する
        get {
            if (currentDropCharaArea == null && originalParent != null) {
                originalParent.TryGetComponent(out currentDropCharaArea);
            }
            return currentDropCharaArea;
        }
    }

    private int slotIndex;                       // 配置されているスロットのインデックス
    public int SlotIndex => slotIndex;

    public Subject<(int oldIndex, int newIndex)> OnDropped = new(); // ドロップイベント通知

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="parentCanvas"></param>
    /// <param name="index"></param>
    public void SetupDragChara(Canvas parentCanvas, int index) {
        TryGetComponent(out rectTransform);
        TryGetComponent(out canvasGroup);

        canvas = parentCanvas;
        rectTransformCanvas = canvas.transform as RectTransform;

        slotIndex = index;

        originalParent = transform.parent;
        originalParent.TryGetComponent(out currentDropCharaArea);
    }

    /// <summary>
    /// ドロップしたことを通知する
    /// TeamPageView で購読している
    /// </summary>
    /// <param name="newIndex"></param>
    public void NotifyDrop(int newIndex) {
        // 配置されているスロットのインデックスと、新しくドロップしたスロットのインデックスを通知
        OnDropped.OnNext((slotIndex, newIndex));

        // 更新
        slotIndex = newIndex;
    }

    /// <summary>
    /// ドラッグ開始時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData) {
        //AudioManager.instance.PlaySE(ENUM_SE.PREBATTLE_CHARA_SELECT);
        currentDropCharaArea?.ReleaseChara();
        currentDropCharaArea = null;

        // 元の親を保存
        originalParent = transform.parent;

        // Canvas の直下に移動
        transform.SetParent(canvas.transform, false);
        
        // 最前面に移動
        transform.SetAsLastSibling();

        // ドラッグ中はレイキャストを無視（ドロップ判定のため）
        canvasGroup.blocksRaycasts = false;

        isDropped = false;
    }

    /// <summary>
    /// ドラッグ中の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData) {
        // ドラッグしているオブジェクトの Image を表示したまま移動させる
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformCanvas, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) {
            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }

    /// <summary>
    /// ドラッグ後の最後の処理
    /// OnDrop 後に呼ばれるコールバック
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData) {
        // レイキャストを有効に戻す
        canvasGroup.blocksRaycasts = true;

        // すでに OnDrop で処理された場合は何もしない
        if (isDropped) {
            return;
        }

        // どこにもドロップされなかった場合、元の場所に戻す
        ReturnDropArea();
    }

    /// <summary>
    /// キャラを元居たスロットの場所に戻す
    /// </summary>
    public void ReturnDropArea() {
        transform.SetParent(originalParent, false);
        transform.localPosition = Vector3.zero;
        if (originalParent.TryGetComponent(out currentDropCharaArea)) {
            // 元のスロットに戻す
            currentDropCharaArea.SetChara(this);
        }
    }

    public void SetCurrentDropCharaArea(DropCharaArea dropCharaArea) {
        currentDropCharaArea = dropCharaArea;
        slotIndex = dropCharaArea.SlotIndex;
    }

    public void SetDropped() {
        isDropped = true;
    }
}