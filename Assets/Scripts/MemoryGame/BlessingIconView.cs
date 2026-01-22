using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 祝福カードの効果アイコン表示クラス
/// </summary>
public class BlessingIconView : PoolBase {
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtDuration;
    [SerializeField] private BlessingData blessingData;
    [SerializeField] private BlessingHoverUI blessingHoverUI;

    public void Setup(BlessingInfoDisplayManager blessingInfoDisplayManager, BlessingData blessingData) {
        isReleased = false;

        this.blessingData = blessingData;

        UpdateDisplayDuration((int)blessingData.value);

        blessingHoverUI.Setup(blessingInfoDisplayManager, blessingData);

        // アイコン画像設定
        Sprite iconImage = blessingData.GetIcon();
        if (iconImage != null) {
            imgIcon.sprite = iconImage;
        }
    }

    public void UpdateDisplayDuration(int duration) {
        txtDuration.text = duration.ToString();
    }

    /// <summary>
    /// アイコンを消すか判定
    /// </summary>
    /// <param name="blessingType"></param>
    /// <param name="blessingValueType"></param>
    public void CheckEndBlessing(BlessingType blessingType, BlessingValueType blessingValueType) {
        if (blessingData.type == blessingType && blessingData.valueType == blessingValueType) {
            Release();
        }        
    }

    public override void Release() {
        base.Release();

        if (this == null || transform == null) return;

        transform.SetParent(GameData.instance.transform);
        transform.localPosition = Vector3.zero;
    }
}