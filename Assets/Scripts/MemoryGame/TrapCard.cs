/// <summary>
/// トラップカードモデルクラス
/// </summary>
[System.Serializable]
public class TrapCard : CardModelBase {
    public TrapData TrapData => cardData.masterData as TrapData;

    public TrapCard(CardData cardData, int cardIndex, ICardExecutor executor) : base(cardData, cardIndex, executor) {}
}