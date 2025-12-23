/// <summary>
/// 鍵カードモデルクラス
/// </summary>
[System.Serializable]
public class StairsCard : CardModelBase {
    public StairsCard(CardData cardData, int cardIndex, ICardExecutor executor) : base(cardData, cardIndex, executor) {}
}