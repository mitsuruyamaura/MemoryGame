using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// ダメージ罠実行クラス
/// </summary>
public class DamageTrapExecutor : ITrap {
    private BattleManager battleManager;
    public DamageTrapExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        DebugLogger.Log(trapData.id);
        float damageRate = trapData.value;
        int damage = 0;

        if (trapData.valueType == TrapValueType.Rate) {
            int maxHp = GameData.instance.charaStatus.MaxHp.Value;
            int currentHp = battleManager.PlayerHP.Value;

            // 目標HP(処理後のHP)を計算
            int targetHp = Mathf.FloorToInt(maxHp * (1f - damageRate));
            targetHp = Mathf.Min(targetHp, currentHp);

            // delta = target - current(マイナスの値になる)
            int delta = targetHp - currentHp;
            DebugLogger.Log($"Trap: max={maxHp}, current={currentHp}, value={damageRate}, target={targetHp}, delta={delta}");

            // ダメージ = 残り HP が指定％ になるようにする(シールド適用なし)
            battleManager.UpdatePlayerHp(delta, EffectType.Magic, false);
        } else {
            // 最大 Hp に指定割合をかけてダメージを算出
            damage = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * damageRate);
            DebugLogger.Log($"damage : {damage} = {damageRate} * {GameData.instance.charaStatus.MaxHp.Value}");

            // 現在 Hp よりも大きい場合には1残す 
            //int currentHp = BattleManager.instance.PlayerHP.Value;
            //if (currentHp <= damage) {
            //    damage = currentHp - 1; 
            //}   

            // ダメージ(シールド適用可能)
            battleManager.UpdatePlayerHp(-damage, EffectType.Physical, false);
        }

        // Hp が 0 になったらゲームオーバー
        if (battleManager.PlayerHP.Value <= 0) {
            await battleManager.ForceGameEndAsync();
        }

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);        
    }
}