using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// HP 全回復イベント実行クラス
/// </summary>
public class HealAllEventExecutor : IEventExecutor {
    private BattleManager battleManager;

    public HealAllEventExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        int healPower = GameData.instance.charaStatus.MaxHp.Value;
        DebugLogger.Log($"healPower : {healPower}");

        int currentHp = battleManager.PlayerHP.Value;
        int maxHp = GameData.instance.charaStatus.MaxHp.Value;

        if (currentHp < maxHp) {
            battleManager.UpdatePlayerHp(healPower, EffectType.Heal, false);
        } else {
            // Hp 最大値以上なら、シールドに加算
            battleManager.UpdatePlayerShieldHp(healPower, false);
        }

        // TODO デバフ解除


        // TODO ステータスバフ


        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}