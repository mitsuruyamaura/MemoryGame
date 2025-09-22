using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class MemoryFragmentsCard : CardModelBase {
    public MemoryFragmentsCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("MemoryFragments");

        await UniTask.Yield(token);
    }
}
