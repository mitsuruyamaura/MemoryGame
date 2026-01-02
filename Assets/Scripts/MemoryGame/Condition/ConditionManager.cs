using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// コンディションの管理クラス
/// </summary>
public class ConditionManager : MonoBehaviour {
    [SerializeField] private List<ConditionProgressData> conditionProgressDataList = new();

    [SerializeField] private List<ConditionIndicator> indicatorList = new();
    [SerializeField] private ConditionIndicatorGenerator indicatorGenerator;
    [SerializeField] private Transform indicatorTran;

    protected ConditionInfoDisplayManager conditionInfoDisplayManager;
    protected ConditionEffectFactory conditionEffectFactory;

    protected CancellationToken token;

    public void Setup(ConditionInfoDisplayManager conditionInfoDisplayManager, ConditionEffectFactory conditionEffectFactory, CancellationToken token) {
        this.conditionInfoDisplayManager = conditionInfoDisplayManager;
        this.conditionEffectFactory = conditionEffectFactory;
        this.token = token;

        indicatorGenerator.SetUp();
    }

    /// <summary>
    /// コンディションを追加。インジケーター生成
    /// </summary>
    /// <param name="conditionData"></param>
    /// <param name="conditionPowerMultiplier"></param>
    /// <param name="stackCount"></param>
    public void AddConditionList(ConditionData conditionData, float conditionPowerMultiplier, int stackCount) {
        // 既存コンディションがあるかチェック
        ConditionProgressData existingConditionProgressData = FindCondition(conditionData);
        if (existingConditionProgressData != null) {
            // 既存のコンディションがある場合にはスタックさせる
            existingConditionProgressData.AddStack(stackCount, conditionPowerMultiplier);
            return;
        }

        // コンディションの効果用クラスの生成
        IConditionEffect conditionEffect = conditionEffectFactory.Create(conditionData.conditionType);

        // コンディション生成
        ConditionProgressData conditionProgressData = new(conditionData, conditionPowerMultiplier, stackCount, conditionEffect, token);
        conditionProgressDataList.Add(conditionProgressData);

        // インジケーター生成
        GenerateConditionIndicator(conditionProgressData);
    }

    private ConditionProgressData FindCondition(ConditionData conditionData) {
        return conditionProgressDataList.FirstOrDefault(c => c.ConditionData.id == conditionData.id);
    }

    /// <summary>
    /// オブジェクトプールを利用して ConditionIndicator の生成
    /// </summary>
    /// <param name="conditionData"></param>
    /// <param name="playerCondition"></param>
    public void GenerateConditionIndicator(ConditionProgressData conditionProgressData) {
        ConditionIndicator conditionIndicator = (ConditionIndicator)indicatorGenerator.GetObjectFromPool(indicatorTran);
        conditionIndicator.SetUpIndiator(conditionInfoDisplayManager, conditionProgressData, ReleaseIndicator);
        indicatorList.Add(conditionIndicator);
    }

    /// <summary>
    /// コンディションの残り強度を更新
    /// 毎ターン終了時に実行される
    /// </summary>
    public void UpdateConditionRemainingPowers() {
        // デバッグ用治癒力の値を取得
        //int recoveryPower = GameData.instance.userData.DebuffRecoveryPower.Value;

        // 治癒力の最新値を取得(アイテムと基本値の合計値)
        int recoveryPower = GameData.instance.GetTotalRecoveryPower();

        // 各コンディションの強度から治癒力分だけ減算。強度 0 以下になったらリムーブする
        for (int i = conditionProgressDataList.Count -1; i >= 0; i--) {
            bool isExpired = conditionProgressDataList[i].UpdateRemainingPower(recoveryPower);
            if (isExpired) {
                conditionProgressDataList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// コンディションを削除。一緒にインジケーターも削除
    /// </summary>
    public void RemoveConditionList(ConditionProgressData conditionProgressData) {
        conditionProgressDataList.Remove(conditionProgressData);

        // 対応するインジケーターを探して削除
        ConditionIndicator indicator = indicatorList.FirstOrDefault(i => i.HasCondition(conditionProgressData));
        if (indicator != null) {
            indicator.Release();
        }
    }

    /// <summary>
    /// 引数に指定されたコンディションが付与されているか確認
    /// </summary>
    /// <param name="checkConditionType"></param>
    /// <returns></returns>
    public bool JudgeConditionType(ConditionType checkConditionType) {
        return conditionProgressDataList.Exists(condition => condition.ConditionData.conditionType == checkConditionType);
    }

    /// <summary>
    /// 終了した Condition をリストから削除
    /// </summary>
    /// <param name="conditionIndicator"></param>
    private void ReleaseIndicator(ConditionIndicator conditionIndicator) {
        indicatorList.Remove(conditionIndicator);
    }

    public void PlayConditionSE(ConditionData conditionData) {
        // コンディションの種類で SE 選択
        if (conditionData.conditionPolarity == ConditionPolarity.Positive) {
            SoundManager.instance.PlaySE(SE_TYPE.Buff);
        } else {
            SoundManager.instance.PlaySE(SE_TYPE.Debuff);
        }
    }

    /// <summary>
    /// お手付きのタイミングで実行される
    /// 主に猛毒と散漫を処理する
    /// </summary>
    public void ApplyMissteps() {
        foreach (ConditionProgressData condition in conditionProgressDataList) {
            condition.ConditionEffect?.OnMisstep(condition);
        }
    }

    /// <summary>
    /// ソウルポイント獲得のタイミングで実行される
    /// 主に封印を処理する
    /// </summary>
    /// <param name="baseExp"></param>
    /// <returns></returns>
    public int ApplyExpModifiers(int baseExp) {
        int exp = baseExp;
        foreach (ConditionProgressData condition in conditionProgressDataList) {
            if (condition is IExpModifier modifier) {
                exp = modifier.ModifyExp(exp);
            }
        }
        return exp;
    }

    /// <summary>
    /// 集中力加算のタイミングで実行される
    /// 主に幻覚を処理する
    /// </summary>
    /// <param name="baseFlipPoint"></param>
    /// <returns></returns>
    public int ApplyFlipPointModifiers(int baseFlipPoint) {
        int flipPoint = baseFlipPoint;
        foreach (ConditionProgressData condition in conditionProgressDataList) {
            if (condition is IFlipPointModifier modifier) {
                flipPoint = modifier.ModifyFlipPoint(flipPoint);
            }
        }
        return flipPoint;
    }
}