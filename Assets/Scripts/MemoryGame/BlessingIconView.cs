using UnityEngine;
using UnityEngine.UI;

public class BlessingIconView : PoolBase {
    [SerializeField] private Image imgIcon;
    [SerializeField] private BlessingData blessingData;

    public void Setup(BlessingData blessingData) {
        isReleased = false;

        this.blessingData = blessingData;

        // TODO アイコン画像設定

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
        imgIcon.sprite = null;
    }
}