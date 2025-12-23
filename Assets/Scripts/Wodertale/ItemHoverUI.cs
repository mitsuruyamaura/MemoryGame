using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// アイテムアイコンのホバー処理
/// </summary>
public class ItemHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private BackPackInItem backPackInItem;
    private ItemInfoDisplayManager itemInfoDisplayManager;

    //void Start() {
    //    TryGetComponent(out backPackInItem);
    //}

    public void Setup(BackPackInItem backPackInItem, ItemInfoDisplayManager itemInfoDisplayManager) {
        this.backPackInItem = backPackInItem;
        this.itemInfoDisplayManager = itemInfoDisplayManager;
    }

    // こちらは、マスクで画面上から見えなくなっている場合でも反応してしまうので、今回は使わない。
    // Image の場合、RaycastTarget は不要だが、Collider2D は必要
    //public void OnMouseEnter() {
    //    if (GameData.instance.CurrentGameState.Value != GameData.GameState.Play) {
    //        return;
    //    }
    //    //if (ItemInfoDisplayManager.instance.isTreasureShow) {
    //    //    return;
    //    //}
    //    // ほかの RaycastTarget があると、それの優先順位が下でも反応してしまうので、これは止めておく
    //    // UIのホバーやボタンなどを選択している場合、そちらを優先する
    //    //if (EventSystem.current.IsPointerOverGameObject()) {
    //    //    return;
    //    //}

    //    if (backPackInItem == null || backPackInItem.itemData == null) {
    //        return;
    //    }

    //    ItemInfoDisplayManager.instance.ShowItemInfo(backPackInItem);
    //    //Debug.Log("Enter");
    //}

    //public void OnMouseExit() {
    //    //if (ItemInfoDisplayManager.instance.isTreasureShow) {
    //    //    return;
    //    //}

    //    ItemInfoDisplayManager.instance.HideItemInfo();
    //    //Debug.Log("Exit");
    //}

    // こちらは Image であっても Collider2D は不要。仮にあっても動作する。
    // RaycastTarget がある UI 要素の上に乗っている場合は反応しないので、マスクされている場合は反応しない。
    public void OnPointerEnter(PointerEventData eventData) {
        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        if (backPackInItem == null || backPackInItem.itemData == null) {
            return;
        }

        itemInfoDisplayManager.ShowItemInfo(backPackInItem);
    }

    public void OnPointerExit(PointerEventData eventData) {
        itemInfoDisplayManager.HideItemInfo();
    }
}