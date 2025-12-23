using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 敵カードの実行処理(振る舞いを決める)クラス
/// </summary>
public class EnemyCardExecutor : ICardExecutor {
    private BattleExecutor battleExecutor;

    public EnemyCardExecutor(BattleExecutor battleExecutor) {
        this.battleExecutor = battleExecutor;
    }

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        if (card is not EnemyCard enemyCard) {
            DebugLogger.Log($"EnemyCard ではありません : {card.GetType().Name}");
            return;
        }

        await battleExecutor.ExecuteBattleAsync(enemyCard.EnemyData, token);
    }
}