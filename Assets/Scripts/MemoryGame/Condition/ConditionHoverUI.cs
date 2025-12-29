using UnityEngine;
using UnityEngine.EventSystems;

public class ConditionHoverUI : MonoBehaviour {
    private ConditionInfoDisplayManager conditionInfoDisplayManager;
    private ConditionProgressData conditionProgressData;


    public void Setup(ConditionInfoDisplayManager conditionInfoDisplayManager, ConditionProgressData conditionProgressData) {
        this.conditionInfoDisplayManager = conditionInfoDisplayManager;
        this.conditionProgressData = conditionProgressData;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        if (GameData.instance == null) {
            return;
        }

        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        if (conditionProgressData == null) {
            return;
        }

        conditionInfoDisplayManager?.ShowConditionInfo(conditionProgressData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (GameData.instance == null) {
            return;
        }

        conditionInfoDisplayManager?.HideConditionInfo();
    }
}