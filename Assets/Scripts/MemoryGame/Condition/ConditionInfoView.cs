using UnityEngine;
using UnityEngine.UI;

public class ConditionInfoView : InfoViewBase {
    [SerializeField] private Text txtRemainingPower;

    /// <summary>
    /// コンディション情報表示
    /// </summary>
    /// <param name="conditionProgressData"></param>
    public void ShowConditionInfo(ConditionProgressData conditionProgressData) {
        isReleased = false;

        txtName.text = $"{conditionProgressData.ConditionData.name} : {conditionProgressData.StackCount.Value}";

        txtDesc.text = conditionProgressData.ConditionData.desc.Replace("\\n", "\n");

        txtRemainingPower.text = $"強度 : {conditionProgressData.RemainingPower.Value}";

        // アイコン画像の設定
        Sprite itemIcon = DataBaseManager.instance.GetConditionIcon(conditionProgressData.ConditionData.id);
        if (itemIcon != null) {
            imgMain.sprite = itemIcon;
        }

        cg.alpha = 1;
    }
}