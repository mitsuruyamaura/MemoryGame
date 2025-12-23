/// <summary>
/// 宝箱カードモデルクラス
/// </summary>
[System.Serializable]
public class TreasureChestCard : CardModelBase {
    public ItemData ItemData => cardData.masterData as ItemData;
    public bool isEnemyDrop;

    public TreasureChestCard(CardData cardData, int cardIndex, ICardExecutor executor, bool isEnemyDrop) : base(cardData, cardIndex, executor) {
        this.isEnemyDrop = isEnemyDrop;
    }
}