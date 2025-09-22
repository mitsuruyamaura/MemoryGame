using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Linq;

public class ConditionItemSymbol : SymbolBase {

    [SerializeField] private ConditionType conditionType;

    [SerializeField, Header("持続時間")] private int duration;

    [SerializeField, Header("効果値")] private float itemValue;

    [SerializeField] private SpriteRenderer srIcon;

    private ConditionData   conditionData;


    public override void OnEnterSymbol() {
        base.OnEnterSymbol();

        conditionData = DataBaseManager.instance.conditionDataSO.conditionDatasList.FirstOrDefault(x=> x.conditionType == conditionType);

        duration = conditionData.duration;
        itemValue = conditionData.conditionValue;

        srIcon.sprite = conditionData.conditionIconSprite;

        // フィールドでのエフェクト演出

    }


    public override async UniTask TriggerEffectAsync() {
        await base.TriggerEffectAsync();

        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InExpo).SetLink(gameObject);

        // すでに同じコンディションが付与されているか確認
        if (GameData.instance.JudgeConditionType(conditionType)) {
            // すでに付与されている場合は、持続時間を更新し、効果は上書きして処理を終了する
            GameData.instance.UpdateConditionDuration(conditionData);
            return;
        }

        // 付与するコンディションが睡眠かつ、すでに混乱のコンディションが付与されているときには、睡眠のコンディションは無視する(操作不能になるため)
        if (conditionType == ConditionType.Sleep && GameData.instance.JudgeConditionType(ConditionType.Confusion)) {
            return;
        }

        // 付与されていないコンディションの場合は、付与するコンディションを設定する
        PlayerConditionBase playerCondition = conditionType switch {
            ConditionType.View_Wide => new PlayerCondition_View(),
            ConditionType.Untouchable => new PlayerCondition_Untouchable(),
            _ => null
        };

        if(playerCondition == null) {
            DebugLogger.Log("該当するコンディションなし");
            return;
        }

        // Player にコンディションを付与。SE はこの中で鳴らす
        await playerCondition.AddCondition(conditionData, SymbolManager.instance.cts.Token);

        GameData.instance.AddConditionList(playerCondition);

        // コンディションアイコンを生成し、残り時間をセット
        ConditionManager.instance.GenerateConditionIndicator(conditionData, playerCondition);

        await UniTask.Delay(500);
        Release();
    }

    // 未使用
    //public override void TriggerAppearEffect(MapMoveController mapMoveController) {

        //base.TriggerAppearEffect(mapMoveController);

        // 獲得時のエフェクト演出


        //transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InExpo).SetLink(gameObject);

        //// すでに同じコンディションが付与されているか確認
        //if (mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == conditionType)) {
        //    // すでに付与されている場合は、持続時間を更新し、効果は上書きして処理を終了する
        //    mapMoveController.GetConditionsList().Find(x => x.GetConditionType() == conditionType).ExtentionCondition(duration, itemValue);
        //    return;
        //}

        //// 付与するコンディションが睡眠かつ、すでに混乱のコンディションが付与されているときには、睡眠のコンディションは無視する(操作不能になるため)
        //if (conditionType == ConditionType.Sleep &&  mapMoveController.GetConditionsList().Exists(x => x.GetConditionType() == ConditionType.Confusion)) {
        //    return;
        //}

        //// 付与されていないコンディションの場合は、付与する準備する
        //PlayerConditionBase playerCondition = new();

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

        // 初期設定を実行
        //playerCondition.AddCondition(conditionType, duration, itemValue, mapMoveController, conditionData.effectInterval);
        //playerCondition.AddCondition(conditionData);

        // コンディション用の List に追加
        //mapMoveController.AddConditionsList(playerCondition);

        //base.OnExitSymbol();
        //Release();
    //}
}
