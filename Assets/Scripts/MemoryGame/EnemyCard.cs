using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class EnemyCard : CardModelBase {
    public EnemyCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Enemy");

        if (cardData.masterData is EnemyData enemyData) {
            // 交渉の成功可否判定などをしてから、バトル開始
            await BattleManager.instance.StartBattleAsync(enemyData, TurnState.Player);
        }

        await UniTask.Yield(token);
    }
}