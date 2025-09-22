using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCondition_Curse : PlayerConditionBase
{
    Tween tween; 

    protected override void OnEnterCondition() {

        base.OnEnterCondition();

        //tween = conditionEffect.transform.DOLocalMoveY(0.25f, 01.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }


    protected override void OnExitCondition() {

        tween.Kill();

        base.OnExitCondition();
    }
}
