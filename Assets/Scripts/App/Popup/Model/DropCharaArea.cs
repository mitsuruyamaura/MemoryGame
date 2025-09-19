using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropCharaArea : MonoBehaviour, IBeginDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Image containerImage;

    private int slotIndex;
    public int SlotIndex => slotIndex;

    private Color normalColor;
    private Color highlightColor = Color.yellow;
    private DragChara currentChara;                  // 現在このスロットに配置されているキャラを保持
    private string permitPointerObjectName = "TEAM_POWER"; // ポインター処理が許容するオブジェクト名

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="index"></param>
    public void SetupDropCharaArea(int index) {
        slotIndex = index;

        if (containerImage != null) {
            normalColor = containerImage.color;
        }
    }

    /// <summary>
    /// ドラッグ開始時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData) {
        currentChara = null;
    }

    /// <summary>
    /// スロット上にキャラをドロップ時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData) {
        containerImage.color = normalColor;

        // ドロップしたオブジェクトの取得
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) {
            return;            
        }

        // ドロップされたキャラ情報の取得
        if (droppedObject.TryGetComponent(out DragChara droppedChara)) {
            droppedChara.SetDropped();

            // 現在のスロットに既にキャラがいる場合、配置を入れ替える
            if (currentChara != null) {
                // ドロップされてきたキャラのいたスロットの DropCharaArea を取得
                DropCharaArea previousSlotArea = droppedChara.CurrentDropCharaArea;

                // 今スロット内にいるキャラを、ドロップされてきたキャラの位置と交換
                currentChara.transform.SetParent(previousSlotArea.transform, false);
                currentChara.transform.localPosition = Vector3.zero;

                // 同様に情報も交換
                currentChara.SetCurrentDropCharaArea(previousSlotArea);
                previousSlotArea.currentChara = currentChara;
            }

            // ドロップされたキャラをスロットへ配置(空のスロットにドロップした場合はここのみ行う)
            droppedChara.transform.SetParent(transform, false);
            droppedChara.transform.localPosition = Vector3.zero;

            // 新しいインデックスを通知(元の Index を保持しているので、交換も行う)
            droppedChara.NotifyDrop(slotIndex);

            // スロットに配置されたキャラ情報の更新
            currentChara = droppedChara;

            // 新しく配置したキャラのスロット情報を更新
            droppedChara.SetCurrentDropCharaArea(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // マウスが侵入しても、ドラッグ中のオブジェクトがない場合は反応しない
        // Team を含まない名前のゲームオブジェクトをドラッグしても反応しない
        if (containerImage == null || eventData.pointerDrag == null || !eventData.pointerDrag.name.Contains(permitPointerObjectName)) {
            return;
        }

        // ドロップ可能な範囲の色を変える
        containerImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        // マウスが出た時も、ドラッグ中のオブジェクトがない場合は反応しない
        // Team を含まない名前のゲームオブジェクトをドラッグしても反応しない
        if (containerImage == null || eventData.pointerDrag == null || !eventData.pointerDrag.name.Contains(permitPointerObjectName)) {
            return;
        }

        // ドロップ可能な範囲の色を戻す
        containerImage.color = normalColor;
    }

    /// <summary>
    /// 配置中のキャラ情報をセット
    /// 下のスクロールバーからスロットに初期配置されたときや、元のスロット位置に戻った時に実行
    /// </summary>
    /// <param name="dragChara"></param>
    public void SetChara(DragChara dragChara) {
        currentChara = dragChara;
    }

    /// <summary>
    /// 配置中のキャラ情報を削除
    /// </summary>
    public void ReleaseChara() {
        currentChara = null;
    }
}