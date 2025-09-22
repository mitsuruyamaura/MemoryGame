using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class BlessingCard : CardModelBase {
    public BlessingCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("blessing");

        await UniTask.Yield(token);
    }
}