/// <summary>
/// 祝福カードモデルクラス
/// </summary>
[System.Serializable]
public class BlessingCard : CardModelBase {
    public BlessingData BlessingData => cardData.masterData as BlessingData;
    public BlessingCard(CardData cardData, int cardIndex, ICardExecutor executor) : base(cardData, cardIndex, executor) {}
}