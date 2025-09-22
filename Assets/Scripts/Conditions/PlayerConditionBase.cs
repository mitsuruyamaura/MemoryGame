using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using R3;
using System.Threading;

/// <summary>
/// コンディションのベースクラス
/// </summary>
[System.Serializable]
public class PlayerConditionBase// : MonoBehaviour
{
    public ReactiveProperty<int> ConditionDuration = new();　　//　持続時間

    [SerializeField]  // Debug
    protected float conditionValue;       //  効果  =>  攻撃力を増減する値、マップの見える範囲を増減する値

    //protected float effectInterval;

    //protected ConditionEffect conditionEffect;

    //protected MapMoveController mapMoveController;

    //protected ConditionType conditionType;   //  この情報が、コンディションの適用値になる


    [SerializeField] protected ConditionData conditionData;
    protected IDisposable disposable;
    protected CancellationToken token;

    /// <summary>
    /// コンディションをセットする際に呼び出す
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="value"></param>
    //public void AddCondition(ConditionType conditionType, float duration, float value, MapMoveController mapMoveController, float effectInterval) {
    //    this.conditionType = conditionType;
    //    conditionDuration = duration;
    //    conditionValue = value;
    //    this.mapMoveController = mapMoveController;
    //    this.effectInterval = effectInterval;



    //    OnEnterCondition();
    //}


    public virtual async UniTask AddCondition(ConditionData conditionData, CancellationToken token) {
        this.conditionData = conditionData;
        ConditionDuration.Value = this.conditionData.duration;
        conditionValue = this.conditionData.conditionValue;
        this.token = token;

        OnEnterCondition();

        // コンディションの種類で SE 選択
        PlayConditionSE();

        await ApplyEffect(this.token);    
    }

    /// <summary>
    /// コンディション共通の最初の処理
    /// </summary>
    /// <returns></returns>
    protected virtual void OnEnterCondition() {
        // 残り時間の購読。ターン終了時にイベント発火
        disposable = SymbolManager.instance.onTurnEnd.Subscribe(_ => CalcDuration());
    }

    /// <summary>
    /// コンディション固有効果を適用
    /// </summary>
    protected virtual async UniTask ApplyEffect(CancellationToken token) {

        // 毒のダメージ、攻撃力半減、移動速度半減などを適用する

        // 値を変化させる効果の場合は、持続時間経過後に OnExitCondition() を上書きして元の値に戻すこと

        await UniTask.Yield();
    }

    /// <summary>
    /// コンディションが終了するときに呼び出す
    /// </summary>
    public void RemoveCondition() {
        ApplyAfterEffect(token).Forget();
        OnExitCondition();
    }

    /// <summary>
    /// コンディション固有の終了時の処理
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual async UniTask ApplyAfterEffect(CancellationToken token) {
        await UniTask.Yield();
    }

    /// <summary>
    /// コンディション共通の終了時の処理
    /// </summary>
    protected virtual void OnExitCondition() {

        //if (conditionEffect != null) {
        //    // エフェクト破棄
        //    Destroy(conditionEffect.gameObject);
        //}

        DebugLogger.Log("コンディション削除");

        // コンディションの List から削除
        //mapMoveController.RemoveConditionsList(this);

        disposable?.Dispose();

        // ピュアクラスのため、List から削除すれば参照が切れるためガベージコレクションの対象となって、オブジェクトは自動的に削除される
        GameData.instance.RemoveConditionList(this);
    }

    /// <summary>
    /// コンディションの残り時間の更新
    /// </summary>
    public virtual void CalcDuration() {
        // 残り時間を減算
        ConditionDuration.Value--;
        DebugLogger.Log($"{this} 残り時間 : {ConditionDuration.Value}");

        // コンディションの残り時間がなくなったら
        if (ConditionDuration.Value <= 0) {
            // エフェクトやコンディションを削除して終了する
            RemoveCondition();
        }
    }

    /// <summary>
    /// 持続時間の延長と効果の上書き
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="value"></param>
    public void ExtentionCondition(int duration, float value) {
        ConditionDuration.Value += duration;
        conditionValue = value;

        PlayConditionSE();

        // コンディションの効果を適用
        //OnEnterCondition();
    }

    /// <summary>
    /// コンディションの効果値を取得
    /// </summary>
    /// <returns></returns>
    public float GetConditionValue() {
        return conditionValue;
    }

    /// <summary>
    /// 設定されているコンディションの種類を取得
    /// </summary>
    /// <returns></returns>
    public ConditionType GetConditionType() {
        return conditionData.conditionType;
    }

    public EnchantType GetEnchantType() {
        return conditionData.enchantType;
    }


    public void PlayConditionSE() {
        // コンディションの種類で SE 選択
        if (conditionData.enchantType == EnchantType.Buff) {
            SoundManager.instance.PlaySE(SE_TYPE.Buff);
        } else {
            SoundManager.instance.PlaySE(SE_TYPE.Debuff);
        }
    }
}