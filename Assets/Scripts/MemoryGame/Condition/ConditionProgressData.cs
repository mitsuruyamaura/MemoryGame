using System;
using UnityEngine;
using R3;
using System.Threading;
using UnityEngine.Events;

/// <summary>
/// コンディションの進捗管理クラス
/// </summary>
[System.Serializable]
public class ConditionProgressData {
    public SerializableReactiveProperty<int> RemainingPower = new();　　// 強度
    public SerializableReactiveProperty<int> StackCount = new();        // スタック数

    [SerializeField] protected float conditionValue;       //  効果  =>  攻撃力を増減する値、マップの見える範囲を増減する値


    [SerializeField] protected ConditionData conditionData;
    public ConditionData ConditionData => conditionData; 

    protected IDisposable disposable;
    protected UnityAction<ConditionProgressData> onExpiredAction;
    protected CancellationToken token;
    protected IConditionEffect conditionEffect;
    public IConditionEffect ConditionEffect => conditionEffect;


    public ConditionProgressData(ConditionData conditionData, float conditionPowerMultiplier, int stackCount, UnityAction<ConditionProgressData> onExpiredAction, IConditionEffect conditionEffect, CancellationToken token) {
        this.conditionData = conditionData;

        int remainingPower = CalcRemainingPower(this.conditionData.conditionPower, conditionPowerMultiplier);
        RemainingPower.Value = Mathf.Min(RemainingPower.Value + remainingPower, conditionData.maxConditionPower);

        StackCount.Value = stackCount;
        conditionValue = this.conditionData.value;

        this.onExpiredAction = onExpiredAction;
        this.conditionEffect = conditionEffect;
        this.token = token;
    }

    /// <summary>
    /// コンディションの残り強度の更新
    /// </summary>Pro
    public void AddRemainingPower(int recoveryPower) {
        // 残り強度を減算
        RemainingPower.Value -= recoveryPower;
        DebugLogger.Log($"{this} 残り強度 : {RemainingPower.Value}");

        // コンディションの残り強度がなくなったら
        if (RemainingPower.Value <= 0) {
            // マネージャーからコンディションを削除してもらう
            onExpiredAction?.Invoke(this);
        }
    }

    /// <summary>
    /// 強度計算
    /// </summary>
    /// <param name="conditionPower"></param>
    /// <param name="conditionPowerMultiplier"></param>
    /// <returns></returns>
    public int CalcRemainingPower(int conditionPower, float conditionPowerMultiplier) {
        return Mathf.FloorToInt(conditionPower * conditionPowerMultiplier);
    }

    /// <summary>
    /// スタック加算
    /// </summary>
    /// <param name="addStackCount"></param>
    /// <param name="powerMultiplier"></param>
    public void AddStack(int addStackCount, float powerMultiplier) {
        StackCount.Value = Mathf.Min(StackCount.Value + addStackCount, conditionData.maxStackCount);

        int addRemainingPower = CalcRemainingPower(conditionData.conditionPower, powerMultiplier);
        RemainingPower.Value = Mathf.Min(RemainingPower.Value + addRemainingPower, conditionData.maxConditionPower);
    }
}