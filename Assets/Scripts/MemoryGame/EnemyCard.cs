/// <summary>
/// 敵カードモデルクラス
/// </summary>
[System.Serializable]
public class EnemyCard : CardModelBase {
    public EnemyData EnemyData => cardData.masterData as EnemyData;

    public EnemyCard(CardData cardData, int cardIndex, ICardExecutor executor) : base(cardData, cardIndex, executor) {}

    //public override async UniTask ExecuteCardAsync(CancellationToken token) {
    //    DebugLogger.Log("Enemy");

    //    if (cardData.masterData is EnemyData enemyData) {
    //        // 交渉の成功可否判定などをしてから、バトル開始
    //        await BattleManager.instance.StartBattleAsync(enemyData, TurnState.Player);
    //    }

    //    await UniTask.Yield(token);
    //}
}