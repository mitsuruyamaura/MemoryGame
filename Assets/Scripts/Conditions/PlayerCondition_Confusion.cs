using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PlayerCondition_Confusion : PlayerConditionBase 
{
    Tween tween;

    /// <summary>
    /// 混乱によるランダム移動と足踏み不可の効果のうち、足踏み不可の効果
    /// </summary>
    protected override async UniTask ApplyEffect(CancellationToken token) {

        // Stage 側で制御
        //mapMoveController.GetStage().GetInputManager().SwitchActivateSteppingButton(false);

        await base.ApplyEffect(token);

        DebugLogger.Log("Confusion");
    }
}
