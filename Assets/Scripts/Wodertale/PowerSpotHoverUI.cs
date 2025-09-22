using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSpotHoverUI : MonoBehaviour
{
    private PowerSpotData powerSpotData;


    //void Start() {
    //    if(TryGetComponent(out PowerSpotSymbol powerSpotSymbol)){
    //        powerSpotData = powerSpotSymbol.powerSpotData;
    //    }
    //}


    public void SetPowerSpotData(PowerSpotData powerSpotData) {
        this.powerSpotData = powerSpotData;
    }

    // Image の場合、RaycastTarget は不要だが、Collider2D は必要
    public void OnMouseEnter() {
        if (GameData.instance.gameState.Value != GameData.GameState.Play) {
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

        if (powerSpotData == null) {
            return;
        }

        // マウスカーソルが今UI上にあるかチェック
        if (!IsPointerOverUIObject()) {
            PowerSpotInfoDisplayManager.instance.ShowPowerSpotInfo(powerSpotData);
            //Debug.Log("Enter");
        }
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void OnMouseExit() {
        if (ItemInfoDisplayManager.instance.isTreasureShow) {
            return;
        }

        PowerSpotInfoDisplayManager.instance.HidePowerSpotInfo();
        //Debug.Log("Exit");
    }
}