using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : AbstractSingleton<ConditionManager>
{
    [SerializeField] private List<ConditionIndicator> indicatorList = new();
    [SerializeField] private ConditionIndicatorGenerator indicatorGenerator;
    [SerializeField] private Transform indicatorTran;


    protected override void Awake() {
        base.Awake();
        indicatorGenerator.SetUp();
    }

    /// <summary>
    /// オブジェクトプールを利用して ConditionIndicator の生成
    /// </summary>
    /// <param name="conditionData"></param>
    /// <param name="playerCondition"></param>
    //public void GenerateConditionIndicator(ConditionData conditionData, PlayerConditionBase playerCondition) {
    //    ConditionIndicator conditionIndicator = (ConditionIndicator)indicatorGenerator.GetObjectFromPool(indicatorTran);
    //    conditionIndicator.SetUpIndiator(conditionData, playerCondition, ReleaseIndicator);
    //    indicatorList.Add(conditionIndicator);
    //}

    /// <summary>
    /// 終了した Condition をリストから削除
    /// </summary>
    /// <param name="conditionIndicator"></param>
    private void ReleaseIndicator(ConditionIndicator conditionIndicator) {
        indicatorList.Remove(conditionIndicator);
    }


    // シンボル内で処理可能になったため未使用
    //public void AddCondition(ConditionType addConditionType) {
    //    ConditionData conditionData = DataBaseManager.instance.conditionDataSO.conditionDatasList.FirstOrDefault(data => data.conditionType == addConditionType);

    //    // すでに同じコンディションが付与されているか確認
    //    if (GameData.instance.JudgeConditionType(addConditionType)) {
    //        // すでに付与されている場合は、持続時間を更新し、効果は上書きして処理を終了する


    //        GameData.instance.UpdateConditionDuration(conditionData);

    //        //mapMoveController.GetConditionsList().Find(x => x.GetConditionType() == conditionType).ExtentionCondition(duration, itemValue);
    //        return;
    //    }

    //    // 付与するコンディションが睡眠かつ、すでに混乱のコンディションが付与されているときには、睡眠のコンディションは無視する(操作不能になるため)
    //    if (addConditionType == ConditionType.Sleep && GameData.instance.JudgeConditionType(ConditionType.Confusion)) {
    //        return;
    //    }

    //    // 付与されていないコンディションの場合は、付与する準備する
    //    PlayerConditionBase playerCondition = new();

        // Player にコンディションを付与
        //playerCondition = conditionType switch {
        //    ConditionType.View_Wide => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
        //    ConditionType.View_Narrow => mapMoveController.gameObject.AddComponent<PlayerCondition_View>(),
        //    ConditionType.Hide_Symbols => mapMoveController.gameObject.AddComponent<PlayerCondition_HideSymbol>(),
        //    ConditionType.Untouchable => mapMoveController.gameObject.AddComponent<PlayerCondition_Untouchable>(),
        //    ConditionType.Walk_through => mapMoveController.gameObject.AddComponent<PlayerCondition_WalkThrough>(),
        //    ConditionType.Sleep => mapMoveController.gameObject.AddComponent<PlayerCondition_Sleep>(),
        //    ConditionType.Confusion => mapMoveController.gameObject.AddComponent<PlayerCondition_Confusion>(),
        //    ConditionType.Curse => mapMoveController.gameObject.AddComponent<PlayerCondition_Curse>(),
        //    _ => null
        //};

        // 生成するエフェクトのプレファブを取得
        //ConditionEffect conditionEffectPrefab = ConditionEffectManager.instance.GetConditionEffect(conditionData.conditionType);
        //Debug.Log(conditionEffectPrefab);

        // プレファブが取得できたら
        //if (conditionEffectPrefab != null) {
        //    // エフェクト生成
        //    conditionEffect = Instantiate(conditionEffectPrefab, EffectManager.instance.effectConteinerTran);

        //    Debug.Log("エフェクト生成 : " + conditionType.ToString());
        //}

        //Debug.Log("コンディション付与");

        // 初期設定を実行
        //playerCondition.AddCondition(conditionData);

        // コンディション用の List に追加
        //mapMoveController.AddConditionsList(playerCondition);
    //}
}