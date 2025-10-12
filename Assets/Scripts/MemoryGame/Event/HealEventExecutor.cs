using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class HealEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        float healRate = blessingData.value;
        int healPower = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * healRate);
        DebugLogger.Log($"healPower : {healPower} = {healRate} * {GameData.instance.charaStatus.MaxHp.Value}");

        int currentHp = BattleManager.instance.PlayerHP.Value;
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;
        DebugLogger.Log($"currentHp : {currentHp} / maxHp : {maxHp}");

        if (currentHp < maxHp) {
            BattleManager.instance.UpdatePlayerHp(healPower, EffectType.Heal, false);
        } else {
            // Hp 最大値以上なら、シールドに加算
            BattleManager.instance.UpdatePlayerShieldHp(healPower, false);
        }

        SoundManager.instance.PlaySE(SE_TYPE.Heal);
        await UniTask.Yield(token);
    }
}