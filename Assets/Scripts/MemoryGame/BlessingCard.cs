using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class BlessingCard : CardModelBase {
    public BlessingCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        //DebugLogger.Log("blessing");

        if (cardData.masterData is BlessingData blessingData) {
            EventExecutor eventExecutor = new();
            await eventExecutor.ExecuteEventAsync(blessingData, token);
        }
    }
}