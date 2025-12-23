using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// HP 回復イベント実行クラス
/// </summary>
public class HealEventExecutor : IEventExecutor {
    private BattleManager battleManager;
    public HealEventExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        float healRate = blessingData.value;
        int healPower = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * healRate);
        DebugLogger.Log($"healPower : {healPower} = {healRate} * {GameData.instance.charaStatus.MaxHp.Value}");

        int currentHp = battleManager.PlayerHP.Value;
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;
        DebugLogger.Log($"currentHp : {currentHp} / maxHp : {maxHp}");

        if (currentHp < maxHp) {
            battleManager.UpdatePlayerHp(healPower, EffectType.Heal, false);
        } else {
            // Hp 最大値以上なら、シールドに加算
            battleManager.UpdatePlayerShieldHp(healPower, false);
        }

        SoundManager.instance.PlaySE(SE_TYPE.Heal);
        await UniTask.Yield(token);
    }
}