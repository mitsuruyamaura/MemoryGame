using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition_Disease : PlayerConditionBase {

    private float originValue;

    /// <summary>
    /// 移動速度半減
    /// </summary>
    /// <returns></returns>
    protected override void OnEnterCondition() {
        conditionValue = 0.5f;

        //// 元に戻すために保持
        //originValue = GameData.instance.currentCharaData.moveSpeed;

        //// バトル時の移動速度を半減
        //GameData.instance.currentCharaData.moveSpeed *= conditionValue;

        base.OnEnterCondition();
    }

    /// <summary>
    /// 移動速度を戻す
    /// </summary>
    /// <returns></returns>
    protected override void OnExitCondition() {

        // 移動速度を元の値に戻す
        //GameData.instance.currentCharaData.moveSpeed = originValue;

        base.OnExitCondition();
    }
}
