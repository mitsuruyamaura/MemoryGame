using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConditionIndicator : PoolBase, IPoolable {

    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtDuration;
    private UnityAction<ConditionIndicator> releasedAction;


    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="conditionData"></param>
    /// <param name="playerCondition"></param>
    /// <param name="releasedAction">ConditionManager の ReleaseIndicator メソッド</param>
    public void SetUpIndiator(ConditionData conditionData, PlayerConditionBase playerCondition, UnityAction<ConditionIndicator> releasedAction) {
        isReleased = false;
        imgIcon.sprite = conditionData.conditionIconSprite;
        this.releasedAction = releasedAction;

        disposable = playerCondition.ConditionDuration.Subscribe(duration => UpdateDuration(duration));
    }

    /// <summary>
    /// 残り時間の表示更新
    /// </summary>
    /// <param name="duration"></param>
    public void UpdateDuration(int duration) {
        if (duration <= 0) {
            Release();
            return;
        }

        txtDuration.text = duration.ToString();
    }

    public override void Release() {
        if (isReleased) {
            DebugLogger.Log("This object has already been released.");
            return;
        }

        if (gameObject.activeInHierarchy) {
            isReleased = true;

            // List から削除する
            releasedAction?.Invoke(this);

            // 一旦 Content の配下から抜く。
            // そうしないと、オブジェクトプールのあった位置に再表示されるため、新しく追加したものの位置が正しく表示されない
            transform.SetParent(ConditionManager.instance.transform);

            disposable?.Dispose();
            objectPool.Release(this);
        }
    }
}