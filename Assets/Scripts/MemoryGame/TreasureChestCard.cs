using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class TreasureChestCard : CardModelBase {
    public TreasureChestCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("TreasureChest");

        await UniTask.Yield(token);
    }
}