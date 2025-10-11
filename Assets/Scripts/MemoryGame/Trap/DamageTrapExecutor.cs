using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DamageTrapExecutor : ITrap {
    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        DebugLogger.Log(trapData.id);
        float damageRate = trapData.value;
        int damage = 0;

        if (trapData.valueType == TrapValueType.Rate) {
            int maxHp = GameData.instance.debugMaxHp;
            int currentHp = BattleManager.instance.PlayerHP.Value;

            // 目標HP(処理後のHP)を計算
            int targetHp = Mathf.FloorToInt(maxHp * (1f - damageRate));
            targetHp = Mathf.Min(targetHp, currentHp);

            // delta = target - current(マイナスの値になる)
            int delta = targetHp - currentHp;
            DebugLogger.Log($"Trap: max={maxHp}, current={currentHp}, value={damageRate}, target={targetHp}, delta={delta}");

            // ダメージ = 残り HP が指定％ になるようにする(シールド適用なし)
            BattleManager.instance.UpdatePlayerHp(delta, EffectType.Magic, false);
        } else {
            // 最大 Hp に指定割合をかけてダメージを算出
            damage = Mathf.FloorToInt(GameData.instance.debugMaxHp * damageRate);
            DebugLogger.Log($"damage : {damage} = {damageRate} * {GameData.instance.debugMaxHp}");

            // 現在 Hp よりも大きい場合には1残す 
            //int currentHp = BattleManager.instance.PlayerHP.Value;
            //if (currentHp <= damage) {
            //    damage = currentHp - 1; 
            //}   

            // ダメージ(シールド適用可能)
            BattleManager.instance.UpdatePlayerHp(-damage, EffectType.Physical, false);
        }

        if (BattleManager.instance.PlayerHP.Value <= 0) {
            StageUIManager stageUIManager = GameObject.FindFirstObjectByType<StageUIManager>();
            stageUIManager.ShowBattleState(BattleResultType.Lose);
            BattleManager.instance.SetBattleResultType(BattleResultType.Lose);
            await BattleManager.instance.BattleResultAsync(null);
        }

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);
        await UniTask.Yield(token);
    }
}