using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyHoverUI : MonoBehaviour {

    private EnemySymbol enemySymbol;

    private bool isMouseOver = false;


    void Start() {
        TryGetComponent(out enemySymbol);
    }

    // この方法だと、UI に隠れている場合でも反応してしまうため、自前で実装する
    //public void OnMouseEnter() {        
    //    // UIのホバーやボタンなどを選択している場合、そちらを優先する
    //    if (EventSystem.current.IsPointerOverGameObject()) {
    //        return;
    //    }

    //    EnemyInfoDisplayManager.instance.ShowEnemyInfo(enemySymbol.enemyData, enemySymbol.nameData.name, enemySymbol.equipItemNoList);
    //    //Debug.Log("Enter");
    //}

    //public void OnMouseExit() {
    //    EnemyInfoDisplayManager.instance.HideEnemyInfo();
    //    //Debug.Log("Exit");
    //}

    private void Update() {
        if (GameData.instance.gameState.Value != GameData.GameState.Play) {
            return;
        }

        // マウスカーソルが今UI上にあるかチェック
        if (!IsPointerOverUIObject()) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject) {
                if (!isMouseOver) {
                    // OnMouseEnter の代わり
                    EnemyInfoDisplayManager.instance.ShowEnemyInfo(enemySymbol.enemyData, enemySymbol.nameData.name, enemySymbol.equipItemNoList, GameData.GameState.Play);
                    isMouseOver = true;
                }
            } else {
                if (isMouseOver) {
                    // OnMouseExit の代わり
                    EnemyInfoDisplayManager.instance.HideEnemyInfo();
                    isMouseOver = false;
                }
            }
        } else {
            if (isMouseOver) {
                // UIの下にマウスが移動した場合の OnMouseExit の代わり
                EnemyInfoDisplayManager.instance.HideEnemyInfo();
                isMouseOver = false;
            }
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
}