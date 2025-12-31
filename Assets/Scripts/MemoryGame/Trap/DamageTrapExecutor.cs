using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// ダメージ罠実行クラス
/// </summary>
public class DamageTrapExecutor : ITrapEffect {
    private BattleManager battleManager;
    public DamageTrapExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        DebugLogger.Log(trapActionData.trapId);

        if (trapActionData.trapDamageType == TrapDamageType.SetRemainingHpRate) {
            // ダメージ = 残り HP が指定％ になるようにする
            int delta = CalcSetRemainingHpRateDamage(trapActionData);

            // 指定 ％ 残すので、シールド適用なしで処理する
            battleManager.UpdatePlayerHp(delta, EffectType.Magic, false);
        } else {
            // 最大 Hp に指定割合をかけてダメージを算出
            int damage = CalcMaxHpRateDamage(trapActionData);

            // ダメージ(シールド適用可能)
            battleManager.UpdatePlayerHp(-damage, EffectType.Physical, false);
        }

        // Hp が 0 になったらゲームオーバー
        if (battleManager.PlayerHP.Value <= 0) {
            await battleManager.ForceGameEndAsync();
        }

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);        
    }

    /// <summary>
    /// 最大 Hp に指定割合をかけてダメージを算出
    /// </summary>
    /// <param name="trapActionData"></param>
    /// <returns></returns>
    private int CalcMaxHpRateDamage(TrapActionData trapActionData) {
        float damageRate = trapActionData.value;

        // 最大 Hp に指定割合をかけてダメージを算出
        int damage = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * damageRate);
        DebugLogger.Log($"damage : {damage} = {damageRate} * {GameData.instance.charaStatus.MaxHp.Value}");

        return damage;
    }

    /// <summary>
    /// ダメージ = 残り HP が指定％ になるようにする(シールド適用なしで使う)
    /// </summary>
    /// <param name="trapActionData"></param>
    /// <returns></returns>
    private int CalcSetRemainingHpRateDamage(TrapActionData trapActionData) {
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;
        int currentHp = battleManager.PlayerHP.Value;
        float damageRate = trapActionData.value;

        // 目標HP(処理後のHP)を計算
        int targetHp = Mathf.FloorToInt(maxHp * (1f - damageRate));
        targetHp = Mathf.Min(targetHp, currentHp);

        // delta = target - current(マイナスの値になる)
        int delta = targetHp - currentHp;
        DebugLogger.Log($"Trap: max={maxHp}, current={currentHp}, value={damageRate}, target={targetHp}, delta={delta}");

        return delta;
    }
}