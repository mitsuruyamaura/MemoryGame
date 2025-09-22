using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition_Fatigue : PlayerConditionBase
{
    private int originValue;

    /// <summary>
    /// 攻撃力半減
    /// </summary>
    /// <returns></returns>
    protected override void OnEnterCondition() {
        conditionValue = 0.5f;

        // 元に戻すために保持
        //originValue = GameData.instance.currentCharaData.attackPower;

        // バトル時の攻撃力を半減
        //GameData.instance.currentCharaData.attackPower = Mathf.FloorToInt(GameData.instance.currentCharaData.attackPower * conditionValue);

        base.OnEnterCondition();
    }

    /// <summary>
    /// 攻撃力を戻す
    /// </summary>
    /// <returns></returns>
    protected override void OnExitCondition() {

        // 攻撃力を元の値に戻す
        //GameData.instance.currentCharaData.attackPower = originValue;

        base.OnExitCondition();
    }
}
