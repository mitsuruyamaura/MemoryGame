using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 行動に失敗した場合のメッセージの種類
/// </summary>
public enum ReleaseActionMessageType {
    NotRelease_SoulPoint,    // ソウルポイントが足りなくて、呪いが解除できなかったとき
    NotRelease_Status,       // ステータスが足りなくて、パワースポットが解放できなかったとき
    Release,                 // 呪い解除成功のとき
    Blessing,                // パワースポットを解放して祝福を受けたとき
    SuccessSettlement        // 交渉成功
}


public class PowerSpotInfoDisplayManager : AbstractSingleton<PowerSpotInfoDisplayManager>
{
    [SerializeField] private Canvas powerSpotInfoCanvas;
    [SerializeField] private Image imgPowerSpotIcon;
    [SerializeField] private Image[] imgRarityIcons;
    [SerializeField] private Text[] txtDescs;
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtNeedSoutPoint;
    [SerializeField] private Text txtPowerPointInfo;    // 画面優先順位の関係上、Canvas_ItemInfo オブジェクト内にあるオブジェクトを使う

    [SerializeField] private Color noReleaseMessageColor;
    [SerializeField] private Color releaseMessageColor;
    [SerializeField] private CircleOutline circleOutline;

    private string soulPointMessage = "ソウルポイントが足りないため、祝福を受けられません";  // 呪いが発動しました
    private string statusPointMessage = "ステータスが足りないため、解放できません";
    private string curseReleaseMessage = "ソウルポイントを消費して、呪いを解除しました！";
    private string blessingMessage = "祝福を受け、ポーチの最大数が増えました!!";
    private string SuccessSettlementMessage = "平和的解決に成功しました!!";

    void Start() {
        HidePowerSpotInfo();
        txtPowerPointInfo.DOFade(0, 0);
    }

    public void ShowPowerSpotInfo(PowerSpotData powerSpotData) {
        txtName.text = powerSpotData.name;

        //txtDesc.text = backPackInItem.currentCoolTime.ToString() + "\n";
        //txtDesc.text += backPackInItem.currentAccuracy.ToString() + "\n";

        // レアリティアイコンの表示
        for (int i = 0; i < imgRarityIcons.Length; i++) {
            imgRarityIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < (int)powerSpotData.rarity + 1; i++) {
            imgRarityIcons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < txtDescs.Length; i++) {
            txtDescs[i].text = string.Empty;
        }

        //txtDescs[0].text = "ReleasePoint : " + powerSpotData.releasePoint.ToString() + "\n";
        //txtDescs[0].text += $"能力値の合計値が {powerSpotData.releasePoint} 以上で\nパワースポット解放";

        //txtDescs[1].text += $"あるいは以下の能力値が\n指定値以上であればパワースポット解放\n\n";
        //for (int i = 0; i < powerSpotData.statusTypes.Length; i++) {
        //    txtDescs[1].text += powerSpotData.statusTypes[i].ToString() + " : " + powerSpotData.requiredValues[i].ToString() + "\n";
        //}

        txtDescs[2].text = $"パワースポット解放による祝福効果\n";
        txtDescs[2].text += powerSpotData.desc;

        txtDescs[3].text = "Rarity : " + powerSpotData.rarity.ToString();

        // アイコン画像の設定
        //Sprite itemIcon = DataBaseManager.instance.GetItemIcon(powerSpotData.no);
        //if (itemIcon != null) {
        //    imgPowerSpotIcon.sprite = itemIcon;
        //}

        // TODO 必要ソウルポイント(一旦、ステータスのトータル値で)
        txtNeedSoutPoint.text = powerSpotData.releasePoint.ToString(); // (ConstData.POWER_SPOT_NEED_RELEASE_POINT * GameData.instance.userData.waveNo).ToString();

        if (powerSpotInfoCanvas != null) {
            powerSpotInfoCanvas.enabled = true;
        }
    }


    public void HidePowerSpotInfo() {
        if (powerSpotInfoCanvas != null) {
            powerSpotInfoCanvas.enabled = false;
        }
    }


    /// <summary>
    /// パワースポットを解放できないときのメッセージ表示
    /// </summary>
    /// <param name="message"></param>
    public void NotReleasePowerPointInfo(ReleaseActionMessageType noReleaseMessageType) {
        string message = noReleaseMessageType switch {
            ReleaseActionMessageType.NotRelease_SoulPoint => soulPointMessage,
            ReleaseActionMessageType.NotRelease_Status => statusPointMessage,
            ReleaseActionMessageType.Release => curseReleaseMessage,
            _ => statusPointMessage,
        };
        circleOutline.SetEffectColor(noReleaseMessageColor);
        txtPowerPointInfo.text = message;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtPowerPointInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtPowerPointInfo.DOFade(0f, 0.5f).SetEase(Ease.Linear)).OnComplete(() => txtPowerPointInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }

    /// <summary>
    /// パワースポットを解放したときのメッセージ表示
    /// </summary>
    public void ReleasePowerSpotInfoByBlessing() {
        circleOutline.SetEffectColor(releaseMessageColor);
        txtPowerPointInfo.text = blessingMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtPowerPointInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtPowerPointInfo.DOFade(0f, 0.5f).SetEase(Ease.Linear)).OnComplete(() => txtPowerPointInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }
    
    /// <summary>
    /// 交渉に成功したときのメッセージ表示
    /// </summary>
    public void SuccessSettlementInfo() {
        circleOutline.SetEffectColor(releaseMessageColor);
        txtPowerPointInfo.text = SuccessSettlementMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtPowerPointInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtPowerPointInfo.DOFade(0f, 0.5f).SetEase(Ease.Linear)).OnComplete(() => txtPowerPointInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }
}