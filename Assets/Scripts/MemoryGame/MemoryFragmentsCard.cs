/// <summary>
/// 思い出の断片カードモデルクラス
/// </summary>
[System.Serializable]
public class MemoryFragmentsCard : CardModelBase {
    public MemoryStoneData MemoryStoneData => cardData.masterData as MemoryStoneData;
    public MemoryFragmentsCard(CardData cardData, int cardIndex, ICardExecutor executor) : base(cardData, cardIndex, executor) { }
}