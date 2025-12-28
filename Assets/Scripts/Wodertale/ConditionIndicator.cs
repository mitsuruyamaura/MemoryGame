using R3;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// コンディションの表示 UI
/// </summary>
public class ConditionIndicator : PoolBase, IPoolable {
    [SerializeField] protected Image imgIcon;
    [SerializeField] protected Text txtRemainingPower;
    [SerializeField] protected Text txtStackCount;
    [SerializeField] protected ConditionHoverUI conditionHoverUI;

    protected ConditionProgressData conditionProgressData;
    protected UnityAction<ConditionIndicator> releasedAction;
    protected IDisposable stackCountDisposable;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="conditionInfoDisplayManager"></param>
    /// <param name="conditionProgressData"></param>
    /// <param name="releasedAction">ConditionManager の ReleaseIndicator メソッド</param>
    public void SetUpIndiator(ConditionInfoDisplayManager conditionInfoDisplayManager, ConditionProgressData conditionProgressData, UnityAction<ConditionIndicator> releasedAction) {
        isReleased = false;

        Sprite sprite = DataBaseManager.instance.GetConditionIcon(conditionProgressData.ConditionData.id);
        if (sprite != null) {
            imgIcon.sprite = sprite;
        }

        this.conditionProgressData = conditionProgressData;
        this.releasedAction = releasedAction;

        disposable = conditionProgressData.RemainingPower.Subscribe(newRemainingPower => UpdateDisplayRemainingPower(newRemainingPower));
        stackCountDisposable = conditionProgressData.StackCount.Subscribe(newStackCount => UpdateDisplayStackCount(newStackCount));

        conditionHoverUI.Setup(conditionInfoDisplayManager, conditionProgressData);
    }

    /// <summary>
    /// 残り強度の表示更新
    /// </summary>
    /// <param name="remainingPower"></param>
    public void UpdateDisplayRemainingPower(int remainingPower) {
        if (remainingPower <= 0) {
            Release();
            return;
        }

        txtRemainingPower.text = remainingPower.ToString();
    }

    /// <summary>
    /// スタック数の表示更新
    /// </summary>
    /// <param name="newStackCount"></param>
    protected void UpdateDisplayStackCount(int newStackCount) {
        txtStackCount.text = newStackCount.ToString();
    }


    public bool HasCondition(ConditionProgressData target) => conditionProgressData == target;


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
            transform.SetParent(GameData.instance.transform);

            disposable?.Dispose();
            stackCountDisposable?.Dispose();

            objectPool.Release(this);
        }
    }
}