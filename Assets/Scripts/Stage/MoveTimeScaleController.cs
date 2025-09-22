using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シンボル移動時の速度の種類と設定値
/// </summary>
public enum MoveTimeScale {
    Normal = 100,
    One_Half = 75,
    Double = 50,
    Count = 3
}

public class MoveTimeScaleController : MonoBehaviour
{
    [SerializeField]
    private Sprite normalSpeedIcon;

    [SerializeField]
    private Sprite oneHalfSpeedIcon;

    [SerializeField]
    private Sprite doubleSpeedIcon;

    [SerializeField]
    private Image imgSpeedIcon;

    [SerializeField]
    private Button btnStaminaFrame;

    public MoveTimeScale currentMoveTimeScale;
    public int currentTimeScaleNo;

    /// <summary>
    /// 初期設定
    /// </summary>
    public void SetUpMoveButtonController() {
        currentMoveTimeScale = MoveTimeScale.Normal;
        imgSpeedIcon.sprite = normalSpeedIcon;
        btnStaminaFrame.onClick.AddListener(OnClickSwitchMoveTimeScale);
    }

    /// <summary>
    /// 残りのスタミナ数表示ボタンを押した際の処理
    /// </summary>
    private void OnClickSwitchMoveTimeScale() {

        // MoveTimeScale を１つ進める。Normal => OneHalf => Double => Normal でサイクル化
        (Sprite nextSprite, MoveTimeScale nextMoveTimeScaleType) nextTimeScaleValue = currentMoveTimeScale switch
        {
            MoveTimeScale.Normal => (oneHalfSpeedIcon, MoveTimeScale.One_Half),
            MoveTimeScale.One_Half => (doubleSpeedIcon, MoveTimeScale.Double),
            MoveTimeScale.Double => (normalSpeedIcon, MoveTimeScale.Normal),
            _ => (normalSpeedIcon, MoveTimeScale.Normal)
        };

        //    // enum 管理
        //    currentMoveTimeScale = currentMoveTimeScale switch {
        //    MoveTimeScaleType.Normal => MoveTimeScaleType.One_Half,
        //    MoveTimeScaleType.One_Half => MoveTimeScaleType.Double,
        //    MoveTimeScaleType.Double => MoveTimeScaleType.Normal,
        //    _ => MoveTimeScaleType.Normal
        //};

        // int 管理
        currentTimeScaleNo++;
        currentTimeScaleNo = currentTimeScaleNo % (int)MoveTimeScale.Count == 0 ? 0 : currentTimeScaleNo;

        //// アイコン画像の設定
        //imgSpeedIcon.sprite = currentMoveTimeScale switch {
        //    MoveTimeScaleType.Normal => normalSpeedIcon,
        //    MoveTimeScaleType.One_Half => oneHalfSpeedIcon,
        //    MoveTimeScaleType.Double => doubleSpeedIcon,
        //    _ => normalSpeedIcon
        //};

        // MoveTimeScale を変更
        currentMoveTimeScale = nextTimeScaleValue.nextMoveTimeScaleType;

        // アイコン画像の変更し、現在の MoveTimeScale に合わせる
        imgSpeedIcon.sprite = nextTimeScaleValue.nextSprite;

        // プレイヤーとエネミーシンボルの移動速度の設定値を現在の MoveTimeScale の内容に更新
        GameData.instance.moveTimeScale = (float)currentMoveTimeScale / 100;
    }
}
