using Cysharp.Threading.Tasks;
using System.Threading;

public class HealAllEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        int healPower = GameData.instance.charaStatus.MaxHp.Value;
        DebugLogger.Log($"healPower : {healPower}");

        int currentHp = BattleManager.instance.PlayerHP.Value;
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;

        if (currentHp < maxHp) {
            BattleManager.instance.UpdatePlayerHp(healPower, EffectType.Heal, false);
        } else {
            // Hp 最大値以上なら、シールドに加算
            BattleManager.instance.UpdatePlayerShieldHp(healPower, false);
        }

        // TODO デバフ解除


        // TODO ステータスバフ


        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}