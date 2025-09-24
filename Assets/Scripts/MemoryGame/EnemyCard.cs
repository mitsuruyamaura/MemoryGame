using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class EnemyCard : CardModelBase {
    public EnemyCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Enemy");

        if (cardData.masterData is EnemyData enemyData) {
            BattleResultType battleResultType = await BattleManager.instance.StartBattle(enemyData, TurnState.Player);

            // TODO OnNext で StageLogicManager の購読をキックする


            if (battleResultType == BattleResultType.Win) {
                // 敵のレアリティから同レアリティのアイテムテータを抽選
                ItemData itemData = DataBaseManager.instance.GetRandomItemByEnemyDrop(enemyData.rarity);

                // カードの種類とマスターデータを設定
                CardData cardData = new() {
                    cardTypeMaster = DataBaseManager.instance.GetCardType(CardEventType.TreasureChest),
                    masterData = itemData
                };

                // 宝箱カードの生成、カードの効果を実行
                TreasureChestCard treasureChestCard = new(cardData);
                await treasureChestCard.ExecuteCardAsync(token);

                GameData.instance.CurrentGameState.Value = GameData.GameState.Play;
            } else if (battleResultType == BattleResultType.Lose) {
                DebugLogger.Log($"Game Over");
                GameData.instance.CurrentGameState.Value = GameData.GameState.GameUp;
            } else if (battleResultType == BattleResultType.Timeout) {
                DebugLogger.Log($"Timeout");
                GameData.instance.CurrentGameState.Value = GameData.GameState.Play;
            }
        }

        await UniTask.Yield(token);
    }
}