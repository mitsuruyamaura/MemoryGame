using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class TrapCard : CardModelBase {
    public TrapCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Trap");

        await UniTask.Yield(token);
    }
}