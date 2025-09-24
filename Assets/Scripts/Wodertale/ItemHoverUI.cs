using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverUI : MonoBehaviour
{
    private BackPackInItem backPackInItem;


    void Start() {
        TryGetComponent(out backPackInItem);
    }

    // Image の場合、RaycastTarget は不要だが、Collider2D は必要
    public void OnMouseEnter() {
        if (GameData.instance.CurrentGameState.Value != GameData.GameState.Play) {
            return;
        }
        if (ItemInfoDisplayManager.instance.isTreasureShow) {
            return;
        }
        // ほかの RaycastTarget があると、それの優先順位が下でも反応してしまうので、これは止めておく
        // UIのホバーやボタンなどを選択している場合、そちらを優先する
        //if (EventSystem.current.IsPointerOverGameObject()) {
        //    return;
        //}

        if(backPackInItem == null || backPackInItem.itemData == null) {
            return;
        }
        
        ItemInfoDisplayManager.instance.ShowItemInfo(backPackInItem);
        //Debug.Log("Enter");
    }

    public void OnMouseExit() {
        if (ItemInfoDisplayManager.instance.isTreasureShow) {
            return;
        }

        ItemInfoDisplayManager.instance.HideItemInfo();
        //Debug.Log("Exit");
    }
}