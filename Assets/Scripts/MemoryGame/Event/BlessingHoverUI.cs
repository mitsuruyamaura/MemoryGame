using UnityEngine;
using UnityEngine.EventSystems;

public class BlessingHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private BlessingInfoDisplayManager blessingInfoDisplayManager;
    private BlessingData blessingData;


    public void Setup(BlessingInfoDisplayManager blessingInfoDisplayManager, BlessingData blessingData) {
        this.blessingInfoDisplayManager = blessingInfoDisplayManager;
        this.blessingData = blessingData;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        if (GameData.instance == null) {
            return;
        }

        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        if (blessingData == null) {
            return;
        }

        blessingInfoDisplayManager?.ShowBlessingInfo(blessingData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (GameData.instance == null) {
            return;
        }

        blessingInfoDisplayManager?.HideBlessingInfo();
    }
}