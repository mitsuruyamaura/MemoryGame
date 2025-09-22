using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PlayerCondition_Sleep : PlayerConditionBase
{
    Tween tween;

    /// <summary>
    /// 睡眠による移動制限の効果(足踏みしかできない)
    /// </summary>
    protected override async UniTask ApplyEffect(CancellationToken token) {

        // Stage 側で制御
        //mapMoveController.GetStage().GetInputManager().SwitchActivateMoveButtons(false);

        DebugLogger.Log("Sleep");
        await base.ApplyEffect(token);
    }

    protected override void OnEnterCondition() {

        base.OnEnterCondition();

        //tween = conditionEffect.transform.DOLocalMoveY(0.25f, 0.5f).SetEase(Ease.InQuart).SetLoops(-1, LoopType.Yoyo);
    }


    protected override void OnExitCondition() {

        //tween.Kill();

        base.OnExitCondition();
    }
}
